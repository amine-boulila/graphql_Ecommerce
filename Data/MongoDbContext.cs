using MongoDB.Driver;
using GraphQLApi.Models;

namespace GraphQLApi.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Categorie> Categories => _database.GetCollection<Categorie>("categories");
    public IMongoCollection<Produit> Produits => _database.GetCollection<Produit>("produits");
    public IMongoCollection<Utilisateur> Utilisateurs => _database.GetCollection<Utilisateur>("utilisateurs");
    public IMongoCollection<Commande> Commandes => _database.GetCollection<Commande>("commandes");
    public IMongoCollection<Avis> Avis => _database.GetCollection<Avis>("avis");
}
