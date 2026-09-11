import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { Paciente, PacientesService } from '../../../core/generated-api';
import { PacienteFormComponent } from '../paciente-form/paciente-form.component';
import { PaginadorComponent } from '../../../shared/components/paginador/paginador.component';

@Component({
  selector: 'app-pacientes-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, PacienteFormComponent, PaginadorComponent],
  templateUrl: './pacientes-list.component.html',
  styleUrl: './pacientes-list.component.scss'
})
export class PacientesListComponent implements OnInit {
  private readonly pacientesService = inject(PacientesService);
  private temporizadorExito?: ReturnType<typeof setTimeout>;
  private searchQuery = '';

  protected readonly buscarControl = new FormControl('', { nonNullable: true });

  protected readonly pacientes = signal<Paciente[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);

  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);

  /** Filas placeholder para el loading skeleton, del tamano de la pagina actual. */
  protected readonly filasEsqueleto = Array.from({ length: 5 });

  protected readonly mostrarFormulario = signal(false);
  protected readonly pacienteEditar = signal<Paciente | null>(null);
  protected readonly mensajeExito = signal<string | null>(null);

  constructor() {
    this.buscarControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe((valor) => {
        this.searchQuery = valor.trim();
        this.currentPage.set(1);
        this.cargarPacientes();
      });
  }

  ngOnInit(): void {
    this.cargarPacientes();
  }

  protected abrirCrear(): void {
    this.pacienteEditar.set(null);
    this.mostrarFormulario.set(true);
  }

  protected abrirEditar(paciente: Paciente): void {
    this.pacienteEditar.set(paciente);
    this.mostrarFormulario.set(true);
  }

  protected cerrarFormulario(): void {
    this.mostrarFormulario.set(false);
  }

  protected onGuardado(_paciente: Paciente): void {
    const esEdicion = this.pacienteEditar() !== null;
    this.mostrarFormulario.set(false);
    this.cargarPacientes();
    this.mostrarExito(esEdicion ? 'Paciente actualizado correctamente.' : 'Paciente registrado correctamente.');
  }

  protected onPaginaCambiada(pagina: number): void {
    this.currentPage.set(pagina);
    this.cargarPacientes();
  }

  private cargarPacientes(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.pacientesService
      .listarPacientes(this.currentPage(), this.pageSize(), this.searchQuery || undefined)
      .subscribe({
        next: (respuesta) => {
          this.pacientes.set(respuesta.data);
          this.currentPage.set(respuesta.pageNumber);
          this.pageSize.set(respuesta.pageSize);
          this.totalCount.set(respuesta.totalCount);
          this.totalPages.set(respuesta.totalPages);
          this.cargando.set(false);
        },
        error: () => {
          this.error.set('No se pudo cargar el listado de pacientes.');
          this.cargando.set(false);
        }
      });
  }

  private mostrarExito(mensaje: string): void {
    clearTimeout(this.temporizadorExito);
    this.mensajeExito.set(mensaje);
    this.temporizadorExito = setTimeout(() => this.mensajeExito.set(null), 3000);
  }
}
