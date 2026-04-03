using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GraphQLApi.Models;

public class Commande
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("numero")]
    public string Numero { get; set; } = string.Empty;

    [BsonElement("utilisateurId")]
    public string UtilisateurId { get; set; } = string.Empty;

    [BsonElement("lignes")]
    public List<LigneCommande> Lignes { get; set; } = new();

    [BsonElement("statut")]
    public string Statut { get; set; } = "En attente"; // En attente, Confirmée, Expédiée, Livrée

    [BsonElement("montantTotal")]
    public decimal MontantTotal { get; set; }

    [BsonElement("dateCommande")]
    public DateTime DateCommande { get; set; } = DateTime.UtcNow;

    [BsonElement("dateLivraison")]
    public DateTime? DateLivraison { get; set; }
}

public class LigneCommande
{
    [BsonElement("produitId")]
    public string ProduitId { get; set; } = string.Empty;

    [BsonElement("quantite")]
    public int Quantite { get; set; }

    [BsonElement("prixUnitaire")]
    public decimal PrixUnitaire { get; set; }

    [BsonElement("sousTotal")]
    public decimal SousTotal => Quantite * PrixUnitaire;
}
