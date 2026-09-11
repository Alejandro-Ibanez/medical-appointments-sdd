import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { UsuariosService } from '../../../core/generated-api';

@Component({
  selector: 'app-cambiar-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './cambiar-password.component.html',
  styleUrl: './cambiar-password.component.scss'
})
export class CambiarPasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly usuariosService = inject(UsuariosService);

  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly exito = signal(false);

  private temporizadorExito?: ReturnType<typeof setTimeout>;

  protected readonly form = this.fb.nonNullable.group({
    passwordActual: ['', Validators.required],
    passwordNueva: ['', [Validators.required, Validators.minLength(6)]]
  });

  protected guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.error.set(null);
    this.exito.set(false);
    this.guardando.set(true);

    const { passwordActual, passwordNueva } = this.form.getRawValue();

    this.usuariosService.cambiarPasswordUsuario({ passwordActual, passwordNueva }).subscribe({
      next: () => {
        this.guardando.set(false);
        this.form.reset({ passwordActual: '', passwordNueva: '' });
        clearTimeout(this.temporizadorExito);
        this.exito.set(true);
        this.temporizadorExito = setTimeout(() => this.exito.set(false), 4000);
      },
      error: (err) => {
        this.guardando.set(false);
        const mensaje = err?.error?.mensaje;
        this.error.set(mensaje || 'No se pudo actualizar la contraseña. Intenta de nuevo.');
      }
    });
  }
}
