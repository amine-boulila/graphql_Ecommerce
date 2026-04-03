using GraphQLApi.Data;
using GraphQLApi.Models;
using GraphQLApi.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace GraphQLApi.GraphQL;

// Exposes all read operations of the GraphQL API.
public class Query
{
    // CATEGORIES
    public async Task<List<CategorieType>> GetCategories(MongoDbContext context)
    {
        var categories = await context.Categories.Find(FilterDefinition<Categorie>.Empty).ToListAsync();
        return categories.Select(CategorieType.FromModel).ToList();
    }

    public async Task<CategorieType?> GetCategorie(string id, MongoDbContext context)
    {
        var categorie = await context.Categories.Find(c => c.Id == id).FirstOrDefaultAsync();
        return categorie != null ? CategorieType.FromModel(categorie) : null;
    }

    // PRODUITS
    public async Task<List<ProduitType>> GetProduits(MongoDbContext context)
    {
        var produits = await context.Produits.Find(FilterDefinition<Produit>.Empty).ToListAsync();
        return produits.Select(ProduitType.FromModel).ToList();
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
        var filters = new List<FilterDefinition<Produit>>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var escapedSearch = Regex.Escape(search);
            var searchFilter = Builders<Produit>.Filter.Or(
                Builders<Produit>.Filter.Regex(
                    p => p.Nom,
                    new BsonRegularExpression(escapedSearch, "i")),
                Builders<Produit>.Filter.Regex(
                    p => p.Description,
                    new BsonRegularExpression(escapedSearch, "i")));

            filters.Add(searchFilter);
        }

        if (!string.IsNullOrWhiteSpace(categorieId))
        {
            filters.Add(Builders<Produit>.Filter.Eq(p => p.CategorieId, categorieId));
        }

        if (minPrix.HasValue)
        {
            filters.Add(Builders<Produit>.Filter.Gte(p => p.Prix, minPrix.Value));
        }

        if (maxPrix.HasValue)
        {
            filters.Add(Builders<Produit>.Filter.Lte(p => p.Prix, maxPrix.Value));
        }

        var filter = filters.Count == 0
            ? FilterDefinition<Produit>.Empty
            : Builders<Produit>.Filter.And(filters);

        var sort = BuildProduitSort(sortBy, sortDirection);

        skip = Math.Max(0, skip);
        take = Math.Clamp(take, 1, 50);

        var totalCount = (int)await context.Produits.CountDocumentsAsync(filter);
        var produits = await context.Produits
            .Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(take)
            .ToListAsync();

        var items = produits.Select(ProduitType.FromModel).ToList();

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
        var produit = await context.Produits.Find(p => p.Id == id).FirstOrDefaultAsync();
        return produit != null ? ProduitType.FromModel(produit) : null;
    }

    public async Task<List<ProduitType>> GetProduitsByCategorie(string categorieId, MongoDbContext context)
    {
        var produits = await context.Produits.Find(p => p.CategorieId == categorieId).ToListAsync();
        return produits.Select(ProduitType.FromModel).ToList();
    }

    // UTILISATEURS
    public async Task<List<UtilisateurType>> GetUtilisateurs(MongoDbContext context)
    {
        var utilisateurs = await context.Utilisateurs.Find(FilterDefinition<Utilisateur>.Empty).ToListAsync();
        return utilisateurs.Select(UtilisateurType.FromModel).ToList();
    }

    public async Task<UtilisateurType?> GetUtilisateur(string id, MongoDbContext context)
    {
        var utilisateur = await context.Utilisateurs.Find(u => u.Id == id).FirstOrDefaultAsync();
        return utilisateur != null ? UtilisateurType.FromModel(utilisateur) : null;
    }

    // COMMANDES
    public async Task<List<CommandeType>> GetCommandes(MongoDbContext context)
    {
        var commandes = await context.Commandes.Find(FilterDefinition<Commande>.Empty).ToListAsync();
        return commandes.Select(CommandeType.FromModel).ToList();
    }

    public async Task<CommandeType?> GetCommande(string id, MongoDbContext context)
    {
        var commande = await context.Commandes.Find(c => c.Id == id).FirstOrDefaultAsync();
        return commande != null ? CommandeType.FromModel(commande) : null;
    }

    public async Task<List<CommandeType>> GetCommandesByUtilisateur(string utilisateurId, MongoDbContext context)
    {
        var commandes = await context.Commandes.Find(c => c.UtilisateurId == utilisateurId).ToListAsync();
        return commandes.Select(CommandeType.FromModel).ToList();
    }

    // AVIS
    public async Task<List<AvisType>> GetAvis(MongoDbContext context)
    {
        var avis = await context.Avis.Find(FilterDefinition<Avis>.Empty).ToListAsync();
        return avis.Select(AvisType.FromModel).ToList();
    }

    public async Task<List<AvisType>> GetAvisByProduit(string produitId, MongoDbContext context)
    {
        var avis = await context.Avis.Find(a => a.ProduitId == produitId).ToListAsync();
        return avis.Select(AvisType.FromModel).ToList();
    }

    private static SortDefinition<Produit> BuildProduitSort(
        ProduitSortField sortBy,
        ProduitSortDirection sortDirection)
    {
        var descending = sortDirection == ProduitSortDirection.Desc;

        return (sortBy, descending) switch
        {
            (ProduitSortField.Prix, true) => Builders<Produit>.Sort.Descending(p => p.Prix),
            (ProduitSortField.Prix, false) => Builders<Produit>.Sort.Ascending(p => p.Prix),
            (ProduitSortField.Stock, true) => Builders<Produit>.Sort.Descending(p => p.Stock),
            (ProduitSortField.Stock, false) => Builders<Produit>.Sort.Ascending(p => p.Stock),
            (ProduitSortField.CreatedAt, true) => Builders<Produit>.Sort.Descending(p => p.CreatedAt),
            (ProduitSortField.CreatedAt, false) => Builders<Produit>.Sort.Ascending(p => p.CreatedAt),
            (ProduitSortField.Nom, true) => Builders<Produit>.Sort.Descending(p => p.Nom),
            _ => Builders<Produit>.Sort.Ascending(p => p.Nom)
        };
    }
}
