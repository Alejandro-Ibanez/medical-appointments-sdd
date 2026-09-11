import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Observable, debounceTime, distinctUntilChanged, of, switchMap } from 'rxjs';

import {
  Cita,
  CitasService,
  MedicoAutocompletado,
  MedicosService,
  PacienteAutocompletado,
  PacientesService,
  ReportesService
} from '../../../core/generated-api';
import { PaginadorComponent } from '../../../shared/components/paginador/paginador.component';

interface FiltrosBusqueda {
  palabraClave?: string;
  medicoId?: number;
  pacienteId?: number;
  fechaInicio?: string;
  fechaFin?: string;
}

@Component({
  selector: 'app-buscador-citas',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PaginadorComponent],
  templateUrl: './buscador-citas.component.html',
  styleUrl: './buscador-citas.component.scss'
})
export class BuscadorCitasComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly citasService = inject(CitasService);
  private readonly reportesService = inject(ReportesService);
  private readonly pacientesService = inject(PacientesService);
  private readonly medicosService = inject(MedicosService);

  protected readonly form = this.fb.group({
    palabraClave: this.fb.control(''),
    pacienteTexto: this.fb.control(''),
    medicoTexto: this.fb.control(''),
    fechaInicio: this.fb.control(''),
    fechaFin: this.fb.control('')
  });

  protected readonly pacienteIdSeleccionado = signal<number | null>(null);
  protected readonly medicoIdSeleccionado = signal<number | null>(null);

  protected readonly sugerenciasPacientes = signal<PacienteAutocompletado[]>([]);
  protected readonly sugerenciasMedicos = signal<MedicoAutocompletado[]>([]);
  protected readonly mostrarSugerenciasPacientes = signal(false);
  protected readonly mostrarSugerenciasMedicos = signal(false);

  protected readonly citas = signal<Cita[]>([]);
  protected readonly buscando = signal(false);
  protected readonly exportandoExcel = signal(false);
  protected readonly exportandoPdf = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly buscoAlMenosUnaVez = signal(false);

  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);

  constructor() {
    this.observarAutocompletado(
      this.form.controls.pacienteTexto,
      this.pacienteIdSeleccionado,
      this.sugerenciasPacientes,
      this.mostrarSugerenciasPacientes,
      (term) => this.pacientesService.autocompletarPacientes(term)
    );

    this.observarAutocompletado(
      this.form.controls.medicoTexto,
      this.medicoIdSeleccionado,
      this.sugerenciasMedicos,
      this.mostrarSugerenciasMedicos,
      (term) => this.medicosService.autocompletarMedicos(term)
    );
  }

  protected seleccionarPaciente(paciente: PacienteAutocompletado): void {
    this.form.controls.pacienteTexto.setValue(paciente.nombreCompleto, { emitEvent: false });
    this.pacienteIdSeleccionado.set(paciente.id);
    this.mostrarSugerenciasPacientes.set(false);
  }

  protected seleccionarMedico(medico: MedicoAutocompletado): void {
    this.form.controls.medicoTexto.setValue(`${medico.nombreCompleto} · ${medico.especialidad}`, { emitEvent: false });
    this.medicoIdSeleccionado.set(medico.id);
    this.mostrarSugerenciasMedicos.set(false);
  }

  protected ocultarSugerenciasPacientesConRetraso(): void {
    setTimeout(() => this.mostrarSugerenciasPacientes.set(false), 150);
  }

  protected ocultarSugerenciasMedicosConRetraso(): void {
    setTimeout(() => this.mostrarSugerenciasMedicos.set(false), 150);
  }

  protected buscar(): void {
    this.currentPage.set(1);
    this.ejecutarBusqueda();
  }

  protected onPaginaCambiada(pagina: number): void {
    this.currentPage.set(pagina);
    this.ejecutarBusqueda();
  }

  protected exportarExcel(): void {
    this.error.set(null);
    this.exportandoExcel.set(true);

    const filtros = this.construirFiltros();

    this.reportesService
      .generarReporteExcelCitas(filtros.palabraClave, filtros.medicoId, filtros.pacienteId, filtros.fechaInicio, filtros.fechaFin)
      .subscribe({
        next: (blob) => {
          this.descargarBlob(blob, 'reporte_citas.xlsx');
          this.exportandoExcel.set(false);
        },
        error: () => {
          this.error.set('No se pudo generar el reporte de Excel.');
          this.exportandoExcel.set(false);
        }
      });
  }

  protected exportarPdf(): void {
    this.error.set(null);
    this.exportandoPdf.set(true);

    const filtros = this.construirFiltros();

    this.reportesService
      .generarReportePdfCitas(filtros.palabraClave, filtros.medicoId, filtros.pacienteId, filtros.fechaInicio, filtros.fechaFin)
      .subscribe({
        next: (blob) => {
          this.descargarBlob(blob, 'reporte_citas.pdf');
          this.exportandoPdf.set(false);
        },
        error: () => {
          this.error.set('No se pudo generar el reporte de PDF.');
          this.exportandoPdf.set(false);
        }
      });
  }

  protected limpiar(): void {
    this.form.reset({ palabraClave: '', pacienteTexto: '', medicoTexto: '', fechaInicio: '', fechaFin: '' });
    this.pacienteIdSeleccionado.set(null);
    this.medicoIdSeleccionado.set(null);
    this.citas.set([]);
    this.buscoAlMenosUnaVez.set(false);
    this.error.set(null);
    this.currentPage.set(1);
    this.totalCount.set(0);
    this.totalPages.set(0);
  }

  private ejecutarBusqueda(): void {
    this.error.set(null);
    this.buscando.set(true);

    const filtros = this.construirFiltros();

    this.citasService
      .buscarCitas(
        filtros.palabraClave,
        filtros.medicoId,
        filtros.pacienteId,
        filtros.fechaInicio,
        filtros.fechaFin,
        this.currentPage(),
        this.pageSize()
      )
      .subscribe({
        next: (resultado) => {
          this.citas.set(resultado.data);
          this.currentPage.set(resultado.pageNumber);
          this.pageSize.set(resultado.pageSize);
          this.totalCount.set(resultado.totalCount);
          this.totalPages.set(resultado.totalPages);
          this.buscoAlMenosUnaVez.set(true);
          this.buscando.set(false);
        },
        error: () => {
          this.error.set('No se pudieron obtener las citas. Intenta nuevamente.');
          this.buscando.set(false);
        }
      });
  }

  private construirFiltros(): FiltrosBusqueda {
    const { palabraClave, fechaInicio, fechaFin } = this.form.getRawValue();

    return {
      palabraClave: palabraClave.trim() ? palabraClave.trim() : undefined,
      medicoId: this.medicoIdSeleccionado() ?? undefined,
      pacienteId: this.pacienteIdSeleccionado() ?? undefined,
      fechaInicio: fechaInicio || undefined,
      fechaFin: fechaFin || undefined
    };
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
}
