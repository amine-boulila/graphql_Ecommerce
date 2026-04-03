using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GraphQLApi.Models;

public class Utilisateur
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("nom")]
    public string Nom { get; set; } = string.Empty;

    [BsonElement("prenom")]
    public string Prenom { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("telephone")]
    public string? Telephone { get; set; }

    [BsonElement("adresse")]
    public Adresse? Adresse { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Adresse
{
    [BsonElement("rue")]
    public string Rue { get; set; } = string.Empty;

    [BsonElement("codePostal")]
    public string CodePostal { get; set; } = string.Empty;

    [BsonElement("ville")]
    public string Ville { get; set; } = string.Empty;

    [BsonElement("pays")]
    public string Pays { get; set; } = string.Empty;
}
