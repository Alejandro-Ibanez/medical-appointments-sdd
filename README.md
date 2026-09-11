# Sistema de Citas Médicas

## 1. Descripción General

**Sistema de Citas Médicas** es una plataforma full-stack diseñada para digitalizar y optimizar la operación administrativa y clínica de centros de salud, consultorios y clínicas privadas.

Desde el punto de vista **comercial**, el sistema resuelve un problema crítico de eficiencia operativa: elimina el agendamiento manual por teléfono o papel, previene el doble-agendamiento de médicos y consultorios físicos, y centraliza la administración de usuarios bajo un modelo de permisos granular, reduciendo errores humanos y tiempo administrativo.

Desde el punto de vista **médico y clínico**, garantiza la integridad del historial clínico de cada paciente (diagnósticos, tratamientos, alergias y evoluciones), automatiza el ciclo de vida de una cita (de `Programada` a `Completada` en el momento en que el médico registra la evolución) y provee reportes imprimibles y ejecutivos que respaldan tanto la continuidad del cuidado del paciente como la toma de decisiones gerenciales sobre el uso de los espacios físicos y el rendimiento del personal médico.

---

## 2. Arquitectura de Software

El proyecto está dividido en dos aplicaciones independientes que se comunican exclusivamente a través de una API REST, cada una construida bajo un patrón arquitectónico distinto pero complementario:

### Backend — `.NET Core` con Clean Architecture + CQRS

El backend implementa **Clean Architecture** en capas estrictamente desacopladas:

- **`Domain`** — Entidades del negocio, enumeraciones y excepciones de dominio. No depende de ninguna otra capa.
- **`Application`** — Casos de uso implementados bajo el patrón **CQRS** (Command Query Responsibility Segregation) con **MediatR**: cada operación del sistema es un `Command` o `Query` con su `Handler` dedicado, manteniendo la lógica de negocio aislada de la infraestructura.
- **`Infrastructure`** — Implementación concreta de persistencia con **Entity Framework Core** sobre **SQLite**, seguridad (hashing de contraseñas con BCrypt, generación de tokens JWT) y demás dependencias externas.
- **`Api`** — Controladores HTTP delgados que únicamente traducen peticiones en `Command`/`Query` de MediatR y aplican las políticas de autorización.

### Frontend — `Angular` con Arquitectura Modular por Características

El frontend sigue una **Arquitectura Modular por Características** (*Feature-Based Modular Architecture*) usando **componentes standalone** de Angular:

- Cada dominio funcional (calendario, pacientes, médicos, seguridad, reportes, etc.) vive en su propio módulo bajo `src/app/modules/`, con sus componentes, formularios reactivos y lógica de presentación encapsulados.
- El estado reactivo se maneja con **Signals** de Angular en lugar de patrones imperativos.
- El diseño visual está construido íntegramente con **Tailwind CSS**, siguiendo un sistema de utilidades consistente para lograr una interfaz moderna, responsiva y coherente en todos los módulos.
- El acceso a la API se realiza mediante un cliente HTTP **generado automáticamente**, nunca escrito a mano (ver sección 3).

---

## 3. Enfoque Specification-Driven Development (SDD)

Este proyecto se desarrolla bajo una metodología estricta de **Specification-Driven Development (SDD)**, donde el contrato de la API es la **única fuente de la verdad** del sistema.

> **`api-spec/openapi.yaml`** es el artefacto central del proyecto. Ningún endpoint, DTO o modelo de datos existe en el código antes de haber sido definido en este archivo.

El flujo de trabajo para cualquier cambio funcional sigue siempre el mismo orden:

1. **Modelar el contrato**: se modifica `api-spec/openapi.yaml` (esquemas, endpoints, parámetros, respuestas, seguridad) y se valida su sintaxis.
2. **Generar el backend**: se ejecuta **NSwag** para generar automáticamente:
   - Los DTOs (`Application/DTOs/Dtos.generated.cs`) a partir de los esquemas del contrato.
   - Las clases base abstractas de cada controlador (`Api/Generated/*ControllerBase.generated.cs`), que los controladores concretos del proyecto heredan e implementan.
3. **Implementar la lógica real**: se escriben los `Command`/`Query`/`Handler` de MediatR y se completan los métodos generados por NSwag con la lógica de negocio real.
4. **Generar el cliente Angular**: se ejecuta **OpenAPI Generator (`openapi-generator-cli`)**, que produce el cliente HTTP tipado en `frontend-angular/src/app/core/generated-api/` (servicios, modelos e interfaces), listo para inyectarse en cualquier componente Angular.
5. **Implementar la interfaz**: se construyen los componentes Angular que consumen el cliente ya generado.

Esta disciplina garantiza que backend y frontend **nunca se desincronizan** entre sí, ya que ambos derivan del mismo contrato versionado.

---

## 4. Características Principales

- 📅 **Calendario de Citas en Español** — Vista de calendario interactiva (FullCalendar) totalmente localizada, con asignación automática del médico cuando el usuario autenticado tiene el rol *Médico*, validación de disponibilidad en tiempo real y prevención de citas en fechas pasadas.
- 🔍 **Buscador Avanzado con Autocompletado** — Búsqueda de citas por palabra clave, médico, paciente y rango de fechas, con autocompletado reactivo (`debounce` + `switchMap`) sobre pacientes y médicos.
- 📊 **Suite de Reportes (Excel y PDF)** — Generación de documentos binarios reales bajo demanda:
  - Historial clínico completo del paciente en PDF (con sección de alergias destacada y espacio de firma médica).
  - Agenda y rendimiento de un médico, exportable en PDF o Excel.
  - Reporte ejecutivo de ocupación de consultorios físicos en Excel.
- 📈 **Dashboard Gerencial Premium** — Panel de indicadores clave (KPIs) del negocio: citas por estado, distribución por especialidad y métricas operativas en tiempo real.
- 🔐 **Sistema de Seguridad Dinámica por Roles y Permisos (RBAC con Claims)** — Autorización basada en permisos dinámicos almacenados en base de datos (tabla puente Rol-Permiso), inyectados como *claims* en el token JWT y evaluados mediante políticas de autorización dinámicas (`[Authorize(Policy = "recurso.accion")]`), sin necesidad de recompilar el backend para agregar nuevos permisos.

---

## 5. Requisitos Previos

Antes de instalar el proyecto, asegúrate de contar con las siguientes herramientas:

| Herramienta | Versión mínima requerida |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0 o superior (el proyecto está compilado y probado sobre .NET 10) |
| [Node.js](https://nodejs.org/) | 18 LTS o superior (Angular 22 requiere Node 22.22+ / 24.15+; se recomienda la última versión LTS disponible) |
| [Angular CLI](https://angular.dev/tools/cli) | `npm install -g @angular/cli` |
| [Entity Framework Core CLI](https://learn.microsoft.com/ef/core/cli/dotnet) | `dotnet-ef` (el repositorio ya declara la versión exacta como herramienta local en `dotnet-tools.json`) |

---

## 6. Guía de Instalación y Despliegue

### 6.1. Clonar y ubicarse en el workspace

```bash
cd proyecto
```

La estructura del workspace es la siguiente:

```
proyecto/
├── api-spec/          # Contrato OpenAPI (fuente única de la verdad)
├── backend-net/        # Solución .NET (Clean Architecture)
└── frontend-angular/   # Aplicación Angular
```

### 6.2. Backend (.NET Core)

Desde la carpeta `backend-net/`:

```bash
cd backend-net

# 1. Restaurar las dependencias NuGet de toda la solución
dotnet restore

# 2. Restaurar las herramientas locales del repositorio (incluye dotnet-ef)
dotnet tool restore

# 3. Aplicar las migraciones de Entity Framework Core y generar la base de datos SQLite
dotnet ef database update --project src/MedicalAppointments.Infrastructure --startup-project src/MedicalAppointments.Api

# 4. Levantar la API en modo desarrollo
dotnet run --project src/MedicalAppointments.Api
```

La API quedará disponible en `http://localhost:5282` (puerto configurado en `launchSettings.json`), y la especificación OpenAPI servida en vivo desde `/openapi/v1.json`.

> **Nota:** el paso 3 crea automáticamente el archivo `medicalappointments.db` (SQLite) junto con los datos semilla iniciales (roles, permisos y usuarios de prueba) definidos en las configuraciones de entidad.

### 6.3. Frontend (Angular)

En una **segunda terminal**, desde la carpeta `frontend-angular/`:

```bash
cd frontend-angular

# 1. Instalar las dependencias de npm
npm install

# 2. (Opcional) Regenerar el cliente HTTP a partir del contrato OpenAPI
npm run generate-api

# 3. Levantar el servidor de desarrollo de Angular
npm start
```

La aplicación quedará disponible en `http://localhost:4200` y consumirá automáticamente la API levantada en el paso anterior.

> **Importante:** el backend debe estar corriendo **antes** de iniciar sesión desde el frontend, ya que la autenticación y todos los módulos dependen de la API en `http://localhost:5282`.

---

## 7. Pruebas Automatizadas

El proyecto cuenta con una suite de **pruebas unitarias de alta cobertura** sobre la capa `Application` (los `Handlers` de MediatR), construida con **xUnit**, **FluentAssertions** y **Entity Framework Core InMemory**, ubicada en `backend-net/tests/MedicalAppointments.Application.Tests`.

La suite está organizada en tres categorías complementarias:

- ✅ **Happy Paths** — Casos de éxito que validan el comportamiento esperado con datos válidos (creación de citas, asignación de horarios, login, paginación, registro de historial clínico).
- ❌ **Sad Paths** — Casos fallidos que validan que se lancen las excepciones de negocio correctas ante datos inválidos (fechas pasadas, solapamientos de agenda, credenciales incorrectas, cuentas inactivas).
- 🎯 **Edge Cases** — Casos límite que estresan los algoritmos de tiempo y consultas del sistema (cierres exactos de horario, sucesión inmediata de citas, bloques continuos entre médicos, paginación fuera de rango, sanitización de búsquedas).

Para ejecutar toda la suite de pruebas:

```bash
cd backend-net
dotnet test
```

Para ejecutar las pruebas con un reporte detallado en consola:

```bash
dotnet test --logger "console;verbosity=detailed"
```

Un resultado exitoso debe mostrar el 100% de las pruebas en verde, similar a:

```
Test Run Successful.
Total tests: 23
     Passed: 23
```
