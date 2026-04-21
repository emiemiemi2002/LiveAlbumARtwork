using System.Collections.Generic;
using LiveAlbumARtwork.Content.Catalog.DataModels;

namespace LiveAlbumARtwork.App.Core.Services.Catalog
{
    public class TargetAliasResolver
    {
        /// <summary>
        /// Busca en el catálogo qué álbum y qué variante corresponden al target de Vuforia.
        /// </summary>
        public bool TryResolve(CatalogManifest catalog, string targetName, out AlbumDefinition album, out string variantId)
        {
            album = null;
            variantId = null;

            if (catalog?.Albums == null) return false;

            // Recorremos el catálogo buscando el alias
            foreach (var alb in catalog.Albums)
            {
                foreach (var variantEntry in alb.Variants)
                {
                    if (variantEntry.Value.TargetAliases.Contains(targetName))
                    {
                        album = alb;
                        variantId = variantEntry.Key;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}