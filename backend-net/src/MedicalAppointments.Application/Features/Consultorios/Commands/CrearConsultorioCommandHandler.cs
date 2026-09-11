using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using DomainConsultorio = MedicalAppointments.Domain.Entities.Consultorio;

namespace MedicalAppointments.Application.Features.Consultorios.Commands;

public class CrearConsultorioCommandHandler : IRequestHandler<CrearConsultorioCommand, ConsultorioResponse>
{
    private readonly IApplicationDbContext _context;

    public CrearConsultorioCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConsultorioResponse> Handle(CrearConsultorioCommand request, CancellationToken cancellationToken)
    {
        var consultorio = new DomainConsultorio
        {
            Nombre = request.Nombre,
            Ubicacion = request.Ubicacion,
            Activo = true
        };

        _context.Consultorios.Add(consultorio);
        await _context.SaveChangesAsync(cancellationToken);

        return consultorio.ToDto();
    }
}
