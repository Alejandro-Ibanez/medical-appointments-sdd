import { CommonModule } from '@angular/common';
import { Component, ViewChild, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { CalendarOptions, EventClickArg, EventInput } from '@fullcalendar/core';
import esLocale from '@fullcalendar/core/locales/es';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin, { DateClickArg } from '@fullcalendar/interaction';
import { FullCalendarComponent, FullCalendarModule } from '@fullcalendar/angular';

import {
  CitaCalendarioResponse,
  CitasService,
  DiaSemana,
  EstadoCita,
  HistorialClinicoResponse,
  HistorialClinicoService,
  HorarioAtencionResponse,
  Medico,
  MedicosService,
  Paciente,
  PacientesService
} from '../../../core/generated-api';
import { AuthSessionService } from '../../../core/services/auth-session.service';

type Pestana = 'detalle' | 'historial' | 'nueva';

const ORDEN_DIAS_SEMANA: DiaSemana[] = [
  DiaSemana.Lunes,
  DiaSemana.Martes,
  DiaSemana.Miercoles,
  DiaSemana.Jueves,
  DiaSemana.Viernes,
  DiaSemana.Sabado
];

function obtenerFechaHoyLocal(): string {
  const hoy = new Date();
  const anio = hoy.getFullYear();
  const mes = String(hoy.getMonth() + 1).padStart(2, '0');
  const dia = String(hoy.getDate()).padStart(2, '0');
  return `${anio}-${mes}-${dia}`;
}

function validarFechaNoPasada(control: AbstractControl): ValidationErrors | null {
  if (!control.value) {
    return null;
  }

  const hoy = new Date();
  hoy.setHours(0, 0, 0, 0);

  const [anio, mes, dia] = String(control.value).split('-').map(Number);
  const valor = new Date(anio, (mes || 1) - 1, dia || 1);

  return valor < hoy ? { fechaPasada: true } : null;
}

function esFechaHoraPasada(fecha: string, hora: string): boolean {
  if (!fecha || !hora) {
    return false;
  }

  return new Date(`${fecha}T${hora}:00`).getTime() < Date.now();
}

const COLOR_POR_ESTADO: Record<EstadoCita, string> = {
  Programada: '#2563eb',
  Confirmada: '#0891b2',
  Completada: '#059669',
  Cancelada: '#dc2626',
  NoAsistio: '#d97706'
};

@Component({
  selector: 'app-calendario-citas',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FullCalendarModule],
  templateUrl: './calendario-citas.component.html',
  styleUrl: './calendario-citas.component.scss'
})
export class CalendarioCitasComponent {
  private readonly citasService = inject(CitasService);
  private readonly historialService = inject(HistorialClinicoService);
  private readonly pacientesService = inject(PacientesService);
  private readonly medicosService = inject(MedicosService);
  private readonly fb = inject(FormBuilder);
  protected readonly session = inject(AuthSessionService);

  @ViewChild('calendar') private calendarComponent?: FullCalendarComponent;

  protected readonly citaSeleccionada = signal<CitaCalendarioResponse | null>(null);
  protected readonly pestanaActiva = signal<Pestana>('detalle');
  protected readonly error = signal<string | null>(null);

  protected readonly historial = signal<HistorialClinicoResponse[]>([]);
  protected readonly cargandoHistorial = signal(false);
  protected readonly errorHistorial = signal<string | null>(null);
  protected readonly pacienteDetalle = signal<Paciente | null>(null);

  protected readonly guardandoEvolucion = signal(false);
  protected readonly errorEvolucion = signal<string | null>(null);

  protected readonly mensajeExito = signal<string | null>(null);
  private temporizadorExito?: ReturnType<typeof setTimeout>;

  protected readonly mostrarFormCancelar = signal(false);
  protected readonly cancelandoCita = signal(false);
  protected readonly errorCancelar = signal<string | null>(null);

  protected readonly mostrarModalNuevaCita = signal(false);
  protected readonly guardandoCita = signal(false);
  protected readonly errorCita = signal<string | null>(null);
  protected readonly pacientesDisponibles = signal<Paciente[]>([]);
  protected readonly medicosDisponibles = signal<Medico[]>([]);

  protected readonly horariosMedicoSeleccionado = signal<HorarioAtencionResponse[]>([]);
  protected readonly cargandoHorariosMedico = signal(false);

  protected readonly fechaMinima = obtenerFechaHoyLocal();
  protected readonly esFechaPasada = signal(false);

  protected readonly formEvolucion = this.fb.nonNullable.group({
    diagnostico: ['', Validators.required],
    tratamiento: [''],
    notas: ['']
  });

  protected readonly formMotivoCancelacion = this.fb.nonNullable.group({
    motivoCancelacion: ['', Validators.required]
  });

  protected readonly formNuevaCita = this.fb.nonNullable.group({
    pacienteId: [0, [Validators.required, Validators.min(1)]],
    medicoId: [0, [Validators.required, Validators.min(1)]],
    fecha: ['', [Validators.required, validarFechaNoPasada]],
    hora: ['09:00', Validators.required],
    motivoConsulta: ['', Validators.required]
  });

  constructor() {
    // Limpia cualquier mensaje de error de agendamiento previo apenas el
    // usuario cambia la fecha, la hora o el medico seleccionado, y
    // reevalua si la combinacion fecha+hora seleccionada ya paso.
    this.formNuevaCita.controls.fecha.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => {
        this.errorCita.set(null);
        this.actualizarEstadoFechaPasada();
      });

    this.formNuevaCita.controls.hora.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => {
        this.errorCita.set(null);
        this.actualizarEstadoFechaPasada();
      });

    this.formNuevaCita.controls.medicoId.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe((medicoId) => {
        this.errorCita.set(null);
        this.cargarHorariosDelMedico(Number(medicoId));
      });
  }

  protected readonly calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, interactionPlugin],
    initialView: 'dayGridMonth',
    locale: esLocale,
    height: 'auto',
    dayMaxEvents: true,
    // Impide navegar/interactuar con dias anteriores al de hoy: esta
    // estrictamente prohibido agendar citas en fechas pasadas.
    validRange: {
      start: obtenerFechaHoyLocal()
    },
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,dayGridWeek,dayGridDay'
    },
    events: (fetchInfo, successCallback, failureCallback) => {
      this.citasService.obtenerCitasCalendario(fetchInfo.startStr, fetchInfo.endStr).subscribe({
        next: (citas) => {
          this.error.set(null);
          successCallback(citas.map((cita) => this.mapearEvento(cita)));
        },
        error: (err) => {
          this.error.set('No se pudieron cargar las citas del calendario.');
          failureCallback(err);
        }
      });
    },
    eventClick: (info: EventClickArg) => {
      const cita = info.event.extendedProps['cita'] as CitaCalendarioResponse;
      this.abrirModalDetalle(cita);
    },
    dateClick: (info: DateClickArg) => {
      this.abrirModalNuevaCita(info.dateStr);
    }
  };

  protected abrirModalDetalle(cita: CitaCalendarioResponse): void {
    this.citaSeleccionada.set(cita);
    this.pestanaActiva.set('detalle');
    this.historial.set([]);
    this.pacienteDetalle.set(null);
    this.errorHistorial.set(null);
    this.errorEvolucion.set(null);
    this.mostrarFormCancelar.set(false);
    this.errorCancelar.set(null);
    this.formEvolucion.reset({ diagnostico: '', tratamiento: '', notas: '' });
    this.formMotivoCancelacion.reset({ motivoCancelacion: '' });
  }

  protected cerrarModal(): void {
    this.citaSeleccionada.set(null);
  }

  protected seleccionarPestana(pestana: Pestana): void {
    this.pestanaActiva.set(pestana);

    if (pestana === 'historial' && this.historial().length === 0) {
      this.cargarHistorial();
    }
  }

  protected claseEstado(estado: EstadoCita): string {
    switch (estado) {
      case 'Confirmada':
      case 'Completada':
        return 'bg-emerald-100 text-emerald-700';
      case 'Cancelada':
      case 'NoAsistio':
        return 'bg-red-100 text-red-700';
      default:
        return 'bg-blue-100 text-blue-700';
    }
  }

  protected puedeCancelarse(estado: EstadoCita): boolean {
    return estado !== 'Cancelada' && estado !== 'Completada';
  }

  protected guardarEvolucion(): void {
    if (this.formEvolucion.invalid) {
      this.formEvolucion.markAllAsTouched();
      return;
    }

    const cita = this.citaSeleccionada();
    if (!cita) {
      return;
    }

    this.errorEvolucion.set(null);
    this.guardandoEvolucion.set(true);

    const { diagnostico, tratamiento, notas } = this.formEvolucion.getRawValue();

    this.historialService
      .agregarEntradaHistorialClinico(cita.pacienteId, {
        diagnostico,
        tratamiento: tratamiento || undefined,
        notas: notas || undefined,
        // El citaId de la cita activa (retenido en citaSeleccionada durante
        // toda la vida del modal) le indica al backend que marque
        // automaticamente esta cita como 'Completada'.
        citaId: cita.id
      })
      .subscribe({
        next: () => {
          this.guardandoEvolucion.set(false);
          this.formEvolucion.reset({ diagnostico: '', tratamiento: '', notas: '' });
          this.cerrarModal();
          this.mostrarExito('Evolución registrada correctamente. La cita se marcó como Completada.');
          this.calendarComponent?.getApi().refetchEvents();
        },
        error: () => {
          this.guardandoEvolucion.set(false);
          this.errorEvolucion.set('No se pudo registrar la evolucion. Verifica los datos e intenta de nuevo.');
        }
      });
  }

  protected mostrarFormularioCancelar(): void {
    this.errorCancelar.set(null);
    this.mostrarFormCancelar.set(true);
  }

  protected cancelarCita(): void {
    if (this.formMotivoCancelacion.invalid) {
      this.formMotivoCancelacion.markAllAsTouched();
      return;
    }

    const cita = this.citaSeleccionada();
    if (!cita) {
      return;
    }

    this.errorCancelar.set(null);
    this.cancelandoCita.set(true);

    const { motivoCancelacion } = this.formMotivoCancelacion.getRawValue();

    this.citasService.cancelarCita(cita.id, { motivoCancelacion }).subscribe({
      next: () => {
        this.cancelandoCita.set(false);
        this.cerrarModal();
        this.calendarComponent?.getApi().refetchEvents();
      },
      error: () => {
        this.cancelandoCita.set(false);
        this.errorCancelar.set('No se pudo cancelar la cita. Intenta de nuevo.');
      }
    });
  }

  protected abrirModalNuevaCita(fechaStr: string): void {
    this.errorCita.set(null);
    this.horariosMedicoSeleccionado.set([]);
    this.cargandoHorariosMedico.set(false);

    // Si el usuario autenticado es un Medico, se auto-asigna su propio
    // medicoId (extraido del JWT) en lugar de pedirle que se busque a si
    // mismo en un selector al que, ademas, no tiene permiso de acceder.
    const medicoIdDelUsuario = this.session.esMedico() ? this.session.medicoId() : null;

    this.formNuevaCita.reset({
      pacienteId: 0,
      medicoId: medicoIdDelUsuario ?? 0,
      fecha: fechaStr,
      hora: '09:00',
      motivoConsulta: ''
    });
    this.actualizarEstadoFechaPasada();

    if (this.pacientesDisponibles().length === 0) {
      this.pacientesService.listarPacientes(1, 200).subscribe({
        next: (respuesta) => this.pacientesDisponibles.set(respuesta.data)
      });
    }

    if (!this.session.esMedico() && this.medicosDisponibles().length === 0) {
      this.medicosService.listarMedicos(1, 200).subscribe({
        next: (respuesta) => this.medicosDisponibles.set(respuesta.data)
      });
    }

    this.mostrarModalNuevaCita.set(true);
  }

  protected cerrarModalNuevaCita(): void {
    this.mostrarModalNuevaCita.set(false);
    this.horariosMedicoSeleccionado.set([]);
  }

  protected guardarNuevaCita(): void {
    if (this.formNuevaCita.invalid || this.esFechaPasada()) {
      this.formNuevaCita.markAllAsTouched();
      return;
    }

    this.errorCita.set(null);
    this.guardandoCita.set(true);

    const { pacienteId, medicoId, fecha, hora, motivoConsulta } = this.formNuevaCita.getRawValue();
    const fechaHora = `${fecha}T${hora}:00`;

    this.citasService
      .crearCita({
        pacienteId: Number(pacienteId),
        medicoId: Number(medicoId),
        fechaHora,
        motivoConsulta
      })
      .subscribe({
        next: () => {
          this.guardandoCita.set(false);
          this.mostrarModalNuevaCita.set(false);
          this.calendarComponent?.getApi().refetchEvents();
        },
        error: (err) => {
          this.guardandoCita.set(false);
          const mensaje = err?.error?.mensaje;
          this.errorCita.set(mensaje || 'No se pudo agendar la cita. Verifica los datos e intenta de nuevo.');
        }
      });
  }

  private mostrarExito(mensaje: string): void {
    clearTimeout(this.temporizadorExito);
    this.mensajeExito.set(mensaje);
    this.temporizadorExito = setTimeout(() => this.mensajeExito.set(null), 4000);
  }

  private actualizarEstadoFechaPasada(): void {
    const { fecha, hora } = this.formNuevaCita.getRawValue();
    this.esFechaPasada.set(esFechaHoraPasada(fecha, hora));
  }

  private cargarHorariosDelMedico(medicoId: number): void {
    if (!medicoId || medicoId < 1) {
      this.horariosMedicoSeleccionado.set([]);
      this.cargandoHorariosMedico.set(false);
      return;
    }

    this.cargandoHorariosMedico.set(true);

    this.medicosService.obtenerHorariosMedico(medicoId).subscribe({
      next: (horarios) => {
        const ordenados = [...horarios].sort(
          (a, b) => ORDEN_DIAS_SEMANA.indexOf(a.diaSemana) - ORDEN_DIAS_SEMANA.indexOf(b.diaSemana)
        );
        this.horariosMedicoSeleccionado.set(ordenados);
        this.cargandoHorariosMedico.set(false);
      },
      error: () => {
        this.horariosMedicoSeleccionado.set([]);
        this.cargandoHorariosMedico.set(false);
      }
    });
  }

  private cargarHistorial(): void {
    const cita = this.citaSeleccionada();
    if (!cita) {
      return;
    }

    this.cargandoHistorial.set(true);
    this.errorHistorial.set(null);

    this.historialService.obtenerHistorialClinico(cita.pacienteId).subscribe({
      next: (entradas) => {
        this.historial.set(entradas);
        this.cargandoHistorial.set(false);
      },
      error: () => {
        this.errorHistorial.set('No se pudo cargar el historial clinico del paciente.');
        this.cargandoHistorial.set(false);
      }
    });

    this.pacientesService.obtenerPacientePorId(cita.pacienteId).subscribe({
      next: (paciente) => this.pacienteDetalle.set(paciente)
    });
  }

  private mapearEvento(cita: CitaCalendarioResponse): EventInput {
    const color = COLOR_POR_ESTADO[cita.estado] ?? '#2563eb';

    return {
      id: String(cita.id),
      title: cita.title,
      start: cita.start,
      end: cita.end,
      backgroundColor: color,
      borderColor: color,
      extendedProps: { cita }
    };
  }
}
