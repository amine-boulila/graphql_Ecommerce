using MongoDB.Driver;
using GraphQLApi.Data;
using GraphQLApi.Models;
using GraphQLApi.Types;
using HotChocolate.Subscriptions;

namespace GraphQLApi.GraphQL;

public class Mutation
{
    // CATEGORIES
    public async Task<CategorieType> CreateCategorie(string nom, string description, MongoDbContext context)
    {
        var categorie = new Categorie { Nom = nom, Description = description };
        await context.Categories.InsertOneAsync(categorie);
        return CategorieType.FromModel(categorie);
    }

    public async Task<CategorieType?> UpdateCategorie(string id, string nom, string description, MongoDbContext context)
    {
        var update = Builders<Categorie>.Update
            .Set(c => c.Nom, nom)
            .Set(c => c.Description, description);

        var result = await context.Categories.FindOneAndUpdateAsync(c => c.Id == id, update);
        return result != null ? CategorieType.FromModel(result) : null;
    }

    public async Task<bool> DeleteCategorie(string id, MongoDbContext context)
    {
        var result = await context.Categories.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    // PRODUITS
    public async Task<ProduitType> CreateProduit(
        string nom, string description, decimal prix, int stock, string categorieId, string? image, MongoDbContext context)
    {
        var produit = new Produit
        {
            Nom = nom,
            Description = description,
            Prix = prix,
            Stock = stock,
            CategorieId = categorieId,
            Image = image
        };
        await context.Produits.InsertOneAsync(produit);
        var produitType = ProduitType.FromModel(produit);
        var categorie = await context.Categories.Find(c => c.Id == produit.CategorieId).FirstOrDefaultAsync();
        produitType.Categorie = categorie != null ? CategorieType.FromModel(categorie) : null;
        return produitType;
    }

    public async Task<ProduitType?> UpdateProduit(
        string id, string? nom, string? description, decimal? prix, int? stock, MongoDbContext context)
    {
        var update = Builders<Produit>.Update;
        var updateDef = Builders<Produit>.Update.Set(p => p.UpdatedAt, DateTime.UtcNow);

        if (!string.IsNullOrEmpty(nom))
            updateDef = update.Combine(updateDef, update.Set(p => p.Nom, nom));
        if (!string.IsNullOrEmpty(description))
            updateDef = update.Combine(updateDef, update.Set(p => p.Description, description));
        if (prix.HasValue)
            updateDef = update.Combine(updateDef, update.Set(p => p.Prix, prix.Value));
        if (stock.HasValue)
            updateDef = update.Combine(updateDef, update.Set(p => p.Stock, stock.Value));

        var result = await context.Produits.FindOneAndUpdateAsync(p => p.Id == id, updateDef);
        if (result == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(nom))
            result.Nom = nom;
        if (!string.IsNullOrEmpty(description))
            result.Description = description;
        if (prix.HasValue)
            result.Prix = prix.Value;
        if (stock.HasValue)
            result.Stock = stock.Value;
        result.UpdatedAt = DateTime.UtcNow;

        var produitType = ProduitType.FromModel(result);
        var categorie = await context.Categories.Find(c => c.Id == result.CategorieId).FirstOrDefaultAsync();
        produitType.Categorie = categorie != null ? CategorieType.FromModel(categorie) : null;
        return produitType;
    }

    public async Task<bool> DeleteProduit(string id, MongoDbContext context)
    {
        var result = await context.Produits.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    // UTILISATEURS
    public async Task<UtilisateurType> CreateUtilisateur(
        string nom, string prenom, string email, string? telephone, MongoDbContext context)
    {
        var utilisateur = new Utilisateur
        {
            Nom = nom,
            Prenom = prenom,
            Email = email,
            Telephone = telephone
        };
        await context.Utilisateurs.InsertOneAsync(utilisateur);
        return UtilisateurType.FromModel(utilisateur);
    }

    public async Task<UtilisateurType?> UpdateUtilisateur(
        string id, string? nom, string? prenom, MongoDbContext context)
    {
        var updates = new List<UpdateDefinition<Utilisateur>>();

        if (!string.IsNullOrEmpty(nom))
            updates.Add(Builders<Utilisateur>.Update.Set(u => u.Nom, nom));
        if (!string.IsNullOrEmpty(prenom))
            updates.Add(Builders<Utilisateur>.Update.Set(u => u.Prenom, prenom));

        // ✅ Validation: au moins un champ doit être fourni
        if (updates.Count == 0)
            return null;

        var updateDef = Builders<Utilisateur>.Update.Combine(updates);
        var result = await context.Utilisateurs.FindOneAndUpdateAsync(u => u.Id == id, updateDef);
        return result != null ? UtilisateurType.FromModel(result) : null;
    }

    // COMMANDES
    public async Task<CommandeType?> CreateCommande(
        string utilisateurId,
        List<LigneCommandeInput> lignes,
        MongoDbContext context,
        ITopicEventSender eventSender)
    {
        var lignesCommande = lignes.Select(l => new LigneCommande
        {
            ProduitId = l.ProduitId,
            Quantite = l.Quantite,
            PrixUnitaire = l.PrixUnitaire
        }).ToList();

        var commande = new Commande
        {
            Numero = $"CMD-{DateTime.UtcNow.Ticks}",
            UtilisateurId = utilisateurId,
            Lignes = lignesCommande,
            MontantTotal = lignesCommande.Sum(l => l.SousTotal)
        };

        await context.Commandes.InsertOneAsync(commande);
        var commandeType = await EnrichCommandeAsync(CommandeType.FromModel(commande), context);
        await eventSender.SendAsync(CommandeSubscriptionTopics.CommandeCreated, commandeType);
        return commandeType;
    }

    public async Task<CommandeType?> UpdateCommandeStatut(
        string id,
        string statut,
        MongoDbContext context,
        ITopicEventSender eventSender)
    {
        var update = Builders<Commande>.Update
            .Set(c => c.Statut, statut)
            .Set(c => c.DateLivraison, statut == "Livrée" ? DateTime.UtcNow : null);

        var result = await context.Commandes.FindOneAndUpdateAsync(c => c.Id == id, update);
        if (result == null)
        {
            return null;
        }

        result.Statut = statut;
        result.DateLivraison = statut == "Livrée" ? DateTime.UtcNow : null;

        var commandeType = await EnrichCommandeAsync(CommandeType.FromModel(result), context);
        await eventSender.SendAsync(CommandeSubscriptionTopics.CommandeUpdated, commandeType);
        return commandeType;
    }

    public async Task<bool> DeleteCommande(string id, MongoDbContext context, ITopicEventSender eventSender)
    {
        var commande = await context.Commandes.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (commande == null)
        {
            return false;
        }

        var result = await context.Commandes.DeleteOneAsync(c => c.Id == id);
        if (result.DeletedCount == 0)
        {
            return false;
        }

        await eventSender.SendAsync(CommandeSubscriptionTopics.CommandeDeleted, CommandeType.FromModel(commande));
        return true;
    }

    // AVIS
    public async Task<AvisType> CreateAvis(
        string produitId, string utilisateurId, int note, string titre, string comment, MongoDbContext context)
    {
        var avis = new Avis
        {
            ProduitId = produitId,
            UtilisateurId = utilisateurId,
            Note = note,
            Titre = titre,
            Comment = comment
        };
        await context.Avis.InsertOneAsync(avis);
        var avisType = AvisType.FromModel(avis);

        var produit = await context.Produits.Find(p => p.Id == avis.ProduitId).FirstOrDefaultAsync();
        if (produit != null)
        {
            avisType.Produit = ProduitType.FromModel(produit);
        }

        var utilisateur = await context.Utilisateurs.Find(u => u.Id == avis.UtilisateurId).FirstOrDefaultAsync();
        if (utilisateur != null)
        {
            avisType.Utilisateur = UtilisateurType.FromModel(utilisateur);
        }

        return avisType;
    }

    public async Task<bool> DeleteAvis(string id, MongoDbContext context)
    {
        var result = await context.Avis.DeleteOneAsync(a => a.Id == id);
        return result.DeletedCount > 0;
    }

    private static async Task<CommandeType> EnrichCommandeAsync(CommandeType commandeType, MongoDbContext context)
    {
        var utilisateur = await context.Utilisateurs
            .Find(u => u.Id == commandeType.UtilisateurId)
            .FirstOrDefaultAsync();

        if (utilisateur != null)
        {
            commandeType.Utilisateur = UtilisateurType.FromModel(utilisateur);
        }

        var produitIds = commandeType.Lignes
            .Select(l => l.ProduitId)
            .Distinct()
            .ToList();

        if (produitIds.Count == 0)
        {
            return commandeType;
        }

        var produits = await context.Produits
            .Find(p => produitIds.Contains(p.Id))
            .ToListAsync();

        var produitsById = produits.ToDictionary(p => p.Id, ProduitType.FromModel);

        foreach (var ligne in commandeType.Lignes)
        {
            produitsById.TryGetValue(ligne.ProduitId, out var produit);
            ligne.Produit = produit;
        }

        return commandeType;
    }
}

public class LigneCommandeInput
{
    public string ProduitId { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
}
