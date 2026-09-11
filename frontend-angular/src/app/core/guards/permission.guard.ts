import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthSessionService } from '../services/auth-session.service';

/**
 * Protege una ruta exigiendo que el usuario autenticado tenga al menos uno
 * de los codigos de permiso indicados (ej. permissionGuard('citas.leer')).
 * Si no esta autenticado lo envia a /login; si esta autenticado pero sin
 * el permiso, lo envia a /dashboard.
 */
export function permissionGuard(...codigos: string[]): CanActivateFn {
  return () => {
    const session = inject(AuthSessionService);
    const router = inject(Router);

    if (!session.estaAutenticado()) {
      return router.createUrlTree(['/login']);
    }

    if (session.tienePermiso(...codigos)) {
      return true;
    }

    return router.createUrlTree(['/dashboard']);
  };
}
