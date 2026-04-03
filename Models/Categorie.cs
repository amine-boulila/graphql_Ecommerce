using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GraphQLApi.Models;

// Represents a product category stored in MongoDB.
public class Categorie
{
    // MongoDB primary key stored as an ObjectId but exposed as a string in .NET.
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    // Category display name, for example "Electronique" or "Livres".
    [BsonElement("nom")]
    public string Nom { get; set; } = string.Empty;

    // Short text describing what this category contains.
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    // Creation timestamp used for tracing and sorting.
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
