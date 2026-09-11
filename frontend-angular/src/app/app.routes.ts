import { Routes } from '@angular/router';

import { authGuard } from './core/guards/auth.guard';
import { permissionGuard } from './core/guards/permission.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./modules/auth/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'dashboard',
    canActivate: [permissionGuard('dashboard.ver')],
    loadComponent: () =>
      import('./modules/dashboard/dashboard-page/dashboard-page.component').then(
        (m) => m.DashboardPageComponent
      )
  },
  {
    path: 'calendario',
    canActivate: [permissionGuard('citas.leer')],
    loadComponent: () =>
      import('./modules/calendar/calendario-citas/calendario-citas.component').then(
        (m) => m.CalendarioCitasComponent
      )
  },
  {
    path: 'citas/buscador',
    canActivate: [permissionGuard('citas.leer')],
    loadComponent: () =>
      import('./modules/reports/buscador-citas/buscador-citas.component').then(
        (m) => m.BuscadorCitasComponent
      )
  },
  {
    path: 'pacientes',
    canActivate: [permissionGuard('pacientes.leer')],
    loadComponent: () =>
      import('./modules/patients/pacientes-list/pacientes-list.component').then(
        (m) => m.PacientesListComponent
      )
  },
  {
    path: 'pacientes/:id',
    canActivate: [permissionGuard('pacientes.leer')],
    loadComponent: () =>
      import('./modules/patients/paciente-detalle/paciente-detalle.component').then(
        (m) => m.PacienteDetalleComponent
      )
  },
  {
    path: 'medicos',
    canActivate: [permissionGuard('medicos.administrar', 'consultorios.administrar')],
    loadComponent: () =>
      import('./modules/doctors/medicos-admin/medicos-admin.component').then(
        (m) => m.MedicosAdminComponent
      )
  },
  {
    path: 'seguridad/permisos',
    canActivate: [permissionGuard('usuarios.administrar')],
    loadComponent: () =>
      import('./modules/security/permisos-admin/permisos-admin.component').then(
        (m) => m.PermisosAdminComponent
      )
  },
  {
    path: 'usuarios',
    canActivate: [permissionGuard('usuarios.administrar')],
    loadComponent: () =>
      import('./modules/users/usuarios-admin/usuarios-admin.component').then(
        (m) => m.UsuariosAdminComponent
      )
  },
  {
    path: 'reportes-centro',
    canActivate: [permissionGuard('reportes.descargar')],
    loadComponent: () =>
      import('./modules/reports-center/centro-reportes/centro-reportes.component').then(
        (m) => m.CentroReportesComponent
      )
  },
  {
    path: 'perfil/cambiar-password',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./modules/auth/cambiar-password/cambiar-password.component').then(
        (m) => m.CambiarPasswordComponent
      )
  }
];
