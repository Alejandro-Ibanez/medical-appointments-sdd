using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Usuarios.Queries;

public record GetUsuariosQuery(int PageNumber, int PageSize, string? SearchQuery) : IRequest<UsuariosPagedResponse>;
