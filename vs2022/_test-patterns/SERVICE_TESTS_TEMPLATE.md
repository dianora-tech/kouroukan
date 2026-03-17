# Template de tests backend pour chaque microservice Kouroukan

Ce document decrit le pattern a suivre pour creer les tests de chaque microservice.

## Structure de repertoire

```
{service}.service/tests/
├── {Service}.Tests.Unit/
│   ├── {Service}.Tests.Unit.csproj
│   ├── Services/
│   │   └── {Entity}ServiceTests.cs          # 1 par entite
│   ├── Handlers/
│   │   ├── {Entity}CommandHandlerTests.cs   # 1 par entite
│   │   └── {Entity}QueryHandlerTests.cs     # 1 par entite
│   └── Validators/
│       ├── Create{Entity}ValidatorTests.cs  # 1 par entite
│       └── Update{Entity}ValidatorTests.cs  # 1 par entite
│
└── {Service}.Tests.Integration/
    ├── {Service}.Tests.Integration.csproj
    ├── Fixtures/
    │   ├── PostgreSqlFixture.cs             # Testcontainers
    │   └── CustomWebApplicationFactory.cs   # WebApplicationFactory
    └── {Entity}EndpointTests.cs             # 1 par entite
```

## Services a couvrir

| Service         | Entites                                               |
|-----------------|-------------------------------------------------------|
| inscriptions    | Inscription, Eleve, DossierAdmission, AnneeScolaire  |
| pedagogie       | NiveauClasse, Classe, Matiere, Salle, Seance, CahierTextes |
| evaluations     | Evaluation, Note, Bulletin                            |
| presences       | Appel, Absence, Badgeage                              |
| finances        | Facture, Paiement, Depense, RemunerationEnseignant    |
| personnel       | Enseignant, DemandeConge                              |
| communication   | Message, Notification, Annonce                        |
| bde             | Association, Evenement, MembreBDE, DepenseBDE         |
| documents       | ModeleDocument, DocumentGenere, Signature             |
| services-premium| ServiceParent, Souscription                           |
| support         | Ticket, Suggestion, ArticleAide, ConversationIA       |

## Pattern ServiceTests.cs

```csharp
public sealed class {Entity}ServiceTests
{
    private readonly Mock<I{Entity}Repository> _repoMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<{Entity}Service>> _loggerMock;
    private readonly {Entity}Service _sut;

    // Test GetByIdAsync - retourne entite quand existe
    // Test GetByIdAsync - retourne null quand inexistante
    // Test GetPagedAsync - retourne resultat pagine
    // Test CreateAsync - cree l'entite avec validation metier
    // Test CreateAsync - publie evenement EntityCreatedEvent
    // Test CreateAsync - lance exception si validation echoue
    // Test UpdateAsync - retourne true et publie evenement
    // Test UpdateAsync - ne publie pas si echec
    // Test DeleteAsync - retourne true et publie evenement
    // Test DeleteAsync - retourne false si inexistante
}
```

## Pattern ValidatorTests.cs

```csharp
public sealed class Create{Entity}ValidatorTests
{
    private readonly Create{Entity}Validator _validator;

    // Test commande valide - pas d'erreur
    // Test chaque champ required vide/zero - erreur specifique
    // Test chaque champ FK zero - erreur
    // Test chaque enum avec valeur invalide - erreur
    // Test chaque enum avec toutes les valeurs valides - pas d'erreur
    // Test montants negatifs - erreur
    // Test champs optionnels null - pas d'erreur
}
```

## Pattern EndpointTests.cs (Integration)

```csharp
public sealed class {Entity}EndpointTests : IClassFixture<PostgreSqlFixture>
{
    // Test GET sans token → 401
    // Test GET avec token invalide → 401
    // Test GET avec token valide → 200
    // Test POST sans permission → 403
    // Test CGU non acceptees → 403 code CGU_NOT_ACCEPTED
    // Test CRUD complet : Create → Read → Update → Delete → verify gone
    // Test GET types
    // Test GET avec pagination
}
```

## Packages NuGet

### Tests unitaires (.csproj)
```xml
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="FluentAssertions" Version="6.12.2" />
```

### Tests integration (.csproj)
```xml
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.11" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
<PackageReference Include="FluentAssertions" Version="6.12.2" />
<PackageReference Include="Npgsql" Version="8.0.6" />
<PackageReference Include="Testcontainers.PostgreSql" Version="3.10.0" />
<PackageReference Include="WireMock.Net" Version="1.6.7" />
```

## Metriques cibles

| Metrique                               | Cible       |
|----------------------------------------|-------------|
| Couverture services + handlers         | > 80%       |
| Tests par entite (min)                 | ~15 tests   |
| Temps par service                      | < 30s       |
| Temps CI total (tous les services)     | < 10 min    |
| Zero test flaky                        | 0           |
