# API de Gestión de Pedidos

Backend del reto técnico para Desarrollador Fullstack Senior, en .NET 8 / ASP.NET Core. Incluye
login con JWT y un CRUD de Pedidos con eliminación lógica, auditoría automática y permisos por
rol. Este repo es solo el backend, no incluye frontend.

## Arquitectura

Son 4 proyectos en la raíz del repo, cada uno dependiendo solo del anterior:

```
Pedidos.Domain  →  Pedidos.Application  →  Pedidos.Infrastructure  →  Reto_Tecnico_Andres_Gallegos (Api)
```

- **Domain**: las entidades `Usuario` y `Pedido`, el enum `EstadoPedido` y las excepciones de
  negocio. No depende de EF Core ni de ASP.NET. `Pedido` tiene setters privados y se construye
  con `Pedido.Crear(...)`, que valida ahí mismo el total, el número de pedido y el cliente — así
  no hace falta confiar en que quien llame a la entidad recuerde validar antes.
- **Application**: los DTOs, las interfaces que implementa Infrastructure (`IPedidoRepository`,
  `IJwtTokenGenerator`, etc.), los servicios `AuthService` y `PedidoService` con la lógica de los
  casos de uso, los validadores de FluentValidation y las excepciones que el middleware traduce a
  HTTP.
- **Infrastructure**: `AppDbContext`, las configuraciones de EF Core, las migraciones, los
  repositorios, el interceptor de auditoría y las implementaciones concretas de JWT, BCrypt y
  "usuario actual".
- **Api**: controladores delgados, el middleware de excepciones y `Program.cs`, que básicamente
  solo arma todo lo anterior.

La idea de separarlo así es poder explicar y testear la lógica de negocio sin arrastrar EF Core o
ASP.NET con ella, y que cambiar de proveedor de base de datos o de hashing no debería tocar
`Application` ni `Domain`.

## Requisitos

- .NET SDK 8 (en esta máquina corrí con el SDK 9 instalado, que compila igual los proyectos
  `net8.0` sin problema).
- SQL Server local. En este entorno es instancia por defecto con autenticación de Windows, sin
  Docker.

## Base de datos

`PedidosDb` se puede dejar en dos estados de partida:

- **Base vacía**: al arrancar, la Api corre `Database.Migrate()`, que crea el esquema completo y
  carga el seed (2 usuarios, pedidos PE-001 a PE-005).
- **Base creada con el script** (`database/01_crear_base_datos.sql`), que es lo que hice acá:
  como la base ya tenía las tablas antes de tocar EF Core, generé la migración `InitialCreate`,
  la comparé columna por columna contra el script y, en vez de correr `database update` contra
  datos reales, registré esa migración como ya aplicada insertando la fila correspondiente en
  `__EFMigrationsHistory` (`database/02_baseline_migracion.sql`). Con eso, `Database.Migrate()`
  no intenta recrear nada al arrancar, pero el mismo código sigue sirviendo para levantar una base
  nueva desde cero en otro entorno.

Si tu instancia tiene nombre (p. ej. `SQLEXPRESS`), ajustá el `Server` de la cadena de conexión a
`localhost\\SQLEXPRESS` (la barra va escapada en el JSON).

## Configuración

Cadena de conexión y datos de JWT en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=PedidosDb;Trusted_Connection=True;TrustServerCertificate=True"
},
"Jwt": {
  "Issuer": "PedidosApi",
  "Audience": "PedidosClient",
  "ExpirationMinutes": 60
}
```

La clave de firma del JWT (`Jwt:Key`) no está en ese archivo: vive solo en
`appsettings.Development.json`, marcada como clave de desarrollo. En producción eso tiene que
salir de una variable de entorno o de un secret manager, nunca de un archivo versionado.

## Correr la API

```bash
dotnet build Reto_Tecnico_Andres_Gallegos.sln
dotnet run --project Reto_Tecnico_Andres_Gallegos
```

Queda escuchando en `http://localhost:5172`. Swagger en `http://localhost:5172/swagger`, con el
botón Authorize para pegar el Bearer token que devuelve el login.

## Usuarios de prueba

| Email | Password | Rol |
|---|---|---|
| admin@pedidos.com | Admin123* | Admin |
| user@pedidos.com | User123* | User |

## Endpoints

`POST /auth/login` recibe `{ email, password }` y devuelve `{ token, expiresIn }` (segundos). Si
las credenciales no son válidas (o el usuario está inactivo) responde 401 con un mensaje genérico,
sin decir si el email existe.

El resto va bajo `/api/pedidos` y requiere `Authorization: Bearer {token}`:

| Método | Ruta | Rol | Qué hace |
|---|---|---|---|
| GET | `/api/pedidos` | cualquiera | activos, ordenados por fecha desc |
| GET | `/api/pedidos/{id}` | cualquiera | 404 si no existe o está eliminado |
| POST | `/api/pedidos` | cualquiera | 201 + Location |
| PUT | `/api/pedidos/{id}` | cualquiera | 200 con el pedido actualizado |
| DELETE | `/api/pedidos/{id}` | Admin | 204, eliminación lógica |

`PUT` no deja cambiar `numeroPedido` — es la clave de negocio y se fija al crear el pedido, el DTO
de actualización solo trae `cliente`, `fecha`, `total` y `estado`.

Ejemplo de creación:

```json
POST /api/pedidos
{
  "numeroPedido": "PE-100",
  "cliente": "Juan Pérez",
  "fecha": "2026-02-01T00:00:00",
  "total": 500.25,
  "estado": "Registrado"
}
```

Todos los errores salen como `ProblemDetails`: 400 en validación (incluye `total <= 0`), 401 sin
token o inválido, 403 si el rol no alcanza, 404 si no existe, 409 si el número de pedido ya está
usado, 500 para lo no controlado (sin exponer stack trace).

## Reglas de negocio

`Total` tiene que ser mayor a 0 — se valida tanto en FluentValidation (para el 400 con el mensaje
por campo) como dentro de la entidad `Pedido`, que es la última línea de defensa si alguien
llamara a la entidad sin pasar por el validador.

`NumeroPedido` es único, y acá hay un detalle: como los pedidos eliminados lógicamente siguen
contando para el UNIQUE de la base, la verificación de duplicados tiene que usar
`IgnoreQueryFilters()` para verlos también. Además de ese chequeo previo, el repositorio captura
la excepción real de SQL Server (errores 2601/2627) por si dos requests pegan al mismo número casi
al mismo tiempo.

## Decisiones que vale la pena explicar

**Eliminación lógica.** Nunca se borra una fila de `Pedidos`. Un `SaveChangesInterceptor`
intercepta el `DELETE` antes de que llegue a la base y lo convierte en un `UPDATE`
(`Eliminado = true`, con usuario y fecha de modificación). Un filtro global en el `DbContext`
oculta esos pedidos de cualquier consulta, salvo que se pida explícitamente lo contrario.

**Auditoría.** El mismo interceptor asigna quién creó o modificó cada pedido, pero el "quién" sale
siempre del claim del JWT a través de `ICurrentUserService`, nunca de lo que venga en el body. Si
el body pudiera decir "este pedido lo creó el usuario 1", cualquiera podría falsificar la autoría.

**Hora local en vez de UTC.** El script original usa `GETDATE()`, así que para no mezclar zonas
horarias en los mismos datos, todo el proyecto usa `DateTime.Now` — incluida la expiración del
JWT, que igual queda bien calculada porque el framework convierte a UTC internamente a partir del
`Kind` del `DateTime`.

**Un detalle raro con los índices.** El script de base de datos no define más índices que los
UNIQUE, pero EF Core agrega automáticamente un índice no único por cada FK durante la
finalización del modelo. No hay una forma soportada de desactivar esa convención desde el Fluent
API (lo confirmó el propio equipo de EF Core en un issue de GitHub), así que terminé borrando a
mano esas dos líneas `CreateIndex` del archivo de migración para que el esquema generado coincida
exactamente con el script.

**401/403 con cuerpo.** Por defecto, JwtBearer devuelve esas respuestas vacías. Se sobreescriben
los eventos `OnChallenge`/`OnForbidden` para que devuelvan el mismo formato `ProblemDetails` que
el resto de los errores.

## Postman

`postman/Pedidos.postman_collection.json`. Corré primero "Login - Admin" (o "Login - User"): el
token queda guardado en una variable de colección y el resto de las peticiones lo usa solo. El
`POST` guarda el id creado para que el `GET/PUT/DELETE` por id no haya que tocarlo a mano.
