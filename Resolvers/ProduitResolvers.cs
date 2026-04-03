using GraphQLApi.DataLoaders;
using GraphQLApi.Types;
using HotChocolate;
using HotChocolate.Types;

namespace GraphQLApi.Resolvers;

[ExtendObjectType<ProduitType>]
public sealed class ProduitResolvers
{
    // Resolves the owning category lazily and batches repeated category lookups.
    public Task<CategorieType?> GetCategorie(
        [Parent] ProduitType produit,
        CategorieByIdDataLoader categorieById,
        CancellationToken cancellationToken) =>
        categorieById.LoadAsync(produit.CategorieId, cancellationToken);

    // Loads reviews only when requested by the GraphQL selection set.
    public async Task<List<AvisType>> GetAvis(
        [Parent] ProduitType produit,
        AvisByProduitIdDataLoader avisByProduitId,
        CancellationToken cancellationToken)
    {
        var avis = await avisByProduitId.LoadAsync(produit.Id, cancellationToken);
        return (avis ?? Array.Empty<AvisType>()).ToList();
    }

    // Finds related orders via order lines and batches requests across products.
    public async Task<List<CommandeType>> GetCommandes(
        [Parent] ProduitType produit,
        CommandesByProduitIdDataLoader commandesByProduitId,
        CancellationToken cancellationToken)
    {
        var commandes = await commandesByProduitId.LoadAsync(produit.Id, cancellationToken);
        return (commandes ?? Array.Empty<CommandeType>()).ToList();
    }
}

