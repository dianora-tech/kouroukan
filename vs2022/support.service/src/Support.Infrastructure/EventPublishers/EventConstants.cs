namespace Support.Infrastructure.EventPublishers;

public static class EventConstants
{
    public const string Exchange = "kouroukan.events";
    public const string EntityCreatedRoutingKeyPrefix = "entity.created";
    public const string EntityUpdatedRoutingKeyPrefix = "entity.updated";
    public const string EntityDeletedRoutingKeyPrefix = "entity.deleted";
}
