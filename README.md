# 🛰️ DEAD SIGNAL: PROTOCOLO RIG (v7.2)
**Subtítulo:** *Operación Memoria — Supervivencia Táctica 16-Bit con Persistencia Relacional en SQL Server*

> *"Cada apagón borra la memoria volátil de la estación. Sincroniza tu traje RIG contra la Bóveda Relacional para que el sistema nunca vuelva a olvidar."*

---

## 📖 Sinopsis y Ambientación
La estación orbital de minería profunda **NEXUS-09** permanece a la deriva tras el colapso de su reactor de plasma. La inteligencia artificial central (**MADRE-09**) ha sellado los mamparos de la Cubierta 09 y activado el protocolo de purga, desplegando enjambres de **Drones Cazadores y Blindados**.

En el rol del **Cadete Exo**, último técnico operativo de la brigada de mantenimiento, deberás abrirte paso en una **cuadrícula táctica de 15x12** con estética retro **16-bit y filtro CRT**, utilizando cobertura física, pulsos electromagnéticos (EMP) y el respaldo de un **Rover Torreta** aliado para alcanzar la esclusa de extracción.

---

## ✨ Características Principales (v7.2)

* **Motor Gráfico 16-Bit & Monitor CRT:** Renderizado pixel-art matriz por matriz mediante GDI+ (`SmoothingMode.None`), filtro de líneas de escaneo (*scanlines*), barra de vida segmentada tipo *RIG* y animaciones en tiempo real.
* **Inteligencia Artificial Táctica:**
  * **Línea de Vista y Miras Láser:** Los drones fijan objetivos en línea recta a $\le 5$ celdas antes de disparar; los mamparos bloquean su visión.
  * **Mutación por Sabotaje:** Si un dron absorbe un **Núcleo de Energía** antes que el jugador, se sobrecarga aumentando su blindaje y daño.
  * **Aliado Autónomo:** Un Rover Torreta patrulla el perímetro inferior disparando plasma a enemigos cercanos.
* **Narrativa Dinámica y Caja Negra SQL:** Terminal de transmisiones con efecto máquina de escribir e interceptaciones de radio en tiempo real durante el combate.
* **Persistencia Relacional (`DeadSignal_DB`):** Elimina la pérdida de datos entre sesiones mediante guardado manual (`Tecla G`), auto-guardado en hitos clave y gestión de múltiples ranuras de partida.

---

## 🏗️ Arquitectura del Proyecto y POO

El proyecto (`NexusVideojuegoEngine`) está desarrollado en **C# (.NET 8 / Windows Forms)** aplicando separación de responsabilidades:

| Capa / Archivo | Responsabilidad Técnica | Conceptos Aplicados |
| :--- | :--- | :--- |
| **`Personaje` / `Cadete`** | Entidades base y especializadas del jugador. | **Clases, Objetos, Herencia (`:`) y Encapsulamiento** (`VidaMax` protegido). |
| **`ModelosPersistencia.cs`** | Objetos de Transferencia de Datos (`PartidaCompletaDTO`, `EstadoCadeteDTO`, `EntidadMapaDTO`). | **Abstracción y serialización** de estado entre RAM y SQL Server. |
| **`GestorBD.cs`** | Capa de Acceso a Datos (DAL) con `Microsoft.Data.SqlClient`. | **ADO.NET, Consultas Parametrizadas, `DataSet` y Transacciones ACID**. |
| **`FormNarrativa.cs`** | Terminal de lore inicial y lector de auditoría histórica. | **Eventos `Timer`, `RichTextBox`** y lectura de `BitacoraEventos`. |
| **`FormMenu.cs`** | Terminal de mando y autenticación de perfil RIG. | **Validación de entrada (`string.IsNullOrWhiteSpace`)** y registro/login SQL. |
| **`FormBovedaMemoria.cs`** | Gestor visual de partidas guardadas (Crear, Cargar, Purgar). | **`DataGridView` enlazado a `DataSet`**, confirmación con `MessageBox`. |
| **`FormMapa2D.cs`** | Motor de juego 2D, bucle de simulación y sincronización en vivo. | **Renderizado GDI+ 16-bit**, captura de teclado y persistencia en tiempo real. |

---

## 🗄️ Esquema Relacional en SQL Server (`DeadSignal_DB`)

La persistencia se apoya en **5 tablas normalizadas** con integridad referencial (`ON DELETE CASCADE`) y operaciones **CRUD completas** protegidas por transacciones (`BeginTransaction`, `Commit`, `Rollback`):

1. **`Jugadores`:** Perfiles de operadores RIG (`IdJugador`, `NombreUsuario`, `CodigoCadete`, `FechaRegistro`).
2. **`Partidas`:** Ranuras de guardado por jugador (`IdPartida`, `IdJugador`, `NombreRanura`, `SectorActual`, `TurnosJugados`).
3. **`EstadoCadete` (Relación 1:1):** Constantes vitales y coordenadas (`VidaActual`, `VidaMax`, `ExperienciaXP`, `Nivel`, `PosicionX`, `PosicionY`, `CargasEMP`).
4. **`EntidadesMapa` (Relación 1:N):** Fotografía exacta de drones activos, núcleos de energía y posición del Rover aliado.
5. **`BitacoraEventos` (Auditoría):** Registro cronológico de despliegues y sincronizaciones (*Caja Negra* consultable desde la interfaz).

---

## 🎮 Controles en Cubierta

| Tecla / Acción | Función Táctica |
| :--- | :--- |
| **`W` `A` `S` `D` / Flechas** | Desplazar al Cadete Exo por la cuadrícula 15x12 |
| **`Espacio`** | Emitir **Pulso EMP** (daña/aturde drones en área 3x3 adyacente) |
| **`G` / Botón `[G] GUARDAR SQL`** | Sincronizar estado vital, coordenadas y entidades en SQL Server |
| **`ESC`** | Regresar a la Terminal de Mando |

---

## ⚙️ Instalación y Ejecución Local

1. **Requisitos:** Visual Studio 2022 (.NET 8 Desktop Development) y SQL Server (LocalDB / SQLEXPRESS / Developer).
2. **Base de Datos:** Ejecutar el script `/database/01_Esquema_DeadSignal_DB.sql` en **SQL Server Management Studio (SSMS)** para crear `DeadSignal_DB` y sus 5 tablas.
3. **Cadena de Conexión:** Si tu instancia SQL no es `localhost` (por ejemplo `.\SQLEXPRESS`), ajústala en `GestorBD.cs`.
4. **Paquete NuGet:** Verificar que `Microsoft.Data.SqlClient` esté restaurado y presionar **`F5`** para iniciar.
