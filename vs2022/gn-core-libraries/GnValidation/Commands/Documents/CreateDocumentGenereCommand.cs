namespace GnValidation.Commands.Documents;

/// <summary>Commande de creation d'un document genere.</summary>
public record CreateDocumentGenereCommand(
    string Name,
    int ModeleDocumentId,
    int? EleveId,
    int? EnseignantId,
    string DonneesJson,
    string StatutSignature);
