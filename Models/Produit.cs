using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GraphQLApi.Models;

public class Produit
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("nom")]
    public string Nom { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("prix")]
    public decimal Prix { get; set; }

    [BsonElement("stock")]
    public int Stock { get; set; }

    [BsonElement("categorieId")]
    public string CategorieId { get; set; } = string.Empty;

    [BsonElement("image")]
    public string? Image { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
