# TradeFlow

Sistema de gestión comercial para la **Distribuidora Yrigoyen** (José y Martín). Aplicación de escritorio y móvil para administrar facturas (remitos), productos, clientes y localidades, con soporte para impresión, exportación a PDF y respaldo de la base de datos.

Construida con **.NET MAUI** sobre la arquitectura **MVVM** + **Repository Pattern**.

---

## Características

### Facturación
- Creación de facturas (remitos) seleccionando cliente y productos activos.
- Búsqueda de cliente y producto con sugerencias en tiempo real (typeahead).
- Cantidad configurable por producto y **descuento por línea de 0 a 10%** (lista desplegable: "Sin Descuento" o de 1% a 10%).
- Cálculo automático de subtotal por ítem y total de la factura.
- Detalle de factura con ítems, descuentos e importes.
- Vista previa del remito en HTML (Original + Duplicado con línea de corte) e impresión desde el diálogo del sistema.
- Búsqueda de facturas por número y filtro por fecha (Hoy / Todas).
- Eliminación de facturas con borrado en cascada de sus ítems.

### Productos
- CRUD completo de productos (código, nombre, precio) con estado **activo/inactivo**.
- Búsqueda en tiempo real por nombre o código (con debounce).
- Contador de productos registrados en la pantalla de listado.
- **Exportar catálogo a PDF** (todos los productos) generado directamente con QuestPDF — tabla Código / Producto / Precio.
- Al facturar solo se ofrecen **productos activos**.

### Clientes
- CRUD completo de clientes (nombre, teléfono, dirección, localidad).
- Búsqueda en tiempo real por nombre.
- Asociación con localidades.
- Bloqueo de eliminación si el cliente tiene facturas asociadas.
- Vista de facturas de un cliente.

### Localidades
- Alta, consulta y eliminación de localidades.
- Validación contra duplicados de nombre.

### Backup
- Creación de copias de seguridad de la base de datos (`.db3`) con marca de tiempo.
- Restauración de respaldos mediante selector de archivos, con respaldo automático y rollback ante fallos.

### Interfaz y validaciones
- Tema claro forzado, paleta de colores definida en `Resources/Styles`.
- Validación de formularios con errores por campo, limpieza al enfocar y resaltado del campo enfocado.
- Overlay de carga en todas las pantallas.
- Cursor de mano en elementos interactivos (Windows).

---

## Stack Tecnológico

| Tecnología | Descripción |
|---|---|
| **.NET 9 / MAUI** | Framework multiplataforma de interfaz (XAML) |
| **SQLite (sqlite-net-pcl)** | Base de datos local relacional |
| **QuestPDF** | Generación de PDF multiplataforma (catálogo de productos) |
| **CommunityToolkit.Maui** | Utilidades de MAUI (builder extension) |
| **MVVM** | Separación de lógica de negocio e interfaz |

---

## Arquitectura

La aplicación sigue el patrón **MVVM** (Model - View - ViewModel) combinado con el **Repository Pattern** y **inyección de dependencias**.

### Flujo de datos

```
Vista (XAML) → ViewModel → Repository/Service → DatabaseService → SQLite
```

- **Models**: clases planas con atributos SQLite.
- **Views**: páginas XAML; el code-behind solo asigna el `BindingContext` y llama a `InicializarAsync()` en `OnAppearing`.
- **ViewModels**: implementan `INotifyPropertyChanged` manualmente; exponen propiedades y comandos.
- **Repositories**: interfaz + implementación para cada entidad.
- **Services**: lógica transversal (impresión, backup, alertas, validaciones).

### Ciclos de vida (DI)

- **Singleton**: `DatabaseService`, repositorios y servicios.
- **Transient**: ViewModels y Views.

---

## Estructura del Proyecto

```
TradeFlow/
├── App.xaml(.cs)                  # Aplicación y temas
├── AppShell.xaml(.cs)             # Navegación (flyout) y rutas
├── MauiProgram.cs                 # Configuración de DI e inicialización
├── Controls/
│   └── CargandoOverlay.xaml(.cs)  # Overlay de carga reutilizable
├── Converters/
│   ├── FirstLetterConverter.cs    # Primera letra (avatares)
│   ├── InverseBoolConverter.cs    # Invierte bool (visibilidad)
│   └── TextoVacioConverter.cs     # Texto de respaldo si viene vacío
├── Data/
│   ├── DatabaseService.cs         # Conexión SQLite y creación de tablas
│   └── Repositories/              # Interfaces + implementaciones
│       ├── IClienteRepository.cs / ClienteRepository.cs
│       ├── IFacturaRepository.cs / FacturaRepository.cs
│       ├── ILocalidadRepository.cs / LocalidadRepository.cs
│       └── IProductoRepository.cs / ProductoRepository.cs
├── Helpers/
│   ├── AppPaths.cs                        # Rutas de datos por PC (Windows / resto)
│   ├── EjecutarComandoAlDesenfocarBehavior.cs
│   ├── HandCursor.cs                      # Cursor de mano (Windows)
│   └── LimpiaErrorAlEnfocarBehavior.cs
├── Models/
│   ├── ClienteModel.cs
│   ├── DetalleFacturaModel.cs
│   ├── FacturaModel.cs
│   ├── LocalidadModel.cs
│   └── ProductoModel.cs
├── Platforms/                     # Código por plataforma
│   ├── Android/  ├── iOS/  ├── MacCatalyst/  └── Windows/
├── Resources/
│   ├── AppIcon/  ├── Fonts/  ├── Images/  ├── Raw/  ├── Splash/
│   └── Styles/                    # Colors.xaml y Styles.xaml
├── Services/
│   ├── IDisplayAlertService.cs / DisplayAlertService.cs
│   ├── IValidacionesService.cs / ValidacionesService.cs
│   ├── IBackupService.cs / BackupService.cs
│   └── IImpresionService.cs / ImpresionService.cs
├── ViewModels/                    # 16 ViewModels (uno por pantalla)
└── Views/                         # 16 páginas XAML + code-behind
```

---

## Modelos de Datos

### Producto
| Propiedad | Tipo | Notas |
|---|---|---|
| `Id` | `int` | Clave primaria, autoincremental |
| `Codigo` | `string` | Máx. 50, opcional |
| `Nombre` | `string` | Máx. 100, requerido |
| `Precio` | `decimal` | Requerido |
| `Activo` | `bool` | Por defecto `true`; solo los activos se ofrecen al facturar |

### Cliente
| Propiedad | Tipo | Notas |
|---|---|---|
| `Id` | `int` | Clave primaria |
| `Nombre` | `string` | Máx. 100, requerido |
| `Telefono` | `string` | Máx. 20 |
| `Direccion` | `string` | Máx. 200 |
| `LocalidadId` | `int` | Indexado |
| `Localidad` | `LocalidadModel?` | Ignorada en DB |

### Factura
| Propiedad | Tipo | Notas |
|---|---|---|
| `Id` | `int` | Clave primaria |
| `Fecha` | `DateTime` | Por defecto `Now` |
| `ClienteId` | `int` | Indexado |
| `Cliente` | `ClienteModel?` | Ignorada en DB |
| `Total` | `decimal` | Suma de subtotales |
| `Items` | `List<DetalleFacturaModel>` | Ignorada en DB |

### DetalleFactura
| Propiedad | Tipo | Notas |
|---|---|---|
| `Id` | `int` | Clave primaria |
| `FacturaId` | `int` | Indexado |
| `ProductoId` | `int` | Indexado |
| `ProductoNombre` | `string` | Máx. 100 |
| `Codigo` | `string` | Máx. 50 |
| `Cantidad` | `int` | Observable |
| `PrecioUnitario` | `decimal` | |
| `DescuentoPorcentaje` | `int` | 0 a 10 |
| `PrecioFinal` | `decimal` | Observable |
| `Subtotal` | `decimal` | Observable |
| `TieneDescuento` | `bool` | Computado, ignorado |
| `DescuentoTexto` | `string` | Computado, ignorado |
| `CantidadPrecioTexto` | `string` | Computado, ignorado |

### Localidad
| Propiedad | Tipo | Notas |
|---|---|---|
| `Id` | `int` | Clave primaria |
| `Nombre` | `string` | Máx. 100, requerido |

---

## Repositorios

| Repositorio | Métodos principales |
|---|---|
| **ProductoRepository** | `ObtenerTodosAsync`, `ObtenerPorIdAsync`, `GuardarAsync`, `EliminarAsync`, `ExisteNombreAsync`, `ExisteCodigoAsync`, `BuscarAsync`, `RegistrarAsync` |
| **ClienteRepository** | `ObtenerTodosAsync`, `ObtenerPorIdAsync`, `GuardarAsync`, `EliminarAsync`, `ExisteNombreAsync`, `ObtenerPorLocalidadAsync`, `BuscarPorNombreAsync`, `RegistrarAsync` |
| **FacturaRepository** | `ObtenerTodasAsync`, `ObtenerPorIdAsync`, `ObtenerDetallesAsync`, `GuardarAsync`, `EliminarAsync`, `RegistrarAsync`, `ObtenerPorClienteAsync`, `ContarPorClienteAsync`, `BuscarPorNumeroAsync`, `ObtenerPorFechaAsync`, `ObtenerUltimasDiezAsync` |
| **LocalidadRepository** | `ObtenerTodasAsync`, `ObtenerPorIdAsync`, `RegistrarAsync`, `GuardarAsync`, `EliminarAsync`, `ExisteNombreAsync` |

---

## Servicios

| Servicio | Descripción |
|---|---|
| **DisplayAlertService** | Envuelve `DisplayAlert` para alertas y confirmaciones (testeable). |
| **ValidacionesService** | Validación de campos vacíos, selecciones y precios; parseo de decimales. |
| **BackupService** | Crea copias `.db3` y restaura respaldos con rollback automático ante fallos. |
| **ImpresionService** | Genera HTML de remito y catálogo; imprime vía WebView2 (Windows); exporta catálogo a PDF con QuestPDF. |

---

## Base de Datos

- **Archivo**: `tradeflow.db3` en la carpeta de datos (`Helpers/AppPaths.cs`): `%LOCALAPPDATA%\TradeFlow` en Windows, `FileSystem.AppDataDirectory` en el resto de las plataformas. Allí también viven los respaldos y los PDFs generados.
- **ORM**: `sqlite-net-pcl` (API asíncrona `SQLiteAsyncConnection`).
- **Creación de tablas** (orden de dependencias): `Localidad` → `Cliente` → `Producto` → `Factura` → `DetalleFactura`.
- La inicialización de tablas ocurre de forma **asíncrona en `App.OnStart`**, sin bloquear el arranque del hilo de UI, protegida con un `SemaphoreSlim` para evitar condiciones de carrera.

---

## Requisitos y Ejecución

1. Instalar el **.NET 9 SDK** y la carga de trabajo de **.NET MAUI**.
2. Clonar el repositorio.
3. Restaurar dependencias:
   ```bash
   dotnet restore
   ```
4. Ejecutar (Windows):
   ```bash
   dotnet build -t:Run -f net9.0-windows10.0.19041.0
   ```
   O abrir la solución `TradeFlow.sln` en Visual Studio 2022 y ejecutar en el dispositivo deseado.

> Nota: la **impresión** (WebView2) solo está disponible en **Windows**. La **exportación de catálogo a PDF** (QuestPDF) es multiplataforma. Además, el binario de **Windows es portable (unpackaged/self-contained)**, sin dependencia de MSIX: se puede copiar y ejecutar desde una PC o pendrive.

---

## Plataformas Soportadas

| Plataforma | Framework | Versión mínima |
|---|---|---|
| Windows | `net9.0-windows10.0.19041.0` | 10.0.17763.0 |
| Android | `net9.0-android` | 21.0 |
| iOS | `net9.0-ios` | 11.0 |
| macOS (Catalyst) | `net9.0-maccatalyst` | 13.1 |

---

## Licencia

Uso interno de la Distribuidora Yrigoyen. Sin licencia pública.
