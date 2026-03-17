using Finances.Domain.Entities;
using MediatR;

namespace Finances.Application.Queries;

public sealed record GetRemunerationEnseignantByIdQuery(int Id) : IRequest<RemunerationEnseignant?>;
