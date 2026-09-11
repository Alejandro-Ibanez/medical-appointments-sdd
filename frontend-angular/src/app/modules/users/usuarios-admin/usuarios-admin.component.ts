import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';

import { UsuarioResponse, UsuariosService } from '../../../core/generated-api';
import { PaginadorComponent } from '../../../shared/components/paginador/paginador.component';
import { UsuarioFormComponent } from '../usuario-form/usuario-form.component';

@Component({
  selector: 'app-usuarios-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PaginadorComponent, UsuarioFormComponent],
  templateUrl: './usuarios-admin.component.html',
  styleUrl: './usuarios-admin.component.scss'
})
export class UsuariosAdminComponent implements OnInit {
  private readonly usuariosService = inject(UsuariosService);

  private temporizadorExito?: ReturnType<typeof setTimeout>;
  private searchQuery = '';

  protected readonly buscarControl = new FormControl('', { nonNullable: true });

  protected readonly usuarios = signal<UsuarioResponse[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);

  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);

  protected readonly filasEsqueleto = Array.from({ length: 5 });

  protected readonly mostrarFormulario = signal(false);
  protected readonly mensajeExito = signal<string | null>(null);
  protected readonly actualizandoEstadoId = signal<number | null>(null);

  constructor() {
    this.buscarControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed())
      .subscribe((valor) => {
        this.searchQuery = valor.trim();
        this.currentPage.set(1);
        this.cargarUsuarios();
      });
  }

  ngOnInit(): void {
    this.cargarUsuarios();
  }

  protected abrirCrear(): void {
    this.mostrarFormulario.set(true);
  }

  protected cerrarFormulario(): void {
    this.mostrarFormulario.set(false);
  }

  protected onUsuarioGuardado(_usuario: UsuarioResponse): void {
    this.mostrarFormulario.set(false);
    this.cargarUsuarios();
    this.mostrarExito('Usuario registrado correctamente.');
  }

  protected onPaginaCambiada(pagina: number): void {
    this.currentPage.set(pagina);
    this.cargarUsuarios();
  }

  protected alternarEstado(usuario: UsuarioResponse): void {
    this.error.set(null);
    this.actualizandoEstadoId.set(usuario.id);

    this.usuariosService.actualizarEstadoUsuario(usuario.id, { activo: !usuario.activo }).subscribe({
      next: () => {
        this.actualizandoEstadoId.set(null);
        this.cargarUsuarios();
        this.mostrarExito(
          usuario.activo ? 'Usuario desactivado correctamente.' : 'Usuario activado correctamente.'
        );
      },
      error: () => {
        this.actualizandoEstadoId.set(null);
        this.error.set('No se pudo actualizar el estado del usuario.');
      }
    });
  }

  private cargarUsuarios(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.usuariosService
      .listarUsuarios(this.currentPage(), this.pageSize(), this.searchQuery || undefined)
      .subscribe({
        next: (respuesta) => {
          this.usuarios.set(respuesta.data);
          this.currentPage.set(respuesta.pageNumber);
          this.pageSize.set(respuesta.pageSize);
          this.totalCount.set(respuesta.totalCount);
          this.totalPages.set(respuesta.totalPages);
          this.cargando.set(false);
        },
        error: () => {
          this.error.set('No se pudo cargar el listado de usuarios.');
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
