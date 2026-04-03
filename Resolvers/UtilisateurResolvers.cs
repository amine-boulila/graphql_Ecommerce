using GraphQLApi.DataLoaders;
using GraphQLApi.Types;
using HotChocolate;
using HotChocolate.Types;

namespace GraphQLApi.Resolvers;

[ExtendObjectType<UtilisateurType>]
public sealed class UtilisateurResolvers
{
    public async Task<List<CommandeType>> GetCommandes(
        [Parent] UtilisateurType utilisateur,
        CommandesByUtilisateurIdDataLoader commandesByUtilisateurId,
        CancellationToken cancellationToken)
    {
        var commandes = await commandesByUtilisateurId.LoadAsync(utilisateur.Id, cancellationToken);
        return (commandes ?? Array.Empty<CommandeType>()).ToList();
    }

    public async Task<List<AvisType>> GetAvis(
        [Parent] UtilisateurType utilisateur,
        AvisByUtilisateurIdDataLoader avisByUtilisateurId,
        CancellationToken cancellationToken)
    {
        var avis = await avisByUtilisateurId.LoadAsync(utilisateur.Id, cancellationToken);
        return (avis ?? Array.Empty<AvisType>()).ToList();
    }
}

