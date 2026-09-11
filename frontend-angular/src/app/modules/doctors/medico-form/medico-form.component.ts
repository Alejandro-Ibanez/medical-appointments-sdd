import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Medico, MedicosService } from '../../../core/generated-api';

@Component({
  selector: 'app-medico-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './medico-form.component.html',
  styleUrl: './medico-form.component.scss'
})
export class MedicoFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly medicosService = inject(MedicosService);

  @Input() medico: Medico | null = null;

  @Output() readonly cerrar = new EventEmitter<void>();
  @Output() readonly guardado = new EventEmitter<Medico>();

  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected get esEdicion(): boolean {
    return this.medico !== null;
  }

  protected readonly form = this.fb.nonNullable.group({
    nombreCompleto: ['', Validators.required],
    documentoIdentidad: ['', Validators.required],
    especialidad: ['', Validators.required],
    numeroColegiado: ['', Validators.required],
    telefono: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    activo: [true],
    username: [''],
    password: ['']
  });

  ngOnInit(): void {
    if (this.medico) {
      this.form.patchValue({
        nombreCompleto: this.medico.nombreCompleto,
        documentoIdentidad: this.medico.documentoIdentidad,
        especialidad: this.medico.especialidad,
        numeroColegiado: this.medico.numeroColegiado,
        telefono: this.medico.telefono ?? '',
        email: this.medico.email ?? '',
        activo: this.medico.activo ?? true
      });
    } else {
      const usernameControl = this.form.controls.username;
      const passwordControl = this.form.controls.password;
      usernameControl.addValidators(Validators.required);
      passwordControl.addValidators([Validators.required, Validators.minLength(6)]);
      usernameControl.updateValueAndValidity();
      passwordControl.updateValueAndValidity();
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

    const peticion = this.esEdicion
      ? this.medicosService.actualizarMedico(this.medico!.id, {
          nombreCompleto: valores.nombreCompleto,
          documentoIdentidad: valores.documentoIdentidad,
          especialidad: valores.especialidad,
          numeroColegiado: valores.numeroColegiado,
          telefono: valores.telefono,
          email: valores.email,
          activo: valores.activo
        })
      : this.medicosService.crearMedico({
          nombreCompleto: valores.nombreCompleto,
          documentoIdentidad: valores.documentoIdentidad,
          especialidad: valores.especialidad,
          numeroColegiado: valores.numeroColegiado,
          telefono: valores.telefono,
          email: valores.email,
          username: valores.username,
          password: valores.password
        });

    peticion.subscribe({
      next: (medico) => {
        this.guardando.set(false);
        this.guardado.emit(medico);
      },
      error: (err) => {
        this.guardando.set(false);
        const mensaje = err?.error?.mensaje;
        this.error.set(mensaje || 'No se pudo guardar el medico. Verifica los datos e intenta de nuevo.');
      }
    });
  }

  protected cancelar(): void {
    this.cerrar.emit();
  }
}
