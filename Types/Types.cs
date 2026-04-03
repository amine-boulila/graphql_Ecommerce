using GraphQLApi.Models;
using HotChocolate;

namespace GraphQLApi.Types;

public class CategorieType
{
    public string Id { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Relations
    [GraphQLIgnore]
    public List<ProduitType> Produits { get; set; } = new();

    public static CategorieType FromModel(Categorie cat) => new()
    {
        Id = cat.Id,
        Nom = cat.Nom,
        Description = cat.Description,
        CreatedAt = cat.CreatedAt
    };
}

public class ProduitType
{
    public string Id { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    public int Stock { get; set; }
    public string CategorieId { get; set; } = string.Empty;
    public string? Image { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Relations
    [GraphQLIgnore]
    public CategorieType? Categorie { get; set; }
    [GraphQLIgnore]
    public List<AvisType> Avis { get; set; } = new();
    [GraphQLIgnore]
    public List<CommandeType> Commandes { get; set; } = new();

    public static ProduitType FromModel(Produit prod) => new()
    {
        Id = prod.Id,
        Nom = prod.Nom,
        Description = prod.Description,
        Prix = prod.Prix,
        Stock = prod.Stock,
        CategorieId = prod.CategorieId,
        Image = prod.Image,
        CreatedAt = prod.CreatedAt,
        UpdatedAt = prod.UpdatedAt
    };
}

public class ProduitPageType
{
    public List<ProduitType> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public enum ProduitSortField
{
    Nom,
    Prix,
    Stock,
    CreatedAt
}

public enum ProduitSortDirection
{
    Asc,
    Desc
}

public class UtilisateurType
{
    public string Id { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public AdresseType? Adresse { get; set; }
    public DateTime CreatedAt { get; set; }

    // Relations
    [GraphQLIgnore]
    public List<CommandeType> Commandes { get; set; } = new();
    [GraphQLIgnore]
    public List<AvisType> Avis { get; set; } = new();

    public static UtilisateurType FromModel(Utilisateur user) => new()
    {
        Id = user.Id,
        Nom = user.Nom,
        Prenom = user.Prenom,
        Email = user.Email,
        Telephone = user.Telephone,
        Adresse = user.Adresse != null ? AdresseType.FromModel(user.Adresse) : null,
        CreatedAt = user.CreatedAt
    };
}

public class AdresseType
{
    public string Rue { get; set; } = string.Empty;
    public string CodePostal { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string Pays { get; set; } = string.Empty;

    public static AdresseType FromModel(Adresse addr) => new()
    {
        Rue = addr.Rue,
        CodePostal = addr.CodePostal,
        Ville = addr.Ville,
        Pays = addr.Pays
    };
}

public class AvisType
{
    public string Id { get; set; } = string.Empty;
    public string ProduitId { get; set; } = string.Empty;
    public string UtilisateurId { get; set; } = string.Empty;
    public int Note { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Relation
    [GraphQLIgnore]
    public ProduitType? Produit { get; set; }
    [GraphQLIgnore]
    public UtilisateurType? Utilisateur { get; set; }

    public static AvisType FromModel(Avis avis) => new()
    {
        Id = avis.Id,
        ProduitId = avis.ProduitId,
        UtilisateurId = avis.UtilisateurId,
        Note = avis.Note,
        Titre = avis.Titre,
        Comment = avis.Comment,
        CreatedAt = avis.CreatedAt
    };
}

public class CommandeType
{
    public string Id { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string UtilisateurId { get; set; } = string.Empty;
    public List<LigneCommandeType> Lignes { get; set; } = new();
    public string Statut { get; set; } = string.Empty;
    public decimal MontantTotal { get; set; }
    public DateTime DateCommande { get; set; }
    public DateTime? DateLivraison { get; set; }

    // Relation
    [GraphQLIgnore]
    public UtilisateurType? Utilisateur { get; set; }

    public static CommandeType FromModel(Commande cmd) => new()
    {
        Id = cmd.Id,
        Numero = cmd.Numero,
        UtilisateurId = cmd.UtilisateurId,
        Lignes = cmd.Lignes.Select(LigneCommandeType.FromModel).ToList(),
        Statut = cmd.Statut,
        MontantTotal = cmd.MontantTotal,
        DateCommande = cmd.DateCommande,
        DateLivraison = cmd.DateLivraison
    };
}

public class LigneCommandeType
{
    public string ProduitId { get; set; } = string.Empty;
    public int Quantite { get; set; }
    public decimal PrixUnitaire { get; set; }
    public decimal SousTotal { get; set; }

    // Relation
    [GraphQLIgnore]
    public ProduitType? Produit { get; set; }

    public static LigneCommandeType FromModel(LigneCommande ligne) => new()
    {
        ProduitId = ligne.ProduitId,
        Quantite = ligne.Quantite,
        PrixUnitaire = ligne.PrixUnitaire,
        SousTotal = ligne.SousTotal
    };
}
