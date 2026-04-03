using GraphQLApi.Data;
using GraphQLApi.Types;
using GreenDonut;
using MongoDB.Driver;

namespace GraphQLApi.DataLoaders;

public sealed class UtilisateurByIdDataLoader : BatchDataLoader<string, UtilisateurType?>
{
    private readonly MongoDbContext _context;

    public UtilisateurByIdDataLoader(
        IBatchScheduler batchScheduler,
        MongoDbContext context,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _context = context;
    }

    protected override async Task<IReadOnlyDictionary<string, UtilisateurType?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var utilisateurs = await _context.Utilisateurs
            .Find(u => keys.Contains(u.Id))
            .ToListAsync(cancellationToken);

        var utilisateursById = utilisateurs
            .Select(UtilisateurType.FromModel)
            .ToDictionary(u => u.Id, u => (UtilisateurType?)u);

        return keys.Distinct().ToDictionary(
            key => key,
            key => utilisateursById.TryGetValue(key, out var utilisateur) ? utilisateur : null);
    }
}

