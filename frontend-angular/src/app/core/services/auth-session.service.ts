import { Injectable, computed, signal } from '@angular/core';

const TOKEN_KEY = 'auth_token';

interface JwtPayload {
  sub: string;
  unique_name: string;
  role: string;
  NombreCompleto: string;
  permission?: string | string[];
  MedicoId?: string;
  exp: number;
}

/**
 * Sesion del usuario autenticado, derivada del JWT guardado en localStorage.
 * No hace ninguna llamada al backend: solo decodifica y expone los claims.
 */
@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  private readonly payload = signal<JwtPayload | null>(this.leerPayloadInicial());

  readonly rol = computed(() => this.payload()?.role ?? null);
  readonly nombreUsuario = computed(() => this.payload()?.NombreCompleto ?? null);
  readonly nombreUsuarioLogin = computed(() => this.payload()?.unique_name ?? null);
  readonly estaAutenticado = computed(() => this.payload() !== null);

  readonly medicoId = computed(() => {
    const valor = this.payload()?.MedicoId;
    return valor ? Number(valor) : null;
  });

  readonly permisos = computed(() => this.normalizarPermisos(this.payload()?.permission));

  guardarToken(token: string): void {
    try {
      localStorage.setItem(TOKEN_KEY, token);
    } catch {
      /* localStorage no disponible; la sesion solo dura en memoria. */
    }
    this.payload.set(this.decodificar(token));
  }

  cerrarSesion(): void {
    try {
      localStorage.removeItem(TOKEN_KEY);
    } catch {
      /* ignorar */
    }
    this.payload.set(null);
  }

  esMedico(): boolean {
    return this.rol() === 'Medico';
  }

  /** true si el usuario tiene AL MENOS UNO de los codigos de permiso indicados. */
  tienePermiso(...codigos: string[]): boolean {
    const permisos = this.permisos();
    return codigos.some((codigo) => permisos.includes(codigo));
  }

  private normalizarPermisos(valor: string | string[] | undefined): string[] {
    if (!valor) {
      return [];
    }
    return Array.isArray(valor) ? valor : [valor];
  }

  private leerPayloadInicial(): JwtPayload | null {
    try {
      const token = localStorage.getItem(TOKEN_KEY);
      return token ? this.decodificar(token) : null;
    } catch {
      return null;
    }
  }

  private decodificar(token: string): JwtPayload | null {
    try {
      const [, payloadBase64] = token.split('.');
      const normalizado = payloadBase64.replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(
        atob(normalizado)
          .split('')
          .map((caracter) => '%' + caracter.charCodeAt(0).toString(16).padStart(2, '0'))
          .join('')
      );
      const payload = JSON.parse(json) as JwtPayload;

      if (!payload.exp || payload.exp * 1000 < Date.now()) {
        return null;
      }

      return payload;
    } catch {
      return null;
    }
  }
}
