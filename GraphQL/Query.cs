using MongoDB.Driver;
using GraphQLApi.Data;
using GraphQLApi.Types;

namespace GraphQLApi.GraphQL;

// Exposes all read operations of the GraphQL API.
public class Query
{
    // CATEGORIES
    public async Task<List<CategorieType>> GetCategories(MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        return graph.Categories;
    }

    public async Task<CategorieType?> GetCategorie(string id, MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        graph.CategoriesById.TryGetValue(id, out var categorie);
        return categorie;
    }

    // PRODUITS
    public async Task<List<ProduitType>> GetProduits(MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        return graph.Produits;
    }

    public async Task<ProduitPageType> GetProduitsPage(
        MongoDbContext context,
        string? search = null,
        string? categorieId = null,
        decimal? minPrix = null,
        decimal? maxPrix = null,
        ProduitSortField sortBy = ProduitSortField.Nom,
        ProduitSortDirection sortDirection = ProduitSortDirection.Asc,
        int skip = 0,
        int take = 10)
    {
        // Build the in-memory graph first, then apply filtering and paging on products.
        var graph = await BuildGraphAsync(context);
        var query = graph.Produits.AsEnumerable();

        // Free-text search on product name and description.
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Nom.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        // Optional filtering by category.
        if (!string.IsNullOrWhiteSpace(categorieId))
        {
            query = query.Where(p => p.CategorieId == categorieId);
        }

        // Optional price range filters.
        if (minPrix.HasValue)
        {
            query = query.Where(p => p.Prix >= minPrix.Value);
        }

        if (maxPrix.HasValue)
        {
            query = query.Where(p => p.Prix <= maxPrix.Value);
        }

        // Sort the result according to the requested field and direction.
        query = (sortBy, sortDirection) switch
        {
            (ProduitSortField.Prix, ProduitSortDirection.Desc) => query.OrderByDescending(p => p.Prix),
            (ProduitSortField.Prix, _) => query.OrderBy(p => p.Prix),
            (ProduitSortField.Stock, ProduitSortDirection.Desc) => query.OrderByDescending(p => p.Stock),
            (ProduitSortField.Stock, _) => query.OrderBy(p => p.Stock),
            (ProduitSortField.CreatedAt, ProduitSortDirection.Desc) => query.OrderByDescending(p => p.CreatedAt),
            (ProduitSortField.CreatedAt, _) => query.OrderBy(p => p.CreatedAt),
            (ProduitSortField.Nom, ProduitSortDirection.Desc) => query.OrderByDescending(p => p.Nom),
            _ => query.OrderBy(p => p.Nom)
        };

        // Keep pagination values within safe bounds for the API.
        skip = Math.Max(0, skip);
        take = Math.Clamp(take, 1, 50);

        var totalCount = query.Count();
        var items = query
            .Skip(skip)
            .Take(take)
            .ToList();

        // Return both the current page items and pagination metadata.
        return new ProduitPageType
        {
            Items = items,
            TotalCount = totalCount,
            Skip = skip,
            Take = take,
            HasPreviousPage = skip > 0,
            HasNextPage = skip + items.Count < totalCount
        };
    }

    public async Task<ProduitType?> GetProduit(string id, MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        graph.ProduitsById.TryGetValue(id, out var produit);
        return produit;
    }

    public async Task<List<ProduitType>> GetProduitsByCategorie(string categorieId, MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        return graph.Produits
            .Where(p => p.CategorieId == categorieId)
            .ToList();
    }

    // UTILISATEURS
    public async Task<List<UtilisateurType>> GetUtilisateurs(MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        return graph.Utilisateurs;
    }

    public async Task<UtilisateurType?> GetUtilisateur(string id, MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        graph.UtilisateursById.TryGetValue(id, out var utilisateur);
        return utilisateur;
    }

    // COMMANDES
    public async Task<List<CommandeType>> GetCommandes(MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        return graph.Commandes;
    }

    public async Task<CommandeType?> GetCommande(string id, MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        graph.CommandesById.TryGetValue(id, out var commande);
        return commande;
    }

    public async Task<List<CommandeType>> GetCommandesByUtilisateur(string utilisateurId, MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        return graph.Commandes
            .Where(c => c.UtilisateurId == utilisateurId)
            .ToList();
    }

    // AVIS
    public async Task<List<AvisType>> GetAvis(MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        return graph.Avis;
    }

    public async Task<List<AvisType>> GetAvisByProduit(string produitId, MongoDbContext context)
    {
        var graph = await BuildGraphAsync(context);
        return graph.Avis
            .Where(a => a.ProduitId == produitId)
            .ToList();
    }

    // Loads all collections, maps them to GraphQL types, then reconnects the references
    // so nested GraphQL queries can navigate the object graph easily.
    private static async Task<GraphData> BuildGraphAsync(MongoDbContext context)
    {
        // Load collections in parallel to reduce waiting time.
        var categoriesTask = context.Categories.Find(_ => true).ToListAsync();
        var produitsTask = context.Produits.Find(_ => true).ToListAsync();
        var utilisateursTask = context.Utilisateurs.Find(_ => true).ToListAsync();
        var commandesTask = context.Commandes.Find(_ => true).ToListAsync();
        var avisTask = context.Avis.Find(_ => true).ToListAsync();

        await Task.WhenAll(categoriesTask, produitsTask, utilisateursTask, commandesTask, avisTask);

        // Convert MongoDB models into the GraphQL types exposed by the API.
        var categories = categoriesTask.Result.Select(CategorieType.FromModel).ToList();
        var produits = produitsTask.Result.Select(ProduitType.FromModel).ToList();
        var utilisateurs = utilisateursTask.Result.Select(UtilisateurType.FromModel).ToList();
        var commandes = commandesTask.Result.Select(CommandeType.FromModel).ToList();
        var avis = avisTask.Result.Select(AvisType.FromModel).ToList();

        // Build lookup dictionaries to reconnect relationships efficiently by id.
        var categoriesById = categories.ToDictionary(c => c.Id);
        var produitsById = produits.ToDictionary(p => p.Id);
        var utilisateursById = utilisateurs.ToDictionary(u => u.Id);
        var commandesById = commandes.ToDictionary(c => c.Id);

        // Attach each order to its user and each order line to its product.
        foreach (var commande in commandes)
        {
            utilisateursById.TryGetValue(commande.UtilisateurId, out var utilisateur);
            commande.Utilisateur = utilisateur;

            foreach (var ligne in commande.Lignes)
            {
                produitsById.TryGetValue(ligne.ProduitId, out var produit);
                ligne.Produit = produit;
            }
        }

        // Attach each review to both its product and its author.
        foreach (var avisItem in avis)
        {
            produitsById.TryGetValue(avisItem.ProduitId, out var produit);
            avisItem.Produit = produit;

            utilisateursById.TryGetValue(avisItem.UtilisateurId, out var utilisateur);
            avisItem.Utilisateur = utilisateur;
        }

        // Attach the category, reviews, and related orders to each product.
        foreach (var produit in produits)
        {
            categoriesById.TryGetValue(produit.CategorieId, out var categorie);
            produit.Categorie = categorie;

            produit.Avis = avis.Where(a => a.ProduitId == produit.Id).ToList();
            produit.Commandes = commandes
                .Where(c => c.Lignes.Any(l => l.ProduitId == produit.Id))
                .ToList();
        }

        // Attach the list of products to each category.
        foreach (var categorie in categories)
        {
            categorie.Produits = produits
                .Where(p => p.CategorieId == categorie.Id)
                .ToList();
        }

        // Attach orders and reviews to each user.
        foreach (var utilisateur in utilisateurs)
        {
            utilisateur.Commandes = commandes
                .Where(c => c.UtilisateurId == utilisateur.Id)
                .ToList();

            utilisateur.Avis = avis
                .Where(a => a.UtilisateurId == utilisateur.Id)
                .ToList();
        }

        return new GraphData(
            categories,
            produits,
            utilisateurs,
            commandes,
            avis,
            categoriesById,
            produitsById,
            utilisateursById,
            commandesById);
    }

    // Small container used to return the fully connected in-memory graph.
    private sealed record GraphData(
        List<CategorieType> Categories,
        List<ProduitType> Produits,
        List<UtilisateurType> Utilisateurs,
        List<CommandeType> Commandes,
        List<AvisType> Avis,
        Dictionary<string, CategorieType> CategoriesById,
        Dictionary<string, ProduitType> ProduitsById,
        Dictionary<string, UtilisateurType> UtilisateursById,
        Dictionary<string, CommandeType> CommandesById);
}
