using MediatR;

namespace Finances.Application.Commands;

public sealed record DeleteRemunerationEnseignantCommand(int Id) : IRequest<bool>;
