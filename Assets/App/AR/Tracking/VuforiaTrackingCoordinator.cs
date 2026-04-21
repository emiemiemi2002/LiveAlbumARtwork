using UnityEngine;
using Vuforia;
using LiveAlbumARtwork.App.Core.Events;

namespace LiveAlbumARtwork.App.AR.Tracking
{
    /// <summary>
    /// Actúa como el puente único entre Vuforia Engine y el resto de la aplicación.
    /// Su única responsabilidad es notificar cuando se encuentra un target.
    /// </summary>
    [RequireComponent(typeof(CloudRecoBehaviour))]
    public class VuforiaTrackingCoordinator : MonoBehaviour
    {
        private CloudRecoBehaviour _cloudRecoBehaviour;

        void Awake()
        {
            // Obtenemos la referencia al componente de Vuforia
            _cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
            
            // Registramos los callbacks nativos de Vuforia
            _cloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
            _cloudRecoBehaviour.RegisterOnNewSearchResultEventHandler(OnNewSearchResult);
        }

        void OnDestroy()
        {
            // Nos desuscribimos de los eventos nativos de Vuforia al destruir el objeto para evitar NullReferenceExceptions.
            if (_cloudRecoBehaviour != null)
            {
                _cloudRecoBehaviour.UnregisterOnInitializedEventHandler(OnInitialized);
                _cloudRecoBehaviour.UnregisterOnNewSearchResultEventHandler(OnNewSearchResult);
            }
        }

        private void OnInitialized(CloudRecoBehaviour crb)
        {
            Debug.Log("[VuforiaTrackingCoordinator] Cloud Reco inicializado y listo para escanear.");
        }

        private void OnNewSearchResult(CloudRecoBehaviour.CloudRecoSearchResult result)
        {
            Debug.Log($"[VuforiaTrackingCoordinator] Target detectado en la nube: {result.TargetName}");

            // 1. Detenemos temporalmente el escaneo para evitar que Vuforia dispare 
            // este evento 60 veces por segundo mientras el target siga en cámara.
            _cloudRecoBehaviour.enabled = false;

            // 2. Publicamos el evento. No sabemos quién lo escuchará (UI, SessionManager), y no nos importa.
            EventBus<TargetDetectedEvent>.Publish(new TargetDetectedEvent(result.TargetName));
        }

        /// <summary>
        /// Método público para reanudar el escaneo. Será llamado por el Session Manager
        /// o por la UI cuando el usuario presione el botón de "Escanear otro álbum".
        /// </summary>
        public void ResumeScanning()
        {
            if (!_cloudRecoBehaviour.enabled)
            {
                _cloudRecoBehaviour.enabled = true;
                Debug.Log("[VuforiaTrackingCoordinator] Escaneo reanudado.");
            }
        }
    }
}