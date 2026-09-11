import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { ConsultorioResponse, ConsultoriosService, Medico, MedicosService } from '../../../core/generated-api';
import { PaginadorComponent } from '../../../shared/components/paginador/paginador.component';
import { MedicoFormComponent } from '../medico-form/medico-form.component';
import { ConsultorioFormComponent } from '../consultorio-form/consultorio-form.component';
import { HorariosMedicoComponent } from '../horarios-medico/horarios-medico.component';

type Tab = 'medicos' | 'consultorios';

@Component({
  selector: 'app-medicos-admin',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PaginadorComponent,
    MedicoFormComponent,
    ConsultorioFormComponent,
    HorariosMedicoComponent
  ],
  templateUrl: './medicos-admin.component.html',
  styleUrl: './medicos-admin.component.scss'
})
export class MedicosAdminComponent implements OnInit {
  private readonly medicosService = inject(MedicosService);
  private readonly consultoriosService = inject(ConsultoriosService);

  private temporizadorExito?: ReturnType<typeof setTimeout>;
  private searchQuery = '';

  protected readonly tabActiva = signal<Tab>('medicos');

  protected readonly buscarControl = new FormControl('', { nonNullable: true });

  protected readonly medicos = signal<Medico[]>([]);
  protected readonly cargandoMedicos = signal(true);
  protected readonly errorMedicos = signal<string | null>(null);

  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);

  protected readonly filasEsqueleto = Array.from({ length: 5 });

  protected readonly mostrarFormularioMedico = signal(false);
  protected readonly medicoEditar = signal<Medico | null>(null);
  protected readonly medicoHorarios = signal<Medico | null>(null);

  protected readonly consultorios = signal<ConsultorioResponse[]>([]);
  protected readonly cargandoConsultorios = signal(true);
  protected readonly errorConsultorios = signal<string | null>(null);
  protected readonly mostrarFormularioConsultorio = signal(false);
  protected readonly consultorioEditar = signal<ConsultorioResponse | null>(null);

  protected readonly mensajeExito = signal<string | null>(null);

  constructor() {
    this.buscarControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe((valor) => {
        this.searchQuery = valor.trim();
        this.currentPage.set(1);
        this.cargarMedicos();
      });
  }

  ngOnInit(): void {
    this.cargarMedicos();
    this.cargarConsultorios();
  }

  protected cambiarTab(tab: Tab): void {
    this.tabActiva.set(tab);
    this.medicoHorarios.set(null);
  }

  protected onPaginaCambiada(pagina: number): void {
    this.currentPage.set(pagina);
    this.cargarMedicos();
  }

  protected abrirCrearMedico(): void {
    this.medicoEditar.set(null);
    this.mostrarFormularioMedico.set(true);
  }

  protected abrirEditarMedico(medico: Medico): void {
    this.medicoEditar.set(medico);
    this.mostrarFormularioMedico.set(true);
  }

  protected cerrarFormularioMedico(): void {
    this.mostrarFormularioMedico.set(false);
  }

  protected onMedicoGuardado(_medico: Medico): void {
    const esEdicion = this.medicoEditar() !== null;
    this.mostrarFormularioMedico.set(false);
    this.cargarMedicos();
    this.mostrarExito(esEdicion ? 'Medico actualizado correctamente.' : 'Medico registrado correctamente.');
  }

  protected abrirHorarios(medico: Medico): void {
    this.medicoHorarios.set(medico);
  }

  protected cerrarHorarios(): void {
    this.medicoHorarios.set(null);
  }

  protected abrirCrearConsultorio(): void {
    this.consultorioEditar.set(null);
    this.mostrarFormularioConsultorio.set(true);
  }

  protected abrirEditarConsultorio(consultorio: ConsultorioResponse): void {
    this.consultorioEditar.set(consultorio);
    this.mostrarFormularioConsultorio.set(true);
  }

  protected cerrarFormularioConsultorio(): void {
    this.mostrarFormularioConsultorio.set(false);
  }

  protected onConsultorioGuardado(_consultorio: ConsultorioResponse): void {
    const esEdicion = this.consultorioEditar() !== null;
    this.mostrarFormularioConsultorio.set(false);
    this.cargarConsultorios();
    this.mostrarExito(esEdicion ? 'Consultorio actualizado correctamente.' : 'Consultorio registrado correctamente.');
  }

  protected darDeBajaConsultorio(consultorio: ConsultorioResponse): void {
    this.consultoriosService.eliminarConsultorio(consultorio.id).subscribe({
      next: () => {
        this.cargarConsultorios();
        this.mostrarExito('Consultorio dado de baja correctamente.');
      },
      error: () => this.errorConsultorios.set('No se pudo dar de baja el consultorio seleccionado.')
    });
  }

  private cargarMedicos(): void {
    this.cargandoMedicos.set(true);
    this.errorMedicos.set(null);

    this.medicosService
      .listarMedicos(this.currentPage(), this.pageSize(), this.searchQuery || undefined)
      .subscribe({
        next: (respuesta) => {
          this.medicos.set(respuesta.data);
          this.currentPage.set(respuesta.pageNumber);
          this.pageSize.set(respuesta.pageSize);
          this.totalCount.set(respuesta.totalCount);
          this.totalPages.set(respuesta.totalPages);
          this.cargandoMedicos.set(false);
        },
        error: () => {
          this.errorMedicos.set('No se pudo cargar el listado de medicos.');
          this.cargandoMedicos.set(false);
        }
      });
  }

  private cargarConsultorios(): void {
    this.cargandoConsultorios.set(true);
    this.errorConsultorios.set(null);

    this.consultoriosService.listarConsultorios().subscribe({
      next: (consultorios) => {
        this.consultorios.set(consultorios);
        this.cargandoConsultorios.set(false);
      },
      error: () => {
        this.errorConsultorios.set('No se pudo cargar el listado de consultorios.');
        this.cargandoConsultorios.set(false);
      }
    });
  }

  private mostrarExito(mensaje: string): void {
    clearTimeout(this.temporizadorExito);
    this.mensajeExito.set(mensaje);
    this.temporizadorExito = setTimeout(() => this.mensajeExito.set(null), 3000);
  }
}
