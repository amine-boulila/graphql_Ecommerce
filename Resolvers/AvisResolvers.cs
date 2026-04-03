using GraphQLApi.DataLoaders;
using GraphQLApi.Types;
using HotChocolate;
using HotChocolate.Types;

namespace GraphQLApi.Resolvers;

[ExtendObjectType<AvisType>]
public sealed class AvisResolvers
{
    public Task<ProduitType?> GetProduit(
        [Parent] AvisType avis,
        ProduitByIdDataLoader produitById,
        CancellationToken cancellationToken) =>
        produitById.LoadAsync(avis.ProduitId, cancellationToken);

    public Task<UtilisateurType?> GetUtilisateur(
        [Parent] AvisType avis,
        UtilisateurByIdDataLoader utilisateurById,
        CancellationToken cancellationToken) =>
        utilisateurById.LoadAsync(avis.UtilisateurId, cancellationToken);
}
