using System.ComponentModel.DataAnnotations;

namespace Kouroukan.Api.Gateway.Models;

/// <summary>
/// Requete de rafraichissement de token.
/// </summary>
public class RefreshRequest
{
    /// <summary>Refresh token a echanger.</summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
