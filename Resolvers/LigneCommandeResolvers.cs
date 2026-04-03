using GraphQLApi.DataLoaders;
using GraphQLApi.Types;
using HotChocolate;
using HotChocolate.Types;

namespace GraphQLApi.Resolvers;

[ExtendObjectType<LigneCommandeType>]
public sealed class LigneCommandeResolvers
{
    public Task<ProduitType?> GetProduit(
        [Parent] LigneCommandeType ligneCommande,
        ProduitByIdDataLoader produitById,
        CancellationToken cancellationToken) =>
        produitById.LoadAsync(ligneCommande.ProduitId, cancellationToken);
}
