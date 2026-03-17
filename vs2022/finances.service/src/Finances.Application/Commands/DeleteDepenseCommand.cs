using MediatR;

namespace Finances.Application.Commands;

public sealed record DeleteDepenseCommand(int Id) : IRequest<bool>;
