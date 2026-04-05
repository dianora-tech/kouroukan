using FluentAssertions;
using GnDapper.Connection;
using Kouroukan.Api.Gateway.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Services;

public sealed class AdminServiceTests
{
    private readonly Mock<IDbConnectionFactory> _connectionFactoryMock;
    private readonly Mock<ILogger<AdminService>> _loggerMock;
    private readonly AdminService _sut;

    public AdminServiceTests()
    {
        _connectionFactoryMock = new Mock<IDbConnectionFactory>();
        _loggerMock = new Mock<ILogger<AdminService>>();
        _sut = new AdminService(_connectionFactoryMock.Object, _loggerMock.Object, new NimbaSmsService(new HttpClient(), new Mock<ILogger<NimbaSmsService>>().Object));
    }

    // ─── Construction ───

    [Fact]
    public void Constructeur_DoitCreerInstance_QuandDependancesValides()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    [Fact]
    public void Constructeur_DoitImplementerIAdminService()
    {
        // Assert
        _sut.Should().BeAssignableTo<IAdminService>();
    }

    // ─── Interface Forfaits ───

    [Fact]
    public void Interface_DoitExposerGetForfaitsAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetForfaitsAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerGetForfaitByIdAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetForfaitByIdAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerCreateForfaitAsync()
    {
        var method = typeof(IAdminService).GetMethod("CreateForfaitAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerUpdateForfaitAsync()
    {
        var method = typeof(IAdminService).GetMethod("UpdateForfaitAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerDeleteForfaitAsync()
    {
        var method = typeof(IAdminService).GetMethod("DeleteForfaitAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerUpdateForfaitTarifAsync()
    {
        var method = typeof(IAdminService).GetMethod("UpdateForfaitTarifAsync");
        method.Should().NotBeNull();
    }

    // ─── Interface Abonnements ───

    [Fact]
    public void Interface_DoitExposerGetAbonnementsAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetAbonnementsAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerCreateAbonnementAsync()
    {
        var method = typeof(IAdminService).GetMethod("CreateAbonnementAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerUpdateAbonnementAsync()
    {
        var method = typeof(IAdminService).GetMethod("UpdateAbonnementAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerDeleteAbonnementAsync()
    {
        var method = typeof(IAdminService).GetMethod("DeleteAbonnementAsync");
        method.Should().NotBeNull();
    }

    // ─── Interface Gestes Commerciaux ───

    [Fact]
    public void Interface_DoitExposerGetGestesCommerciauxAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetGestesCommerciauxAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerCreateGesteCommercialAsync()
    {
        var method = typeof(IAdminService).GetMethod("CreateGesteCommercialAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerUpdateGesteCommercialAsync()
    {
        var method = typeof(IAdminService).GetMethod("UpdateGesteCommercialAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerDeleteGesteCommercialAsync()
    {
        var method = typeof(IAdminService).GetMethod("DeleteGesteCommercialAsync");
        method.Should().NotBeNull();
    }

    // ─── Interface Email Config ───

    [Fact]
    public void Interface_DoitExposerGetEmailConfigAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetEmailConfigAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerUpdateEmailConfigAsync()
    {
        var method = typeof(IAdminService).GetMethod("UpdateEmailConfigAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerSendTestEmailAsync()
    {
        var method = typeof(IAdminService).GetMethod("SendTestEmailAsync");
        method.Should().NotBeNull();
    }

    // ─── Interface SMS Config ───

    [Fact]
    public void Interface_DoitExposerGetSmsConfigAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetSmsConfigAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerUpdateSmsConfigAsync()
    {
        var method = typeof(IAdminService).GetMethod("UpdateSmsConfigAsync");
        method.Should().NotBeNull();
    }

    // ─── Interface Comptes Mobile ───

    [Fact]
    public void Interface_DoitExposerGetComptesMobileAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetComptesMobileAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerCreateCompteMobileAsync()
    {
        var method = typeof(IAdminService).GetMethod("CreateCompteMobileAsync");
        method.Should().NotBeNull();
    }

    // ─── Interface Contenu IA ───

    [Fact]
    public void Interface_DoitExposerGetContenusIaAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetContenusIaAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerGetContenuIaByIdAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetContenuIaByIdAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerCreateContenuIaAsync()
    {
        var method = typeof(IAdminService).GetMethod("CreateContenuIaAsync");
        method.Should().NotBeNull();
    }

    // ─── Interface Etablissements ───

    [Fact]
    public void Interface_DoitExposerGetEtablissementsAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetEtablissementsAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerGetEtablissementByIdAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetEtablissementByIdAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerUpdateEtablissementAsync()
    {
        var method = typeof(IAdminService).GetMethod("UpdateEtablissementAsync");
        method.Should().NotBeNull();
    }

    [Fact]
    public void Interface_DoitExposerDeleteEtablissementAsync()
    {
        var method = typeof(IAdminService).GetMethod("DeleteEtablissementAsync");
        method.Should().NotBeNull();
    }

    // ─── Interface Statistiques ───

    [Fact]
    public void Interface_DoitExposerGetForfaitStatsAsync()
    {
        var method = typeof(IAdminService).GetMethod("GetForfaitStatsAsync");
        method.Should().NotBeNull();
    }
}
