namespace GnValidation.Commands.Documents;

/// <summary>Commande de creation d'une signature.</summary>
public record CreateSignatureCommand(
    int DocumentGenereId,
    int SignataireId,
    int OrdreSignature,
    string StatutSignature,
    string NiveauSignature);
