import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { ConsultorioResponse, ConsultoriosService } from '../../../core/generated-api';

@Component({
  selector: 'app-consultorio-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './consultorio-form.component.html',
  styleUrl: './consultorio-form.component.scss'
})
export class ConsultorioFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly consultoriosService = inject(ConsultoriosService);

  @Input() consultorio: ConsultorioResponse | null = null;

  @Output() readonly cerrar = new EventEmitter<void>();
  @Output() readonly guardado = new EventEmitter<ConsultorioResponse>();

  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    nombre: ['', Validators.required],
    ubicacion: ['', Validators.required]
  });

  protected get esEdicion(): boolean {
    return this.consultorio !== null;
  }

  ngOnInit(): void {
    if (this.consultorio) {
      this.form.patchValue({
        nombre: this.consultorio.nombre,
        ubicacion: this.consultorio.ubicacion
      });
    }
  }

  protected guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.error.set(null);
    this.guardando.set(true);

    const valores = this.form.getRawValue();
    const peticion = this.consultorio
      ? this.consultoriosService.actualizarConsultorio(this.consultorio.id, valores)
      : this.consultoriosService.crearConsultorio(valores);

    peticion.subscribe({
      next: (consultorio) => {
        this.guardando.set(false);
        this.guardado.emit(consultorio);
      },
      error: () => {
        this.guardando.set(false);
        this.error.set('No se pudo guardar el consultorio. Verifica los datos e intenta de nuevo.');
      }
    });
  }

  protected cancelar(): void {
    this.cerrar.emit();
  }
}
