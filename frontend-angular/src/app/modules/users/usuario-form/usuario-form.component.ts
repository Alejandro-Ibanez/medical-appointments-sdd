import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import {
  Medico,
  MedicosService,
  RolResponse,
  SeguridadService,
  UsuarioResponse,
  UsuariosService
} from '../../../core/generated-api';

const NOMBRE_ROL_MEDICO = 'Medico';

@Component({
  selector: 'app-usuario-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './usuario-form.component.html',
  styleUrl: './usuario-form.component.scss'
})
export class UsuarioFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly usuariosService = inject(UsuariosService);
  private readonly seguridadService = inject(SeguridadService);
  private readonly medicosService = inject(MedicosService);

  @Output() readonly cerrar = new EventEmitter<void>();
  @Output() readonly guardado = new EventEmitter<UsuarioResponse>();

  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly roles = signal<RolResponse[]>([]);
  protected readonly medicosDisponibles = signal<Medico[]>([]);
  protected readonly mostrarSelectorMedico = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    nombreCompleto: ['', Validators.required],
    username: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]],
    rolId: [0, [Validators.required, Validators.min(1)]],
    medicoId: [0]
  });

  ngOnInit(): void {
    this.seguridadService.listarRoles().subscribe({
      next: (roles) => this.roles.set(roles),
      error: () => this.error.set('No se pudo cargar el catalogo de roles.')
    });

    this.form.controls.rolId.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe((rolId) => this.actualizarVisibilidadMedico(Number(rolId)));
  }

  protected guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.error.set(null);
    this.guardando.set(true);

    const valores = this.form.getRawValue();

    this.usuariosService
      .crearUsuario({
        nombreCompleto: valores.nombreCompleto,
        username: valores.username,
        password: valores.password,
        rolId: Number(valores.rolId),
        medicoId: this.mostrarSelectorMedico() ? Number(valores.medicoId) : undefined
      })
      .subscribe({
        next: (usuario) => {
          this.guardando.set(false);
          this.guardado.emit(usuario);
        },
        error: (err) => {
          this.guardando.set(false);
          const mensaje = err?.error?.mensaje;
          this.error.set(mensaje || 'No se pudo registrar el usuario. Verifica los datos e intenta de nuevo.');
        }
      });
  }

  protected cancelar(): void {
    this.cerrar.emit();
  }

  private actualizarVisibilidadMedico(rolId: number): void {
    const rol = this.roles().find((r) => r.id === rolId);
    const esMedico = rol?.nombre === NOMBRE_ROL_MEDICO;

    this.mostrarSelectorMedico.set(esMedico);

    if (esMedico) {
      this.form.controls.medicoId.setValidators([Validators.required, Validators.min(1)]);

      if (this.medicosDisponibles().length === 0) {
        this.medicosService.listarMedicos(1, 200).subscribe({
          next: (respuesta) => this.medicosDisponibles.set(respuesta.data)
        });
      }
    } else {
      this.form.controls.medicoId.clearValidators();
      this.form.controls.medicoId.setValue(0);
    }

    this.form.controls.medicoId.updateValueAndValidity();
  }
}
