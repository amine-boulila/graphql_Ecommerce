using GraphQLApi.Data;
using GraphQLApi.Types;
using GreenDonut;
using MongoDB.Driver;

namespace GraphQLApi.DataLoaders;

public sealed class CategorieByIdDataLoader : BatchDataLoader<string, CategorieType?>
{
    private readonly MongoDbContext _context;

    public CategorieByIdDataLoader(
        IBatchScheduler batchScheduler,
        MongoDbContext context,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _context = context;
    }

    protected override async Task<IReadOnlyDictionary<string, CategorieType?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .Find(c => keys.Contains(c.Id))
            .ToListAsync(cancellationToken);

        var categoriesById = categories
            .Select(CategorieType.FromModel)
            .ToDictionary(c => c.Id, c => (CategorieType?)c);

        return keys.Distinct().ToDictionary(
            key => key,
            key => categoriesById.TryGetValue(key, out var categorie) ? categorie : null);
    }
}

