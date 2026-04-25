# Documentación Técnica Iterativa — LiveAlbumARtwork (Refactorización)

## Hito 1: La "Columna Vertebral" de Datos y Tracking
**Estado:** Completado
**Objetivo del Hito:** Desacoplar la detección de Vuforia de la lógica de presentación y establecer un modelo de datos canónico y normalizado para los álbumes.

---

### 1. Ubicación en la Arquitectura General
Los componentes desarrollados en este hito se insertan en las siguientes capas definidas en el documento de arquitectura formal:

* **Infraestructura Core / App Shell:** `EventBus`, `IEvent`, `TrackingEvents`, `SessionEvents`. Actúan como el sistema nervioso que permite la "Comunicación por contratos".
* **Capa 3 - Tracking y sesión AR:** `VuforiaTrackingCoordinator`, `AlbumSessionManager`. Manejan el ciclo de vida del reconocimiento sin instanciar elementos visuales.
* **Capa 2 - Servicios de aplicación:** `TargetAliasResolver`. Provee la capacidad transversal de traducir nombres brutos a identidades formales.
* **Capa 4 - Catálogo y contenido:** `CatalogDataModels`. Sustituye la dependencia de archivos locales dispersos (`Albums.json` y `Tracks.json`) por un esquema unificado y orientado a datos.

---

### 2. Descripción de Componentes

#### 2.1. Modelos de Datos (Capa 4: Catálogo)
* **`CatalogDataModels.cs`:** Contiene las clases serializables (`CatalogManifest`, `AlbumDefinition`, `AlbumVariant`, `TrackSequence`) diseñadas para ser parseadas mediante Newtonsoft Json. 
    * **Responsabilidad:** Definir el "único origen de verdad" de cada álbum.
    * **Innovación clave:** Introduce el patrón de *Variantes*, permitiendo que múltiples portadas físicas (`TargetAliases`) apunten al mismo álbum base, pero cargando assets específicos (`CoverAssetId`, `VideoAssetId`, `AnimatedCoverAssetId`) y permitiendo sobrescribir el audio y la secuencia de tracks si la edición lo requiere.

#### 2.2. Sistema de Eventos (Infraestructura Core)
* **`IEvent.cs`:** Interfaz marcadora para garantizar la seguridad de tipos en el bus.
* **`EventBus<T>.cs`:** Clase genérica estática que implementa el patrón Publicador/Suscriptor.
    * **Responsabilidad:** Eliminar las dependencias directas entre clases, permitiendo que módulos independientes se comuniquen emitiendo y escuchando mensajes globales.
* **`TrackingEvents.cs` & `SessionEvents.cs`:** Contenedores de datos (DTOs) que transportan la información del evento (ej. `TargetDetectedEvent`, `AlbumSessionStartedEvent`).

#### 2.3. Coordinador de Tracking (Capa 3: Tracking AR)
* **`VuforiaTrackingCoordinator.cs`:** Script anclado al `CloudRecoBehaviour` de Vuforia.
    * **Responsabilidad:** Detectar la imagen en la nube y detener el escaneo temporalmente. Es el único script que interactúa con la API de Vuforia. Inmediatamente publica un `TargetDetectedEvent` en el Event Bus y delega toda la responsabilidad posterior.

#### 2.4. Resolución y Sesión (Capas 2 y 3)
* **`TargetAliasResolver.cs`:** Servicio de mapeo sin estado.
    * **Responsabilidad:** Recibe el nombre crudo del target detectado por Vuforia y lo busca dentro del manifiesto del catálogo. Devuelve el identificador canónico del álbum (`AlbumId`) y la variante específica (`VariantId`) para evitar la fragilidad de depender de coincidencias exactas de strings.
* **`AlbumSessionManager.cs`:** El cerebro del estado actual de la experiencia AR.
    * **Responsabilidad:** Escucha pasivamente el `TargetDetectedEvent`. Utiliza el `TargetAliasResolver` para validar el escaneo. Si es válido, limpia la sesión anterior, actualiza su estado interno y emite el `AlbumSessionStartedEvent` para que las futuras capas de UI y Presentación comiencen su trabajo.

---

### 3. Interacción y Flujo de Dependencias

El diseño arquitectónico de este hito transforma el flujo lineal e interdependiente del prototipo original en una secuencia reactiva y modular. El ciclo de vida de un escaneo funciona de la siguiente manera:

1.  **Detección (Emisión):** La cámara enfoca una portada. Vuforia dispara su callback interno. El `VuforiaTrackingCoordinator` lo intercepta, pausa la cámara y publica `TargetDetectedEvent("NombreCrudoVuforia")` al `EventBus`. *En este punto, Vuforia deja de participar.*
2.  **Interceptación:** El `AlbumSessionManager`, que está suscrito al `EventBus`, recibe el mensaje.
3.  **Resolución (Mapeo):** El `AlbumSessionManager` pasa el string `"NombreCrudoVuforia"` al `TargetAliasResolver`. Este servicio escanea el catálogo (basado en `CatalogDataModels`) buscando qué variante contiene ese alias en su lista de `TargetAliases`.
4.  **Apertura de Sesión (Notificación):** El `TargetAliasResolver` devuelve un éxito junto con el `AlbumId` y `VariantId`. El `AlbumSessionManager` guarda estos IDs como la sesión activa y publica un `AlbumSessionStartedEvent(AlbumDefinition, VariantId)` en el `EventBus`.
5.  **En espera de Hitos Posteriores:** Actualmente, este evento viaja por el bus sin que nadie lo escuche. En los Hitos 3 y 4, la UI y el Gestor de Assets se suscribirán a este evento para descargar los recursos y construir el modelo 3D.

---

### 4. Estado Final y Preparación para Siguientes Fases
El sistema ahora es capaz de "ver" y "comprender" qué álbum se está escaneando de manera abstracta. No existe ningún código que intente mover objetos 3D, instanciar prefabs o cargar texturas localmente. La columna vertebral de información está lista para conectarse al sistema de inyección de dependencias y proveeduría de assets (Addressables) del **Hito 2**.