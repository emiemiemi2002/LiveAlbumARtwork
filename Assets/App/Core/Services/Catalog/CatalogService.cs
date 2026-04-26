using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using LiveAlbumARtwork.Content.Catalog.DataModels;

namespace LiveAlbumARtwork.App.Core.Services.Catalog
{
    public class CatalogService
    {
        public CatalogManifest Manifest { get; private set; }

        /// <summary>
        /// Carga y deserializa el manifiesto del catálogo de forma asíncrona.
        /// </summary>
        public async Task LoadCatalogAsync(string jsonContent)
        {
            if (string.IsNullOrEmpty(jsonContent))
            {
                Debug.LogError("[CatalogService] El contenido del JSON está vacío.");
                return;
            }

            // Deserialización en un hilo secundario para no bloquear el frame principal
            await Task.Run(() => 
            {
                try 
                {
                    Manifest = JsonConvert.DeserializeObject<CatalogManifest>(jsonContent);
                    Debug.Log("[CatalogService] Catálogo deserializado exitosamente.");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[CatalogService] Error al deserializar JSON: {e.Message}");
                }
            });
        }
        
        /// <summary>
        /// Busca un álbum específico por su ID canónico.
        /// </summary>
        public AlbumDefinition GetAlbum(string albumId)
        {
            return Manifest?.Albums?.Find(a => a.AlbumId == albumId);
        }
    }
}