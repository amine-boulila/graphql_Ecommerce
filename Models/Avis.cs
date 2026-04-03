using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GraphQLApi.Models;

public class Avis
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("produitId")]
    public string ProduitId { get; set; } = string.Empty;

    [BsonElement("utilisateurId")]
    public string UtilisateurId { get; set; } = string.Empty;

    [BsonElement("note")]
    public int Note { get; set; } // 1 à 5 étoiles

    [BsonElement("titre")]
    public string Titre { get; set; } = string.Empty;

    [BsonElement("comment")]
    public string Comment { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
