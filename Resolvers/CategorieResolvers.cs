using GraphQLApi.DataLoaders;
using GraphQLApi.Types;
using HotChocolate;
using HotChocolate.Types;

namespace GraphQLApi.Resolvers;

[ExtendObjectType<CategorieType>]
public sealed class CategorieResolvers
{
    // Resolves category products on demand instead of preloading the whole graph.
    public async Task<List<ProduitType>> GetProduits(
        [Parent] CategorieType categorie,
        ProduitsByCategorieIdDataLoader produitsByCategorieId,
        CancellationToken cancellationToken)
    {
        var produits = await produitsByCategorieId.LoadAsync(categorie.Id, cancellationToken);
        return (produits ?? Array.Empty<ProduitType>()).ToList();
    }
}

