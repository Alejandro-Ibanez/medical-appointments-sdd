import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Observable, debounceTime, distinctUntilChanged, of, switchMap } from 'rxjs';

import {
  MedicoAutocompletado,
  MedicosService,
  PacienteAutocompletado,
  PacientesService,
  ReportesService
} from '../../../core/generated-api';

@Component({
  selector: 'app-centro-reportes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './centro-reportes.component.html'
})
export class CentroReportesComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly reportesService = inject(ReportesService);
  private readonly pacientesService = inject(PacientesService);
  private readonly medicosService = inject(MedicosService);

  // ---------- Tarjeta 1: Historial clinico del paciente ----------
  protected readonly formHistorial = this.fb.group({
    pacienteTexto: this.fb.control('')
  });

  protected readonly pacienteIdSeleccionado = signal<number | null>(null);
  protected readonly sugerenciasPacientes = signal<PacienteAutocompletado[]>([]);
  protected readonly mostrarSugerenciasPacientes = signal(false);
  protected readonly descargandoHistorial = signal(false);
  protected readonly errorHistorial = signal<string | null>(null);

  // ---------- Tarjeta 2: Agenda y rendimiento del medico ----------
  protected readonly formAgenda = this.fb.group({
    medicoTexto: this.fb.control(''),
    fechaInicio: this.fb.control(''),
    fechaFin: this.fb.control(''),
    formato: this.fb.control<'pdf' | 'excel'>('pdf')
  });

  protected readonly medicoIdSeleccionado = signal<number | null>(null);
  protected readonly sugerenciasMedicos = signal<MedicoAutocompletado[]>([]);
  protected readonly mostrarSugerenciasMedicos = signal(false);
  protected readonly descargandoAgenda = signal(false);
  protected readonly errorAgenda = signal<string | null>(null);

  // ---------- Tarjeta 3: Ocupacion de consultorios ----------
  protected readonly formOcupacion = this.fb.group({
    fechaInicio: this.fb.control(''),
    fechaFin: this.fb.control('')
  });

  protected readonly descargandoOcupacion = signal(false);
  protected readonly errorOcupacion = signal<string | null>(null);

  protected readonly mensajeExito = signal<string | null>(null);

  constructor() {
    this.observarAutocompletado(
      this.formHistorial.controls.pacienteTexto,
      this.pacienteIdSeleccionado,
      this.sugerenciasPacientes,
      this.mostrarSugerenciasPacientes,
      (term) => this.pacientesService.autocompletarPacientes(term)
    );

    this.observarAutocompletado(
      this.formAgenda.controls.medicoTexto,
      this.medicoIdSeleccionado,
      this.sugerenciasMedicos,
      this.mostrarSugerenciasMedicos,
      (term) => this.medicosService.autocompletarMedicos(term)
    );
  }

  protected seleccionarPaciente(paciente: PacienteAutocompletado): void {
    this.formHistorial.controls.pacienteTexto.setValue(paciente.nombreCompleto, { emitEvent: false });
    this.pacienteIdSeleccionado.set(paciente.id);
    this.mostrarSugerenciasPacientes.set(false);
  }

  protected seleccionarMedico(medico: MedicoAutocompletado): void {
    this.formAgenda.controls.medicoTexto.setValue(`${medico.nombreCompleto} · ${medico.especialidad}`, { emitEvent: false });
    this.medicoIdSeleccionado.set(medico.id);
    this.mostrarSugerenciasMedicos.set(false);
  }

  protected ocultarSugerenciasPacientesConRetraso(): void {
    setTimeout(() => this.mostrarSugerenciasPacientes.set(false), 150);
  }

  protected ocultarSugerenciasMedicosConRetraso(): void {
    setTimeout(() => this.mostrarSugerenciasMedicos.set(false), 150);
  }

  protected descargarHistorial(): void {
    const pacienteId = this.pacienteIdSeleccionado();
    if (!pacienteId) {
      this.errorHistorial.set('Selecciona un paciente de la lista de sugerencias.');
      return;
    }

    this.errorHistorial.set(null);
    this.descargandoHistorial.set(true);

    const nombreArchivo = `historial_clinico_${this.formHistorial.controls.pacienteTexto.value.trim().replace(/\s+/g, '_')}.pdf`;

    this.reportesService.generarReporteHistorialPdf(pacienteId).subscribe({
      next: (blob) => {
        this.descargarBlob(blob, nombreArchivo);
        this.descargandoHistorial.set(false);
        this.mostrarExito('Historial clínico descargado correctamente.');
      },
      error: () => {
        this.errorHistorial.set('No se pudo generar el historial clínico del paciente.');
        this.descargandoHistorial.set(false);
      }
    });
  }

  protected descargarAgenda(): void {
    const medicoId = this.medicoIdSeleccionado();
    if (!medicoId) {
      this.errorAgenda.set('Selecciona un médico de la lista de sugerencias.');
      return;
    }

    this.errorAgenda.set(null);
    this.descargandoAgenda.set(true);

    const { fechaInicio, fechaFin, formato } = this.formAgenda.getRawValue();
    const extension = formato === 'excel' ? 'xlsx' : 'pdf';
    const nombreArchivo = `agenda_${this.formAgenda.controls.medicoTexto.value.split('·')[0].trim().replace(/\s+/g, '_')}.${extension}`;

    this.reportesService
      .generarReporteAgendaMedico(medicoId, formato, fechaInicio || undefined, fechaFin || undefined)
      .subscribe({
        next: (blob) => {
          this.descargarBlob(blob, nombreArchivo);
          this.descargandoAgenda.set(false);
          this.mostrarExito('Reporte de agenda descargado correctamente.');
        },
        error: () => {
          this.errorAgenda.set('No se pudo generar el reporte de agenda del médico.');
          this.descargandoAgenda.set(false);
        }
      });
  }

  protected descargarOcupacion(): void {
    this.errorOcupacion.set(null);
    this.descargandoOcupacion.set(true);

    const { fechaInicio, fechaFin } = this.formOcupacion.getRawValue();

    this.reportesService
      .generarReporteOcupacionConsultorios(fechaInicio || undefined, fechaFin || undefined)
      .subscribe({
        next: (blob) => {
          this.descargarBlob(blob, 'ocupacion_consultorios.xlsx');
          this.descargandoOcupacion.set(false);
          this.mostrarExito('Reporte de ocupación descargado correctamente.');
        },
        error: () => {
          this.errorOcupacion.set('No se pudo generar el reporte de ocupación de consultorios.');
          this.descargandoOcupacion.set(false);
        }
      });
  }

  private observarAutocompletado<T>(
    control: FormControl<string>,
    idSeleccionado: ReturnType<typeof signal<number | null>>,
    sugerencias: ReturnType<typeof signal<T[]>>,
    mostrarSugerencias: ReturnType<typeof signal<boolean>>,
    buscar: (term: string) => Observable<T[]>
  ): void {
    control.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((valor) => {
          idSeleccionado.set(null);

          const termino = valor.trim();
          if (termino.length === 0) {
            return of([] as T[]);
          }

          return buscar(termino);
        }),
        takeUntilDestroyed()
      )
      .subscribe((resultados) => {
        sugerencias.set(resultados);
        mostrarSugerencias.set(resultados.length > 0);
      });
  }

  private descargarBlob(blob: Blob, nombreArchivo: string): void {
    const url = URL.createObjectURL(blob);
    const enlace = document.createElement('a');
    enlace.href = url;
    enlace.download = nombreArchivo;
    document.body.appendChild(enlace);
    enlace.click();
    enlace.remove();
    URL.revokeObjectURL(url);
  }

  private mostrarExito(mensaje: string): void {
    this.mensajeExito.set(mensaje);
    setTimeout(() => this.mensajeExito.set(null), 3500);
  }
}
