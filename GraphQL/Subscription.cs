using GraphQLApi.Types;
using HotChocolate;
using HotChocolate.Subscriptions;

namespace GraphQLApi.GraphQL;

// Centralizes the topic names used to publish and subscribe to order events.
public static class CommandeSubscriptionTopics
{
    public const string CommandeCreated = "COMMANDE_CREATED";
    public const string CommandeUpdated = "COMMANDE_UPDATED";
    public const string CommandeDeleted = "COMMANDE_DELETED";
}

// Exposes GraphQL subscriptions so clients can react to live order changes.
public class Subscription
{
    // Triggered whenever a new order is created.
    [Subscribe]
    [Topic(CommandeSubscriptionTopics.CommandeCreated)]
    public CommandeType OnCommandeCreated([EventMessage] CommandeType commande) => commande;

    // Triggered whenever an existing order is updated.
    [Subscribe]
    [Topic(CommandeSubscriptionTopics.CommandeUpdated)]
    public CommandeType OnCommandeUpdated([EventMessage] CommandeType commande) => commande;

    // Triggered whenever an order is deleted.
    [Subscribe]
    [Topic(CommandeSubscriptionTopics.CommandeDeleted)]
    public CommandeType OnCommandeDeleted([EventMessage] CommandeType commande) => commande;
}
