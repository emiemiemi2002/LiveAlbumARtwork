using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LiveAlbumARtwork.App.Core.Services.Assets
{
    public class AssetProviderService
    {
        // Diccionario para rastrear los handles y evitar fugas de memoria al liberar assets
        private readonly Dictionary<string, AsyncOperationHandle> _addressableHandles = new Dictionary<string, AsyncOperationHandle>();

        /// <summary>
        /// Carga cualquier tipo de asset de forma asíncrona mediante su Key de Addressables.
        /// </summary>
        public async Task<T> LoadAssetAsync<T>(string key) where T : Object
        {
            if (string.IsNullOrEmpty(key)) return null;

            // Si ya está cargado, devolvemos el resultado existente
            if (_addressableHandles.TryGetValue(key, out var existingHandle))
            {
                return existingHandle.Result as T;
            }

            // Iniciamos la carga asíncrona
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            _addressableHandles.Add(key, handle);

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }

            Debug.LogError($"[AssetProviderService] Error al cargar asset: {key}");
            _addressableHandles.Remove(key);
            return null;
        }

        /// <summary>
        /// Libera la memoria de un asset específico cuando ya no se necesita (ej. al limpiar la escena).
        /// </summary>
        public void ReleaseAsset(string key)
        {
            if (_addressableHandles.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _addressableHandles.Remove(key);
                Debug.Log($"[AssetProviderService] Asset liberado: {key}");
            }
        }

        /// <summary>
        /// Libera todos los assets cargados. Ideal para cambios de escena mayores.
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var handle in _addressableHandles.Values)
            {
                Addressables.Release(handle);
            }
            _addressableHandles.Clear();
            Debug.Log("[AssetProviderService] Todos los assets han sido liberados.");
        }
    }
}