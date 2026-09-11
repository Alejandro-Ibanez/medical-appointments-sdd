import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import {
  ConsultorioResponse,
  ConsultoriosService,
  DiaSemana,
  HorarioAtencionResponse,
  Medico,
  MedicosService
} from '../../../core/generated-api';

@Component({
  selector: 'app-horarios-medico',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './horarios-medico.component.html',
  styleUrl: './horarios-medico.component.scss'
})
export class HorariosMedicoComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly medicosService = inject(MedicosService);
  private readonly consultoriosService = inject(ConsultoriosService);

  @Input({ required: true }) medico!: Medico;
  @Output() readonly cerrar = new EventEmitter<void>();

  protected readonly diasSemana = Object.values(DiaSemana);

  protected readonly horarios = signal<HorarioAtencionResponse[]>([]);
  protected readonly consultorios = signal<ConsultorioResponse[]>([]);
  protected readonly consultoriosActivos = computed(() => this.consultorios().filter((c) => c.activo));

  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly errorFormulario = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    diaSemana: [DiaSemana.Lunes, Validators.required],
    horaInicio: ['08:00', Validators.required],
    horaFin: ['09:00', Validators.required],
    consultorioId: [0, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.cargarHorarios();
    this.cargarConsultorios();
  }

  private cargarHorarios(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.medicosService.obtenerHorariosMedico(this.medico.id).subscribe({
      next: (horarios) => {
        this.horarios.set(horarios);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar la disponibilidad horaria del medico.');
        this.cargando.set(false);
      }
    });
  }

  private cargarConsultorios(): void {
    this.consultoriosService.listarConsultorios().subscribe({
      next: (consultorios) => this.consultorios.set(consultorios)
    });
  }

  protected agregarHorario(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.errorFormulario.set(null);
    this.guardando.set(true);

    const valores = this.form.getRawValue();

    this.medicosService
      .agregarHorarioMedico(this.medico.id, {
        diaSemana: valores.diaSemana,
        horaInicio: valores.horaInicio,
        horaFin: valores.horaFin,
        consultorioId: Number(valores.consultorioId)
      })
      .subscribe({
        next: () => {
          this.guardando.set(false);
          this.form.patchValue({ horaInicio: '08:00', horaFin: '09:00' });
          this.cargarHorarios();
        },
        error: (err) => {
          this.guardando.set(false);
          const mensaje = err?.error?.mensaje;
          this.errorFormulario.set(
            mensaje || 'No se pudo asignar el horario. Verifica que no exista un solapamiento.'
          );
        }
      });
  }

  protected eliminarHorario(horario: HorarioAtencionResponse): void {
    this.medicosService.eliminarHorarioAtencion(horario.id).subscribe({
      next: () => this.cargarHorarios(),
      error: () => this.error.set('No se pudo eliminar el horario seleccionado.')
    });
  }

  protected volver(): void {
    this.cerrar.emit();
  }
}
