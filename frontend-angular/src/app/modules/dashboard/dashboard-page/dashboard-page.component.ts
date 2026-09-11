import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';

import { DashboardMetricas, DashboardService } from '../../../core/generated-api';
import { ChangelogComponent } from '../changelog/changelog.component';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, ChangelogComponent],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.scss'
})
export class DashboardPageComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);
  private readonly router = inject(Router);

  protected readonly metricas = signal<DashboardMetricas | null>(null);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);

  protected readonly demandaMaxima = computed(() => {
    const demandas = this.metricas()?.demandasPorEspecialidad ?? [];
    return demandas.reduce((maximo, demanda) => Math.max(maximo, demanda.totalCitas), 0);
  });

  ngOnInit(): void {
    this.cargarMetricas();
  }

  protected irACalendario(): void {
    this.router.navigateByUrl('/calendario');
  }

  protected irABuscador(): void {
    this.router.navigateByUrl('/citas/buscador');
  }

  protected irAPacientes(): void {
    this.router.navigateByUrl('/pacientes');
  }

  protected porcentajeDemanda(totalCitas: number): number {
    const maximo = this.demandaMaxima();
    return maximo === 0 ? 0 : Math.round((totalCitas / maximo) * 100);
  }

  private cargarMetricas(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.dashboardService.obtenerMetricasDashboard().subscribe({
      next: (metricas) => {
        this.metricas.set(metricas);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar las metricas del dashboard.');
        this.cargando.set(false);
      }
    });
  }
}
