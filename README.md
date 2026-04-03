# GraphQL API - Phase 1 et Phase 2

## Theme choisi

Le projet expose une API GraphQL autour d'un mini domaine e-commerce.

## Partie 1 validee

La partie 1 est bien couverte dans le projet :

- Schema GraphQL avec plusieurs types relies entre eux
- Queries pour consulter les donnees
- Mutations pour creer, modifier et supprimer
- Subscriptions pour suivre les commandes en temps reel
- Base MongoDB alimentee avec des donnees de demonstration

### Entites principales

- `Categorie`
- `Produit`
- `Utilisateur`
- `Commande`
- `Avis`

### Relations principales

- Une categorie contient plusieurs produits
- Un produit appartient a une categorie
- Un utilisateur peut avoir plusieurs commandes
- Une commande contient plusieurs lignes de commande
- Un produit peut recevoir plusieurs avis
- Un utilisateur peut publier plusieurs avis

## Partie 2 implemente

### 1. Securisation de l'API

L'API est protegee par des client credentials envoyes dans les headers HTTP :

- `X-Client-Id`
- `X-Client-Secret`

Valeurs par defaut :

- `graphql-client`
- `graphql-secret`

Vous pouvez les changer dans :

- `.env`
- `appsettings.json`

### 2. Fonctionnalites supplementaires

Les fonctionnalites suivantes ont ete ajoutees sur la consultation des produits :

- Pagination
- Filtrage
- Tri

## Nouvelle query utile

```graphql
query {
  produitsPage(
    search: "Laptop"
    categorieId: null
    minPrix: 500
    maxPrix: 2000
    sortBy: PRIX
    sortDirection: DESC
    skip: 0
    take: 5
  ) {
    totalCount
    skip
    take
    hasNextPage
    hasPreviousPage
    items {
      id
      nom
      prix
      stock
      categorie {
        nom
      }
    }
  }
}
```

## Signification des parametres

- `search` : recherche sur le nom ou la description
- `categorieId` : filtre par categorie
- `minPrix` : prix minimum
- `maxPrix` : prix maximum
- `sortBy` : champ de tri (`NOM`, `PRIX`, `STOCK`, `CREATED_AT`)
- `sortDirection` : sens du tri (`ASC` ou `DESC`)
- `skip` : nombre d'elements a ignorer
- `take` : nombre d'elements a retourner

## Lancer le projet

```bash
dotnet run
```

Endpoint GraphQL :

- `http://localhost:5145/graphql`
- `https://localhost:7050/graphql`

## Fichier de test

Le fichier `GraphQLApi.http` contient des exemples de requetes prêtes a executer.
