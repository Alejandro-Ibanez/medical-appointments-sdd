using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainHorario = MedicalAppointments.Domain.Entities.HorarioAtencion;

namespace MedicalAppointments.Application.Features.Medicos.Commands;

public class AsignarHorarioCommandHandler : IRequestHandler<AsignarHorarioCommand, HorarioAtencionResponse>
{
    private readonly IApplicationDbContext _context;

    public AsignarHorarioCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HorarioAtencionResponse> Handle(AsignarHorarioCommand request, CancellationToken cancellationToken)
    {
        var medicoExiste = await _context.Medicos.AnyAsync(m => m.Id == request.MedicoId, cancellationToken);
        if (!medicoExiste)
        {
            throw new NotFoundException(nameof(Medico), request.MedicoId);
        }

        var consultorio = await _context.Consultorios
            .FirstOrDefaultAsync(c => c.Id == request.ConsultorioId, cancellationToken)
            ?? throw new NotFoundException(nameof(MedicalAppointments.Domain.Entities.Consultorio), request.ConsultorioId);

        var diaSemana = Enum.Parse<MedicalAppointments.Domain.Enums.DiaSemana>(request.DiaSemana.ToString());

        // a) El medico no debe tener otro horario que se solape en el mismo dia y rango de horas.
        var medicoOcupado = await _context.HorariosAtencion.AnyAsync(h =>
            h.MedicoId == request.MedicoId &&
            h.DiaSemana == diaSemana &&
            h.HoraInicio < request.HoraFin &&
            request.HoraInicio < h.HoraFin,
            cancellationToken);

        if (medicoOcupado)
        {
            throw new BusinessRuleException(
                "El medico ya tiene un horario asignado que se solapa con el rango indicado para ese dia.");
        }

        // b) El consultorio no debe estar reservado por otro medico en ese mismo dia y rango de horas.
        var consultorioOcupado = await _context.HorariosAtencion.AnyAsync(h =>
            h.ConsultorioId == request.ConsultorioId &&
            h.DiaSemana == diaSemana &&
            h.HoraInicio < request.HoraFin &&
            request.HoraInicio < h.HoraFin,
            cancellationToken);

        if (consultorioOcupado)
        {
            throw new BusinessRuleException(
                "El consultorio ya esta reservado por otro medico en ese mismo dia y rango horario.");
        }

        var horario = new DomainHorario
        {
            MedicoId = request.MedicoId,
            ConsultorioId = request.ConsultorioId,
            DiaSemana = diaSemana,
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin
        };

        _context.HorariosAtencion.Add(horario);
        await _context.SaveChangesAsync(cancellationToken);

        horario.Consultorio = consultorio;

        return horario.ToDto();
    }
}
