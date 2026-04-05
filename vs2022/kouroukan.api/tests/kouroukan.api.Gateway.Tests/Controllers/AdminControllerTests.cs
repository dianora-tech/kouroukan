using Xunit;
using FluentAssertions;
using Kouroukan.Api.Gateway.Controllers;
using Kouroukan.Api.Gateway.Models;
using Kouroukan.Api.Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kouroukan.Api.Gateway.Tests.Controllers;

public sealed class AdminControllerTests
{
    private readonly Mock<IAdminService> _adminServiceMock;
    private readonly AdminController _sut;

    public AdminControllerTests()
    {
        _adminServiceMock = new Mock<IAdminService>();
        _sut = new AdminController(_adminServiceMock.Object);
    }

    // ─── Forfaits ───

    [Fact]
    public async Task GetForfaits_DoitRetournerListePaginee()
    {
        // Arrange
        var paged = new PagedResult<ForfaitDto>
        {
            Items = new List<ForfaitDto> { new() { Id = 1, Nom = "Essentiel" } },
            TotalCount = 1, Page = 1, PageSize = 20
        };
        _adminServiceMock
            .Setup(x => x.GetForfaitsAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        // Act
        var result = await _sut.GetForfaits(1, 20);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<PagedResult<ForfaitDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetForfait_DoitRetournerForfait_QuandExiste()
    {
        // Arrange
        var forfait = new ForfaitDto { Id = 1, Nom = "Premium" };
        _adminServiceMock
            .Setup(x => x.GetForfaitByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(forfait);

        // Act
        var result = await _sut.GetForfait(1, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ForfaitDto>>().Subject;
        response.Data!.Nom.Should().Be("Premium");
    }

    [Fact]
    public async Task GetForfait_DoitRetournerNotFound_QuandInexistant()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.GetForfaitByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ForfaitDto?)null);

        // Act
        var result = await _sut.GetForfait(999, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateForfait_DoitRetournerForfaitCree()
    {
        // Arrange
        var request = new CreateForfaitRequest { Code = "PREM", Nom = "Premium" };
        var created = new ForfaitDto { Id = 5, Code = "PREM", Nom = "Premium" };
        _adminServiceMock
            .Setup(x => x.CreateForfaitAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result = await _sut.CreateForfait(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ForfaitDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Id.Should().Be(5);
    }

    [Fact]
    public async Task UpdateForfait_DoitRetournerOk()
    {
        // Arrange
        var request = new UpdateForfaitRequest { Nom = "Premium V2" };
        _adminServiceMock
            .Setup(x => x.UpdateForfaitAsync(1, request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateForfait(1, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteForfait_DoitRetournerOk()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.DeleteForfaitAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteForfait(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _adminServiceMock.Verify(x => x.DeleteForfaitAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateForfaitTarif_DoitRetournerOk()
    {
        // Arrange
        var request = new UpdateForfaitTarifRequest { PrixMensuel = 50000, PrixVacances = 25000, DateEffet = DateTime.UtcNow };
        _adminServiceMock
            .Setup(x => x.UpdateForfaitTarifAsync(1, request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateForfaitTarif(1, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // ─── Abonnements ───

    [Fact]
    public async Task GetAbonnements_DoitRetournerListePaginee()
    {
        // Arrange
        var paged = new PagedResult<AbonnementDto>
        {
            Items = new List<AbonnementDto> { new() { Id = 1 } },
            TotalCount = 1, Page = 1, PageSize = 20
        };
        _adminServiceMock
            .Setup(x => x.GetAbonnementsAsync(1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        // Act
        var result = await _sut.GetAbonnements();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<PagedResult<AbonnementDto>>>().Subject;
        response.Success.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAbonnement_DoitRetournerAbonnementCree()
    {
        // Arrange
        var request = new CreateAbonnementRequest { ForfaitId = 1, CompanyId = 10 };
        var created = new AbonnementDto { Id = 1, ForfaitId = 1, CompanyId = 10 };
        _adminServiceMock
            .Setup(x => x.CreateAbonnementAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result = await _sut.CreateAbonnement(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<AbonnementDto>>().Subject;
        response.Data!.Id.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAbonnement_DoitRetournerOk()
    {
        // Arrange
        var request = new UpdateAbonnementRequest { EstActif = false };
        _adminServiceMock
            .Setup(x => x.UpdateAbonnementAsync(1, request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateAbonnement(1, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteAbonnement_DoitRetournerOk()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.DeleteAbonnementAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAbonnement(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // ─── Gestes Commerciaux ───

    [Fact]
    public async Task GetGestesCommerciaux_DoitRetournerListe()
    {
        // Arrange
        var items = new List<GesteCommercialDto> { new() { Id = 1, Nom = "Promo rentree" } };
        _adminServiceMock
            .Setup(x => x.GetGestesCommerciauxAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.GetGestesCommerciaux();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<GesteCommercialDto>>>().Subject;
        response.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateGesteCommercial_DoitRetournerGesteCree()
    {
        // Arrange
        var request = new CreateGesteCommercialRequest { Nom = "Promo" };
        var created = new GesteCommercialDto { Id = 1, Nom = "Promo" };
        _adminServiceMock
            .Setup(x => x.CreateGesteCommercialAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result = await _sut.CreateGesteCommercial(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<GesteCommercialDto>>().Subject;
        response.Data!.Nom.Should().Be("Promo");
    }

    [Fact]
    public async Task UpdateGesteCommercial_DoitRetournerOk()
    {
        // Arrange
        var request = new UpdateGesteCommercialRequest { Nom = "Promo V2" };
        _adminServiceMock
            .Setup(x => x.UpdateGesteCommercialAsync(1, request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateGesteCommercial(1, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteGesteCommercial_DoitRetournerOk()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.DeleteGesteCommercialAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteGesteCommercial(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // ─── Email Config ───

    [Fact]
    public async Task GetEmailConfig_DoitRetournerConfig_QuandExiste()
    {
        // Arrange
        var config = new EmailConfigDto { Id = 1, Host = "smtp.test.com" };
        _adminServiceMock
            .Setup(x => x.GetEmailConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Act
        var result = await _sut.GetEmailConfig(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<EmailConfigDto>>().Subject;
        response.Data!.Host.Should().Be("smtp.test.com");
    }

    [Fact]
    public async Task GetEmailConfig_DoitRetournerNotFound_QuandInexistante()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.GetEmailConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmailConfigDto?)null);

        // Act
        var result = await _sut.GetEmailConfig(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateEmailConfig_DoitRetournerOk()
    {
        // Arrange
        var request = new UpdateEmailConfigRequest { Host = "smtp.new.com" };
        _adminServiceMock
            .Setup(x => x.UpdateEmailConfigAsync(request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateEmailConfig(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task SendTestEmail_DoitRetournerOk_QuandSucces()
    {
        // Arrange
        var request = new TestEmailRequest { Email = "test@test.com" };
        _adminServiceMock
            .Setup(x => x.SendTestEmailAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.SendTestEmail(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task SendTestEmail_DoitRetournerBadRequest_QuandEchec()
    {
        // Arrange
        var request = new TestEmailRequest { Email = "test@test.com" };
        _adminServiceMock
            .Setup(x => x.SendTestEmailAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.SendTestEmail(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─── SMS Config ───

    [Fact]
    public async Task GetSmsConfig_DoitRetournerConfig_QuandExiste()
    {
        // Arrange
        var config = new SmsConfigDto { Id = 1, SenderName = "Kouroukan" };
        _adminServiceMock
            .Setup(x => x.GetSmsConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Act
        var result = await _sut.GetSmsConfig(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<SmsConfigDto>>().Subject;
        response.Data!.SenderName.Should().Be("Kouroukan");
    }

    [Fact]
    public async Task GetSmsConfig_DoitRetournerNotFound_QuandInexistante()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.GetSmsConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((SmsConfigDto?)null);

        // Act
        var result = await _sut.GetSmsConfig(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateSmsConfig_DoitRetournerOk()
    {
        // Arrange
        var request = new UpdateSmsConfigRequest { ServiceId = "svc", SecretToken = "key", SenderName = "Test" };
        _adminServiceMock
            .Setup(x => x.UpdateSmsConfigAsync(request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateSmsConfig(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // ─── SMS Test / Sync / Historique ───

    [Fact]
    public async Task SendTestSms_DoitRetournerOk_QuandSucces()
    {
        // Arrange
        var request = new TestSmsRequest { To = "224621000000", Message = "Test" };
        _adminServiceMock
            .Setup(x => x.SendTestSmsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.SendTestSms(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task SendTestSms_DoitRetournerBadRequest_QuandEchec()
    {
        // Arrange
        var request = new TestSmsRequest { To = "224621000000", Message = "Test" };
        _adminServiceMock
            .Setup(x => x.SendTestSmsAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.SendTestSms(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task SyncSmsBalance_DoitRetournerConfigMiseAJour()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.SyncSmsBalanceAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var config = new SmsConfigDto { Id = 1, Solde = 5000, SmsRestants = 25 };
        _adminServiceMock
            .Setup(x => x.GetSmsConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(config);

        // Act
        var result = await _sut.SyncSmsBalance(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<SmsConfigDto>>().Subject;
        response.Data!.Solde.Should().Be(5000);
    }

    [Fact]
    public async Task GetSmsHistorique_DoitRetournerListePaginee()
    {
        // Arrange
        var paged = new PagedResult<SmsHistoriqueDto>
        {
            Items = new List<SmsHistoriqueDto> { new() { Id = 1, Destinataire = "224621000000" } },
            TotalCount = 1, Page = 1, PageSize = 20
        };
        _adminServiceMock
            .Setup(x => x.GetSmsHistoriqueAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        // Act
        var result = await _sut.GetSmsHistorique();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<PagedResult<SmsHistoriqueDto>>>().Subject;
        response.Data!.Items.Should().HaveCount(1);
    }

    // ─── Comptes Mobile Money ───

    [Fact]
    public async Task GetComptesMobile_DoitRetournerListe()
    {
        // Arrange
        var items = new List<CompteMobileDto> { new() { Id = 1, Operateur = "OrangeMoney" } };
        _adminServiceMock
            .Setup(x => x.GetComptesMobileAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.GetComptesMobile(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<CompteMobileDto>>>().Subject;
        response.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateCompteMobile_DoitRetournerCompteCree()
    {
        // Arrange
        var request = new CreateCompteMobileRequest { Operateur = "OrangeMoney", NumeroCompte = "621000000" };
        var created = new CompteMobileDto { Id = 1, Operateur = "OrangeMoney" };
        _adminServiceMock
            .Setup(x => x.CreateCompteMobileAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result = await _sut.CreateCompteMobile(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<CompteMobileDto>>().Subject;
        response.Data!.Operateur.Should().Be("OrangeMoney");
    }

    [Fact]
    public async Task UpdateCompteMobile_DoitRetournerOk()
    {
        // Arrange
        var request = new UpdateCompteMobileRequest { Operateur = "MTN" };
        _adminServiceMock
            .Setup(x => x.UpdateCompteMobileAsync(1, request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateCompteMobile(1, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteCompteMobile_DoitRetournerOk()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.DeleteCompteMobileAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteCompteMobile(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // ─── Contenu IA ───

    [Fact]
    public async Task GetContenusIa_DoitRetournerListe()
    {
        // Arrange
        var items = new List<ContenuIaDto> { new() { Id = 1, Titre = "Introduction" } };
        _adminServiceMock
            .Setup(x => x.GetContenusIaAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.GetContenusIa();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<ContenuIaDto>>>().Subject;
        response.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetContenuIa_DoitRetournerContenu_QuandExiste()
    {
        // Arrange
        var contenu = new ContenuIaDto { Id = 1, Titre = "Bienvenue" };
        _adminServiceMock
            .Setup(x => x.GetContenuIaByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contenu);

        // Act
        var result = await _sut.GetContenuIa(1, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ContenuIaDto>>().Subject;
        response.Data!.Titre.Should().Be("Bienvenue");
    }

    [Fact]
    public async Task GetContenuIa_DoitRetournerNotFound_QuandInexistant()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.GetContenuIaByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ContenuIaDto?)null);

        // Act
        var result = await _sut.GetContenuIa(999, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreateContenuIa_DoitRetournerContenuCree()
    {
        // Arrange
        var request = new CreateContenuIaRequest { Titre = "Nouveau", Rubrique = "aide" };
        var created = new ContenuIaDto { Id = 5, Titre = "Nouveau" };
        _adminServiceMock
            .Setup(x => x.CreateContenuIaAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result = await _sut.CreateContenuIa(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ContenuIaDto>>().Subject;
        response.Data!.Id.Should().Be(5);
    }

    [Fact]
    public async Task UpdateContenuIa_DoitRetournerOk()
    {
        // Arrange
        var request = new UpdateContenuIaRequest { Titre = "Mise a jour" };
        _adminServiceMock
            .Setup(x => x.UpdateContenuIaAsync(1, request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateContenuIa(1, request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteContenuIa_DoitRetournerOk()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.DeleteContenuIaAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteContenuIa(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // ─── Etablissements ───

    [Fact]
    public async Task GetEtablissements_DoitRetournerListePaginee()
    {
        // Arrange
        var paged = new PagedResult<AdminEtablissementDto>
        {
            Items = new List<AdminEtablissementDto> { new() { Id = 1, Name = "Ecole Alpha" } },
            TotalCount = 1, Page = 1, PageSize = 20
        };
        _adminServiceMock
            .Setup(x => x.GetEtablissementsAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        // Act
        var result = await _sut.GetEtablissements();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<PagedResult<AdminEtablissementDto>>>().Subject;
        response.Data!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetEtablissement_DoitRetournerDetail_QuandExiste()
    {
        // Arrange
        var detail = new AdminEtablissementDetailDto { Id = 1, Name = "Ecole Alpha" };
        _adminServiceMock
            .Setup(x => x.GetEtablissementByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(detail);

        // Act
        var result = await _sut.GetEtablissement(1, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<AdminEtablissementDetailDto>>().Subject;
        response.Data!.Name.Should().Be("Ecole Alpha");
    }

    [Fact]
    public async Task GetEtablissement_DoitRetournerNotFound_QuandInexistant()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.GetEtablissementByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdminEtablissementDetailDto?)null);

        // Act
        var result = await _sut.GetEtablissement(999, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateEtablissement_DoitRetournerEtablissementMisAJour()
    {
        // Arrange
        var request = new UpdateEtablissementRequest { Name = "Ecole Beta" };
        var updated = new AdminEtablissementDetailDto { Id = 1, Name = "Ecole Beta" };
        _adminServiceMock
            .Setup(x => x.UpdateEtablissementAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        // Act
        var result = await _sut.UpdateEtablissement(1, request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<AdminEtablissementDetailDto>>().Subject;
        response.Data!.Name.Should().Be("Ecole Beta");
    }

    [Fact]
    public async Task DeleteEtablissement_DoitRetournerOk()
    {
        // Arrange
        _adminServiceMock
            .Setup(x => x.DeleteEtablissementAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteEtablissement(1, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    // ─── Statistiques Forfaits ───

    [Fact]
    public async Task GetForfaitStats_DoitRetournerStatistiques()
    {
        // Arrange
        var stats = new ForfaitStatsDto { TotalEtablissements = 50, EtablissementsAvecForfait = 30 };
        _adminServiceMock
            .Setup(x => x.GetForfaitStatsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(stats);

        // Act
        var result = await _sut.GetForfaitStats(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<ForfaitStatsDto>>().Subject;
        response.Data!.TotalEtablissements.Should().Be(50);
    }

    // ─── Transactions Mobile Money ───

    [Fact]
    public async Task GetTransactionsMobile_DoitRetournerListePaginee()
    {
        // Arrange
        var paged = new PagedResult<TransactionMobileDto>
        {
            Items = new List<TransactionMobileDto> { new() { Id = 1, Operateur = "OrangeMoney", Montant = 50000 } },
            TotalCount = 1, Page = 1, PageSize = 20
        };
        _adminServiceMock
            .Setup(x => x.GetTransactionsMobileAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paged);

        // Act
        var result = await _sut.GetTransactionsMobile();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<PagedResult<TransactionMobileDto>>>().Subject;
        response.Data!.Items.Should().HaveCount(1);
        response.Data!.Items[0].Montant.Should().Be(50000);
    }

    // ─── Dashboard Stats ───

    [Fact]
    public async Task GetDashboardKpis_DoitRetournerKpis()
    {
        // Arrange
        var kpis = new DashboardKpiDto { TotalEtablissements = 10, TotalEnseignants = 50, TotalParents = 200, TotalEleves = 500 };
        _adminServiceMock
            .Setup(x => x.GetDashboardKpisAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(kpis);

        // Act
        var result = await _sut.GetDashboardKpis(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<DashboardKpiDto>>().Subject;
        response.Data!.TotalEtablissements.Should().Be(10);
        response.Data!.TotalEleves.Should().Be(500);
    }

    [Fact]
    public async Task GetRevenusMensuels_DoitRetournerRevenus()
    {
        // Arrange
        var revenus = new List<RevenuMensuelDto>
        {
            new() { Mois = "Jan", Montant = 3_500_000 },
            new() { Mois = "Fev", Montant = 4_200_000 },
        };
        _adminServiceMock
            .Setup(x => x.GetRevenusMensuelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(revenus);

        // Act
        var result = await _sut.GetRevenusMensuels(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<RevenuMensuelDto>>>().Subject;
        response.Data.Should().HaveCount(2);
        response.Data![0].Mois.Should().Be("Jan");
    }

    [Fact]
    public async Task GetRegionStats_DoitRetournerRegions()
    {
        // Arrange
        var regions = new List<RegionStatDto>
        {
            new() { Nom = "Conakry", Count = 22, Pct = 47 },
            new() { Nom = "Kindia", Count = 8, Pct = 17 },
        };
        _adminServiceMock
            .Setup(x => x.GetRegionStatsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(regions);

        // Act
        var result = await _sut.GetRegionStats(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<RegionStatDto>>>().Subject;
        response.Data.Should().HaveCount(2);
        response.Data![0].Nom.Should().Be("Conakry");
    }

    [Fact]
    public async Task GetUsageStats_DoitRetournerStatistiquesUsage()
    {
        // Arrange
        var usage = new List<UsageStatDto>
        {
            new() { Label = "Taux de connexion", Value = "68%", Trend = "+5%" },
            new() { Label = "SMS envoyes", Value = "450", Trend = "+22%" },
        };
        _adminServiceMock
            .Setup(x => x.GetUsageStatsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(usage);

        // Act
        var result = await _sut.GetUsageStats(CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<List<UsageStatDto>>>().Subject;
        response.Data.Should().HaveCount(2);
        response.Data![0].Value.Should().Be("68%");
    }
}
