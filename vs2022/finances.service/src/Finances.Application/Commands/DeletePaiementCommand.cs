using MediatR;

namespace Finances.Application.Commands;

public sealed record DeletePaiementCommand(int Id) : IRequest<bool>;
