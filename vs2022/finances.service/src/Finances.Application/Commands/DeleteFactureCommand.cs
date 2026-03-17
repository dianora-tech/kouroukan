using MediatR;

namespace Finances.Application.Commands;

public sealed record DeleteFactureCommand(int Id) : IRequest<bool>;
