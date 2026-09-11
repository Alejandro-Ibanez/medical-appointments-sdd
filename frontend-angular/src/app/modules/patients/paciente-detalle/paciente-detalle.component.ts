import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import {
  HistorialClinicoResponse,
  HistorialClinicoService,
  Paciente,
  PacientesService
} from '../../../core/generated-api';

@Component({
  selector: 'app-paciente-detalle',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './paciente-detalle.component.html',
  styleUrl: './paciente-detalle.component.scss'
})
export class PacienteDetalleComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly pacientesService = inject(PacientesService);
  private readonly historialService = inject(HistorialClinicoService);

  protected readonly paciente = signal<Paciente | null>(null);
  protected readonly historial = signal<HistorialClinicoResponse[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly entradaExpandida = signal<number | null>(null);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!id) {
      this.router.navigateByUrl('/pacientes');
      return;
    }

    this.cargando.set(true);
    this.error.set(null);

    this.pacientesService.obtenerPacientePorId(id).subscribe({
      next: (paciente) => {
        this.paciente.set(paciente);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar la informacion del paciente.');
        this.cargando.set(false);
      }
    });

    this.historialService.obtenerHistorialClinico(id).subscribe({
      next: (entradas) => this.historial.set(entradas)
    });
  }

  protected alternarEntrada(id: number): void {
    this.entradaExpandida.set(this.entradaExpandida() === id ? null : id);
  }
}
