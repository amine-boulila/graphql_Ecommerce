using GraphQLApi.Data;
using GraphQLApi.Types;
using GreenDonut;
using MongoDB.Driver;

namespace GraphQLApi.DataLoaders;

public sealed class ProduitsByCategorieIdDataLoader : GroupedDataLoader<string, ProduitType>
{
    private readonly MongoDbContext _context;

    public ProduitsByCategorieIdDataLoader(
        IBatchScheduler batchScheduler,
        MongoDbContext context,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _context = context;
    }

    protected override async Task<ILookup<string, ProduitType>> LoadGroupedBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var produits = await _context.Produits
            .Find(p => keys.Contains(p.CategorieId))
            .ToListAsync(cancellationToken);

        return produits
            .Select(ProduitType.FromModel)
            .ToLookup(p => p.CategorieId);
    }
}

