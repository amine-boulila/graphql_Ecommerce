using GraphQLApi.DataLoaders;
using GraphQLApi.Types;
using HotChocolate;
using HotChocolate.Types;

namespace GraphQLApi.Resolvers;

[ExtendObjectType<CommandeType>]
public sealed class CommandeResolvers
{
    public Task<UtilisateurType?> GetUtilisateur(
        [Parent] CommandeType commande,
        UtilisateurByIdDataLoader utilisateurById,
        CancellationToken cancellationToken) =>
        utilisateurById.LoadAsync(commande.UtilisateurId, cancellationToken);
}
