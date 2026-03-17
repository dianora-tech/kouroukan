using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Queries;

public sealed record GetAllRemunerationsEnseignantsQuery : IRequest<IReadOnlyList<RemunerationEnseignant>>;
