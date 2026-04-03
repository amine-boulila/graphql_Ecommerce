using GraphQLApi.Data;
using GraphQLApi.Types;
using GreenDonut;
using MongoDB.Driver;

namespace GraphQLApi.DataLoaders;

public sealed class CommandesByUtilisateurIdDataLoader : GroupedDataLoader<string, CommandeType>
{
    private readonly MongoDbContext _context;

    public CommandesByUtilisateurIdDataLoader(
        IBatchScheduler batchScheduler,
        MongoDbContext context,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _context = context;
    }

    protected override async Task<ILookup<string, CommandeType>> LoadGroupedBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var commandes = await _context.Commandes
            .Find(c => keys.Contains(c.UtilisateurId))
            .ToListAsync(cancellationToken);

        return commandes
            .Select(CommandeType.FromModel)
            .ToLookup(c => c.UtilisateurId);
    }
}

