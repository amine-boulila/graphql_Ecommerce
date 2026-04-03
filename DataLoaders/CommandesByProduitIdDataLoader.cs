using GraphQLApi.Data;
using GraphQLApi.Types;
using GreenDonut;
using MongoDB.Driver;

namespace GraphQLApi.DataLoaders;

public sealed class CommandesByProduitIdDataLoader : GroupedDataLoader<string, CommandeType>
{
    private readonly MongoDbContext _context;

    public CommandesByProduitIdDataLoader(
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
        var filter = Builders<Models.Commande>.Filter.In("lignes.produitId", keys);
        var commandes = await _context.Commandes
            .Find(filter)
            .ToListAsync(cancellationToken);

        return commandes
            .SelectMany(
                commande =>
                {
                    var commandeType = CommandeType.FromModel(commande);
                    return commande.Lignes
                        .Where(l => keys.Contains(l.ProduitId))
                        .Select(ligne => new { ligne.ProduitId, Commande = commandeType });
                })
            .ToLookup(x => x.ProduitId, x => x.Commande);
    }
}

