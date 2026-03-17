namespace Personnel.Infrastructure.EventPublishers;

/// <summary>
/// Les evenements sont publies directement via IMessagePublisher (GnMessaging)
/// dans les services du domaine. Ce fichier sert de point d'extension
/// pour des publishers personnalises si necessaire.
/// </summary>
public static class EventConstants
{
    public const string Exchange = "kouroukan.events";
    public const string EntityCreatedRoutingKeyPrefix = "entity.created";
    public const string EntityUpdatedRoutingKeyPrefix = "entity.updated";
    public const string EntityDeletedRoutingKeyPrefix = "entity.deleted";
}
