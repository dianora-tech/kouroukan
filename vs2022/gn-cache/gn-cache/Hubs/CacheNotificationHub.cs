using Microsoft.AspNetCore.SignalR;

namespace GnCache.Api.Hubs;

/// <summary>
/// Hub SignalR pour les notifications en temps reel de rechargement de cache.
/// Les clients peuvent se connecter pour recevoir des evenements lorsqu'un cache est recharge.
/// </summary>
/// <remarks>
/// Methodes server-to-client (invoquees via IHubContext):
/// <list type="bullet">
///   <item><description>"CacheReloaded" : { cacheKey, source, itemCount, timestamp }</description></item>
///   <item><description>"CacheError" : { cacheKey, error, timestamp }</description></item>
///   <item><description>"AllCachesReloaded" : { timestamp }</description></item>
/// </list>
/// </remarks>
public sealed class CacheNotificationHub : Hub
{
    /// <summary>
    /// Methode appelee lorsqu'un client se connecte au hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}
