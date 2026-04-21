using UnityEngine;
using LiveAlbumARtwork.App.Core.Events;
using LiveAlbumARtwork.App.Core.Services.Catalog;
using LiveAlbumARtwork.Content.Catalog.DataModels;

namespace LiveAlbumARtwork.App.AR.Session
{
    public class AlbumSessionManager : MonoBehaviour
    {
        [Header("Servicios")]
        private TargetAliasResolver _aliasResolver;
        
        // Referencia al catálogo
        private CatalogManifest _currentCatalog;

        [Header("Estado de Sesión")]
        public string activeAlbumId;
        public string activeVariantId;

        void Awake()
        {
            _aliasResolver = new TargetAliasResolver();
        }

        void OnEnable()
        {
            EventBus<TargetDetectedEvent>.Subscribe(OnTargetDetected);
        }

        void OnDisable()
        {
            EventBus<TargetDetectedEvent>.Unsubscribe(OnTargetDetected);
        }

        private void OnTargetDetected(TargetDetectedEvent evt)
        {
            // 1. Intentamos resolver el target a una identidad canónica
            if (_aliasResolver.TryResolve(_currentCatalog, evt.TargetName, out AlbumDefinition album, out string vId))
            {
                StartNewSession(album, vId);
            }
            else
            {
                Debug.LogWarning($"[SessionManager] No se encontró registro para: {evt.TargetName}");
                // Aquí podrías emitir un evento de "Error de reconocimiento"
            }
        }

        private void StartNewSession(AlbumDefinition album, string variantId)
        {
            // 2. Si ya hay una sesión del mismo álbum y variante, no reiniciamos
            if (activeAlbumId == album.AlbumId && activeVariantId == variantId) return;

            // 3. Limpieza de la sesión anterior (si existía)
            ClearActiveSession();

            // 4. Establecemos el nuevo estado
            activeAlbumId = album.AlbumId;
            activeVariantId = variantId;

            Debug.Log($"[SessionManager] Iniciando sesión: {album.DisplayTitle} ({variantId})");

            // 5. Notificamos al resto de la app (Presentación, UI, etc.)
            EventBus<AlbumSessionStartedEvent>.Publish(new AlbumSessionStartedEvent(album, variantId));
        }

        public void ClearActiveSession()
        {
            if (string.IsNullOrEmpty(activeAlbumId)) return;

            activeAlbumId = null;
            activeVariantId = null;
            
            // Notificamos que la sesión terminó
            EventBus<AlbumSessionEndedEvent>.Publish(new AlbumSessionEndedEvent());
        }
        
        // Método para inyectar el catálogo una vez cargado
        public void Initialize(CatalogManifest catalog) => _currentCatalog = catalog;
    }
}