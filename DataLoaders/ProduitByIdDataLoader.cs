using GraphQLApi.Data;
using GraphQLApi.Types;
using GreenDonut;
using MongoDB.Driver;

namespace GraphQLApi.DataLoaders;

public sealed class ProduitByIdDataLoader : BatchDataLoader<string, ProduitType?>
{
    private readonly MongoDbContext _context;

    public ProduitByIdDataLoader(
        IBatchScheduler batchScheduler,
        MongoDbContext context,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options ?? new DataLoaderOptions())
    {
        _context = context;
    }

    protected override async Task<IReadOnlyDictionary<string, ProduitType?>> LoadBatchAsync(
        IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        var produits = await _context.Produits
            .Find(p => keys.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var produitsById = produits
            .Select(ProduitType.FromModel)
            .ToDictionary(p => p.Id, p => (ProduitType?)p);

        return keys.Distinct().ToDictionary(
            key => key,
            key => produitsById.TryGetValue(key, out var produit) ? produit : null);
    }
}

