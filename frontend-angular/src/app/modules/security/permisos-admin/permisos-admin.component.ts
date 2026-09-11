import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { PermisoResponse, RolUsuario, SeguridadService } from '../../../core/generated-api';

@Component({
  selector: 'app-permisos-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './permisos-admin.component.html',
  styleUrl: './permisos-admin.component.scss'
})
export class PermisosAdminComponent implements OnInit {
  private readonly seguridadService = inject(SeguridadService);

  protected readonly roles = Object.values(RolUsuario);
  protected readonly rolSeleccionado = signal<RolUsuario>(RolUsuario.Admin);

  protected readonly todosLosPermisos = signal<PermisoResponse[]>([]);
  protected readonly permisosSeleccionadosIds = signal<Set<number>>(new Set());

  protected readonly cargandoCatalogo = signal(true);
  protected readonly cargandoRol = signal(true);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly guardadoExitoso = signal(false);

  protected readonly hayCambiosPendientes = signal(false);
  private permisosOriginalesIds = new Set<number>();

  ngOnInit(): void {
    this.cargandoCatalogo.set(true);
    this.seguridadService.listarPermisos().subscribe({
      next: (permisos) => {
        this.todosLosPermisos.set(permisos);
        this.cargandoCatalogo.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar el catalogo de permisos.');
        this.cargandoCatalogo.set(false);
      }
    });

    this.cargarPermisosDelRol();
  }

  protected seleccionarRol(rol: string): void {
    this.rolSeleccionado.set(rol as RolUsuario);
    this.guardadoExitoso.set(false);
    this.cargarPermisosDelRol();
  }

  protected estaActivo(permisoId: number): boolean {
    return this.permisosSeleccionadosIds().has(permisoId);
  }

  protected alternarPermiso(permisoId: number): void {
    const actuales = new Set(this.permisosSeleccionadosIds());

    if (actuales.has(permisoId)) {
      actuales.delete(permisoId);
    } else {
      actuales.add(permisoId);
    }

    this.permisosSeleccionadosIds.set(actuales);
    this.hayCambiosPendientes.set(!this.mismoConjunto(actuales, this.permisosOriginalesIds));
    this.guardadoExitoso.set(false);
  }

  protected guardar(): void {
    this.error.set(null);
    this.guardando.set(true);

    const permisosIds = Array.from(this.permisosSeleccionadosIds());

    this.seguridadService.actualizarPermisosDeRol(this.rolSeleccionado(), { permisosIds }).subscribe({
      next: (permisosActualizados) => {
        this.permisosOriginalesIds = new Set(permisosActualizados.map((p) => p.id));
        this.hayCambiosPendientes.set(false);
        this.guardando.set(false);
        this.guardadoExitoso.set(true);
      },
      error: () => {
        this.guardando.set(false);
        this.error.set('No se pudieron guardar los permisos del rol.');
      }
    });
  }

  private cargarPermisosDelRol(): void {
    this.cargandoRol.set(true);
    this.error.set(null);

    this.seguridadService.obtenerPermisosDeRol(this.rolSeleccionado()).subscribe({
      next: (permisos) => {
        this.permisosOriginalesIds = new Set(permisos.map((p) => p.id));
        this.permisosSeleccionadosIds.set(new Set(this.permisosOriginalesIds));
        this.hayCambiosPendientes.set(false);
        this.cargandoRol.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar los permisos del rol seleccionado.');
        this.cargandoRol.set(false);
      }
    });
  }

  private mismoConjunto(a: Set<number>, b: Set<number>): boolean {
    if (a.size !== b.size) {
      return false;
    }
    for (const valor of a) {
      if (!b.has(valor)) {
        return false;
      }
    }
    return true;
  }
}
