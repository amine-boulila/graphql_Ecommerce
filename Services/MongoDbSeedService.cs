using GraphQLApi.Data;
using GraphQLApi.Models;
using MongoDB.Driver;
using MongoDB.Bson;
namespace GraphQLApi.Services;

public class MongoDbSeedService
{
    public static async Task SeedDatabaseAsync(MongoDbContext context)
    {
        // Vérifier si données existent déjà
        var categoriesCount = await context.Categories.CountDocumentsAsync(_ => true);
        if (categoriesCount > 0) return;

        Console.WriteLine("🌱 Seed data en cours...");

        // Créer les catégories
        var categories = new List<Categorie>
        {
            new Categorie { Nom = "Électronique", Description = "Appareils électroniques et informatiques" },
            new Categorie { Nom = "Vêtements", Description = "Habits et accessoires de mode" },
            new Categorie { Nom = "Livres", Description = "Livres et littérature" },
            new Categorie { Nom = "Sports", Description = "Équipement sportif" }
        };
        await context.Categories.InsertManyAsync(categories);

        // Créer les produits
        var produits = new List<Produit>
        {
            new Produit
            {
                Nom = "Laptop Dell XPS",
                Description = "Ordinateur portable haute performance",
                Prix = 1299.99m,
                Stock = 15,
                CategorieId = categories[0].Id,
                Image = "https://via.placeholder.com/300"
            },
            new Produit
            {
                Nom = "iPhone 15",
                Description = "Dernier modèle d'Apple",
                Prix = 999.99m,
                Stock = 25,
                CategorieId = categories[0].Id,
                Image = "https://via.placeholder.com/300"
            },
            new Produit
            {
                Nom = "T-Shirt Coton",
                Description = "T-shirt confortable en coton 100%",
                Prix = 29.99m,
                Stock = 100,
                CategorieId = categories[1].Id,
                Image = "https://via.placeholder.com/300"
            },
            new Produit
            {
                Nom = "Jeans Classique",
                Description = "Jeans bleu classique",
                Prix = 59.99m,
                Stock = 50,
                CategorieId = categories[1].Id,
                Image = "https://via.placeholder.com/300"
            },
            new Produit
            {
                Nom = "Le Seigneur des Anneaux",
                Description = "La trilogie complète de Tolkien",
                Prix = 45.99m,
                Stock = 30,
                CategorieId = categories[2].Id,
                Image = "https://via.placeholder.com/300"
            },
            new Produit
            {
                Nom = "Ballon de Football",
                Description = "Ballon officiel de football",
                Prix = 79.99m,
                Stock = 20,
                CategorieId = categories[3].Id,
                Image = "https://via.placeholder.com/300"
            }
        };
        await context.Produits.InsertManyAsync(produits);

        // Créer les utilisateurs
        var utilisateurs = new List<Utilisateur>
        {
            new Utilisateur
            {
                Nom = "Dupont",
                Prenom = "Jean",
                Email = "jean.dupont@email.com",
                Telephone = "0612345678",
                Adresse = new Adresse
                {
                    Rue = "123 Rue de Paris",
                    CodePostal = "75001",
                    Ville = "Paris",
                    Pays = "France"
                }
            },
            new Utilisateur
            {
                Nom = "Martin",
                Prenom = "Marie",
                Email = "marie.martin@email.com",
                Telephone = "0687654321",
                Adresse = new Adresse
                {
                    Rue = "456 Avenue des Champs",
                    CodePostal = "75008",
                    Ville = "Paris",
                    Pays = "France"
                }
            },
            new Utilisateur
            {
                Nom = "Bernard",
                Prenom = "Pierre",
                Email = "pierre.bernard@email.com",
                Telephone = "0698765432",
                Adresse = new Adresse
                {
                    Rue = "789 Boulevard Saint-Germain",
                    CodePostal = "75005",
                    Ville = "Paris",
                    Pays = "France"
                }
            }
        };
        await context.Utilisateurs.InsertManyAsync(utilisateurs);

        // Créer les commandes
        var commandes = new List<Commande>
        {
            new Commande
            {
                Numero = $"CMD-{DateTime.UtcNow.Ticks}",
                UtilisateurId = utilisateurs[0].Id,
                Statut = "Confirmée",
                Lignes = new List<LigneCommande>
                {
                    new LigneCommande { ProduitId = produits[0].Id, Quantite = 1, PrixUnitaire = produits[0].Prix },
                    new LigneCommande { ProduitId = produits[2].Id, Quantite = 3, PrixUnitaire = produits[2].Prix }
                },
                MontantTotal = produits[0].Prix + (3 * produits[2].Prix),
                DateCommande = DateTime.UtcNow.AddDays(-5),
                DateLivraison = DateTime.UtcNow.AddDays(-1)
            },
            new Commande
            {
                Numero = $"CMD-{DateTime.UtcNow.Ticks + 1}",
                UtilisateurId = utilisateurs[1].Id,
                Statut = "En attente",
                Lignes = new List<LigneCommande>
                {
                    new LigneCommande { ProduitId = produits[1].Id, Quantite = 1, PrixUnitaire = produits[1].Prix }
                },
                MontantTotal = produits[1].Prix,
                DateCommande = DateTime.UtcNow.AddDays(-1)
            }
        };
        await context.Commandes.InsertManyAsync(commandes);

        // Créer les avis
        var avis = new List<Avis>
        {
            new Avis
            {
                ProduitId = produits[0].Id,
                UtilisateurId = utilisateurs[0].Id,
                Note = 5,
                Titre = "Excellent laptop!",
                Comment = "Très satisfait de cet achat, performance exceptionnelle"
            },
            new Avis
            {
                ProduitId = produits[0].Id,
                UtilisateurId = utilisateurs[1].Id,
                Note = 4,
                Titre = "Bon produit",
                Comment = "Très bon, mais un peu cher"
            },
            new Avis
            {
                ProduitId = produits[2].Id,
                UtilisateurId = utilisateurs[2].Id,
                Note = 5,
                Titre = "Confortable et bien fait",
                Comment = "Qualité parfait pour le prix"
            }
        };
        await context.Avis.InsertManyAsync(avis);

        Console.WriteLine("✅ Seed data complété!");
    }
}
