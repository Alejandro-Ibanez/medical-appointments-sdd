import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { GeneroPaciente, Paciente, PacientesService } from '../../../core/generated-api';

@Component({
  selector: 'app-paciente-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './paciente-form.component.html',
  styleUrl: './paciente-form.component.scss'
})
export class PacienteFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly pacientesService = inject(PacientesService);

  /** Cuando se recibe un paciente, el formulario opera en modo edicion. */
  @Input() paciente: Paciente | null = null;

  @Output() readonly cerrar = new EventEmitter<void>();
  @Output() readonly guardado = new EventEmitter<Paciente>();

  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly opcionesGenero = Object.values(GeneroPaciente);

  protected readonly form = this.fb.nonNullable.group({
    nombreCompleto: ['', Validators.required],
    documentoIdentidad: ['', Validators.required],
    fechaNacimiento: ['', Validators.required],
    telefono: [''],
    email: ['', Validators.email],
    direccion: [''],
    activo: [true],
    genero: [GeneroPaciente.Femenino as GeneroPaciente, Validators.required],
    alergias: ['']
  });

  protected get esEdicion(): boolean {
    return this.paciente !== null;
  }

  ngOnInit(): void {
    if (this.paciente) {
      this.form.patchValue({
        nombreCompleto: this.paciente.nombreCompleto,
        documentoIdentidad: this.paciente.documentoIdentidad,
        fechaNacimiento: this.paciente.fechaNacimiento,
        telefono: this.paciente.telefono ?? '',
        email: this.paciente.email ?? '',
        direccion: this.paciente.direccion ?? '',
        activo: this.paciente.activo ?? true,
        genero: this.paciente.genero ?? GeneroPaciente.Femenino,
        alergias: this.paciente.alergias ?? ''
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
    const input = {
      nombreCompleto: valores.nombreCompleto,
      documentoIdentidad: valores.documentoIdentidad,
      fechaNacimiento: valores.fechaNacimiento,
      telefono: valores.telefono || undefined,
      email: valores.email || undefined,
      direccion: valores.direccion || undefined,
      activo: valores.activo,
      genero: valores.genero,
      alergias: valores.alergias || undefined
    };

    const peticion = this.paciente
      ? this.pacientesService.actualizarPaciente(this.paciente.id, input)
      : this.pacientesService.crearPaciente(input);

    peticion.subscribe({
      next: (paciente) => {
        this.guardando.set(false);
        this.guardado.emit(paciente);
      },
      error: () => {
        this.guardando.set(false);
        this.error.set('No se pudo guardar el paciente. Verifica los datos e intenta de nuevo.');
      }
    });
  }

  protected cancelar(): void {
    this.cerrar.emit();
  }
}
