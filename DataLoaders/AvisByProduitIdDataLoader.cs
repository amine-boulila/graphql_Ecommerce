using GraphQLApi.Data;
using GraphQLApi.Types;
using GreenDonut;
using MongoDB.Driver;

namespace GraphQLApi.DataLoaders;

public sealed class AvisByProduitIdDataLoader : GroupedDataLoader<string, AvisType>
{
    private readonly MongoDbContext _context;

    public AvisByProduitIdDataLoader(
        IBatchScheduler batchScheduler,
        MongoDbContext context,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _context = context;
    }

    protected override async Task<ILookup<string, AvisType>> LoadGroupedBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var avis = await _context.Avis
            .Find(a => keys.Contains(a.ProduitId))
            .ToListAsync(cancellationToken);

        return avis
            .Select(AvisType.FromModel)
            .ToLookup(a => a.ProduitId);
    }
}

