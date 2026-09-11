import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';

type EstadoVersion = 'disponible' | 'en-desarrollo' | 'planificado';

interface EntradaChangelog {
  id: string;
  version: string;
  titulo: string;
  fecha: string;
  estado: EstadoVersion;
  cambios: string[];
}

const CAMBIOS: EntradaChangelog[] = [
  {
    id: 'v1-0-0',
    version: 'v1.0.0',
    titulo: 'Lanzamiento base',
    fecha: 'Septiembre 2026',
    estado: 'disponible',
    cambios: [
      'Gestion de pacientes, medicos y usuarios con roles (Admin, Medico, Recepcionista).',
      'Buscador avanzado de citas por palabra clave, medico, paciente y rango de fechas.',
      'Exportacion de resultados a Excel (.xlsx) desde el mismo buscador.',
      'Autenticacion basada en JWT.'
    ]
  },
  {
    id: 'v1-1-0',
    version: 'v1.1.0',
    titulo: 'Calendario medico en espanol',
    fecha: 'Septiembre 2026',
    estado: 'disponible',
    cambios: [
      'Vista de calendario mensual, semanal y diaria localizada en espanol.',
      'Carga dinamica de citas al navegar entre periodos.',
      'Modal de detalle con datos del paciente, medico y la cita seleccionada.'
    ]
  },
  {
    id: 'v1-2-0',
    version: 'v1.2.0',
    titulo: 'Notificaciones y estadisticas',
    fecha: 'Proximamente',
    estado: 'en-desarrollo',
    cambios: [
      'Recordatorios automaticos de citas por correo electronico.',
      'Panel de estadisticas de ocupacion por medico y especialidad.',
      'Historial de cambios de estado de cada cita.'
    ]
  },
  {
    id: 'v2-0-0',
    version: 'v2.0.0',
    titulo: 'Portal del paciente',
    fecha: 'Proximamente',
    estado: 'planificado',
    cambios: [
      'Auto-agendamiento de citas para pacientes registrados.',
      'Historial clinico basico consultable por el paciente.',
      'Integracion con pasarela de pagos para consultas particulares.'
    ]
  }
];

@Component({
  selector: 'app-changelog',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './changelog.component.html',
  styleUrl: './changelog.component.scss'
})
export class ChangelogComponent {
  protected readonly entradas = CAMBIOS;
  protected readonly expandido = signal<string | null>(CAMBIOS[0]?.id ?? null);

  protected alternar(id: string): void {
    this.expandido.set(this.expandido() === id ? null : id);
  }

  protected estiloEstado(estado: EstadoVersion): string {
    switch (estado) {
      case 'disponible':
        return 'bg-emerald-100 text-emerald-700';
      case 'en-desarrollo':
        return 'bg-amber-100 text-amber-700';
      default:
        return 'bg-slate-200 text-slate-600';
    }
  }

  protected etiquetaEstado(estado: EstadoVersion): string {
    switch (estado) {
      case 'disponible':
        return 'Disponible';
      case 'en-desarrollo':
        return 'En desarrollo';
      default:
        return 'Planificado';
    }
  }

  protected puntoEstado(estado: EstadoVersion): string {
    switch (estado) {
      case 'disponible':
        return 'bg-emerald-500';
      case 'en-desarrollo':
        return 'bg-amber-500';
      default:
        return 'bg-slate-400';
    }
  }
}
