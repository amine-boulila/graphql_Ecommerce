# API Documentation

## Overview

This project exposes a GraphQL API for a small e-commerce domain built with:

- ASP.NET Core
- Hot Chocolate
- MongoDB

Main entities:

- `Categorie`
- `Produit`
- `Utilisateur`
- `Commande`
- `Avis`

The API supports:

- queries
- mutations
- subscriptions
- product filtering
- product sorting
- product pagination
- client credential protection

## GraphQL Endpoint

Default endpoints:

- `http://localhost:5145/graphql`
- `https://localhost:7050/graphql`

## Security

The API is protected by two HTTP headers:

- `X-Client-Id`
- `X-Client-Secret`

Default values:

- `graphql-client`
- `graphql-secret`

These values can be configured in:

- `.env`
- `appsettings.json`

## Project Structure

- `GraphQL/Query.cs`: root query resolvers
- `GraphQL/Mutation.cs`: root mutation resolvers
- `GraphQL/Subscription.cs`: subscription resolvers
- `Resolvers/`: nested field resolvers
- `DataLoaders/`: batched data loading to avoid the N+1 problem
- `Types/Types.cs`: GraphQL output types
- `Models/`: MongoDB document models
- `Data/MongoDbContext.cs`: MongoDB collections access

## Main GraphQL Types

### Categorie

Fields:

- `id`
- `nom`
- `description`
- `createdAt`
- `produits`

### Produit

Fields:

- `id`
- `nom`
- `description`
- `prix`
- `stock`
- `categorieId`
- `image`
- `createdAt`
- `updatedAt`
- `categorie`
- `avis`
- `commandes`

### Utilisateur

Fields:

- `id`
- `nom`
- `prenom`
- `email`
- `telephone`
- `adresse`
- `createdAt`
- `commandes`
- `avis`

### Adresse

Fields:

- `rue`
- `codePostal`
- `ville`
- `pays`

### Avis

Fields:

- `id`
- `produitId`
- `utilisateurId`
- `note`
- `titre`
- `comment`
- `createdAt`
- `produit`
- `utilisateur`

### Commande

Fields:

- `id`
- `numero`
- `utilisateurId`
- `lignes`
- `statut`
- `montantTotal`
- `dateCommande`
- `dateLivraison`
- `utilisateur`

### LigneCommande

Fields:

- `produitId`
- `quantite`
- `prixUnitaire`
- `sousTotal`
- `produit`

## Queries

### Categories

- `categories`
- `categorie(id: String!)`

Example:

```graphql
query {
  categories {
    id
    nom
    description
    createdAt
    produits {
      id
      nom
      prix
    }
  }
}
```

### Produits

- `produits`
- `produit(id: String!)`
- `produitsByCategorie(categorieId: String!)`
- `produitsPage(...)`

`produitsPage` supports:

- `search`
- `categorieId`
- `minPrix`
- `maxPrix`
- `sortBy`
- `sortDirection`
- `skip`
- `take`

Example:

```graphql
query {
  produitsPage(
    search: "Laptop"
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

### Utilisateurs

- `utilisateurs`
- `utilisateur(id: String!)`

Example:

```graphql
query {
  utilisateurs {
    id
    nom
    prenom
    email
    telephone
    adresse {
      rue
      codePostal
      ville
      pays
    }
    commandes {
      id
      numero
      statut
    }
    avis {
      id
      note
      titre
    }
  }
}
```

### Commandes

- `commandes`
- `commande(id: String!)`
- `commandesByUtilisateur(utilisateurId: String!)`

Example:

```graphql
query {
  commandes {
    id
    numero
    statut
    montantTotal
    dateCommande
    dateLivraison
    utilisateur {
      id
      nom
      prenom
    }
    lignes {
      produitId
      quantite
      prixUnitaire
      sousTotal
      produit {
        id
        nom
        prix
      }
    }
  }
}
```

### Avis

- `avis`
- `avisByProduit(produitId: String!)`

Example:

```graphql
query {
  avis {
    id
    note
    titre
    comment
    createdAt
    produit {
      nom
    }
    utilisateur {
      nom
      prenom
    }
  }
}
```

## Mutations

### Categories

- `createCategorie(nom, description)`
- `updateCategorie(id, nom, description)`
- `deleteCategorie(id)`

Example:

```graphql
mutation {
  createCategorie(
    nom: "Maison"
    description: "Produits pour la maison"
  ) {
    id
    nom
    description
    createdAt
  }
}
```

### Produits

- `createProduit(nom, description, prix, stock, categorieId, image)`
- `updateProduit(id, nom, description, prix, stock)`
- `deleteProduit(id)`

Example:

```graphql
mutation {
  createProduit(
    nom: "Chaise de bureau"
    description: "Chaise confortable pour le travail"
    prix: 199.99
    stock: 12
    categorieId: "REMPLACE_PAR_CATEGORIE_ID"
    image: "https://via.placeholder.com/300"
  ) {
    id
    nom
    prix
    stock
    categorie {
      id
      nom
    }
  }
}
```

### Utilisateurs

- `createUtilisateur(nom, prenom, email, telephone)`
- `updateUtilisateur(id, nom, prenom)`

Example:

```graphql
mutation {
  createUtilisateur(
    nom: "Demo"
    prenom: "Etudiant"
    email: "demo.validation@fac.test"
    telephone: "0600000000"
  ) {
    id
    nom
    prenom
    email
    telephone
    createdAt
  }
}
```

### Commandes

- `createCommande(utilisateurId, lignes)`
- `updateCommandeStatut(id, statut)`
- `deleteCommande(id)`

Example:

```graphql
mutation {
  createCommande(
    utilisateurId: "REMPLACE_PAR_UTILISATEUR_ID"
    lignes: [
      {
        produitId: "REMPLACE_PAR_PRODUIT_ID"
        quantite: 2
        prixUnitaire: 1299.99
      }
    ]
  ) {
    id
    numero
    statut
    montantTotal
    utilisateur {
      nom
      prenom
    }
    lignes {
      quantite
      prixUnitaire
      sousTotal
      produit {
        nom
      }
    }
  }
}
```

### Avis

- `createAvis(produitId, utilisateurId, note, titre, comment)`
- `deleteAvis(id)`

Example:

```graphql
mutation {
  createAvis(
    produitId: "REMPLACE_PAR_PRODUIT_ID"
    utilisateurId: "REMPLACE_PAR_UTILISATEUR_ID"
    note: 5
    titre: "Excellent"
    comment: "Tres bon produit pour la demonstration"
  ) {
    id
    note
    titre
    comment
    produit {
      nom
    }
    utilisateur {
      nom
      prenom
    }
  }
}
```

## Subscriptions

Available subscriptions:

- `onCommandeCreated`
- `onCommandeUpdated`
- `onCommandeDeleted`

Examples:

```graphql
subscription {
  onCommandeCreated {
    id
    numero
    statut
    montantTotal
  }
}
```

```graphql
subscription {
  onCommandeUpdated {
    id
    numero
    statut
    dateLivraison
  }
}
```

```graphql
subscription {
  onCommandeDeleted {
    id
    numero
    statut
  }
}
```

## Product Pagination, Filtering and Sorting

The `produitsPage` query allows clients to retrieve paginated products efficiently.

Arguments:

- `search`: search by name or description
- `categorieId`: filter by category id
- `minPrix`: minimum price
- `maxPrix`: maximum price
- `sortBy`: `NOM`, `PRIX`, `STOCK`, `CREATED_AT`
- `sortDirection`: `ASC`, `DESC`
- `skip`: number of items to skip
- `take`: number of items to return

Returned pagination fields:

- `items`
- `totalCount`
- `skip`
- `take`
- `hasNextPage`
- `hasPreviousPage`

## Resolver-Based Architecture

This API uses a resolver-based approach:

- root queries load only the requested main documents
- nested fields are resolved lazily
- DataLoaders batch repeated lookups

Examples:

- `getProduit` loads one product
- `Produit.categorie` is resolved later by a nested resolver
- `Produit.avis` is resolved only if requested
- category, product, user and order lookups are batched through DataLoaders

This improves scalability because the API no longer loads all collections into memory for each request.

## Testing

You can test the API with:

- the `/graphql` endpoint
- the `GraphQLApi.http` file
- the `requette.graphql` file

## Notes

- Keep valid MongoDB connection settings in `.env` or `appsettings.json`
- Keep valid client credentials in request headers
- Subscriptions require WebSocket support
