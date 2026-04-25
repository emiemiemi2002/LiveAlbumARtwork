using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// Se usa un namespace (espacio de nombres) para aislar la lógica del resto de Unity
namespace LiveAlbumARtwork.Content.Catalog.DataModels
{
    /// <summary>
    /// Contenedor principal que envuelve toda la base de datos de álbumes.
    /// </summary>
    [Serializable]
    public class CatalogManifest
    {
        [JsonProperty("Albums")]
        public List<AlbumDefinition> Albums { get; set; }
    }

    /// <summary>
    /// Registro canónico y único de cada álbum.
    /// </summary>
    [Serializable]
    public class AlbumDefinition
    {
        [JsonProperty("AlbumId")]
        public string AlbumId { get; set; }

        [JsonProperty("DisplayTitle")]
        public string DisplayTitle { get; set; }

        [JsonProperty("Artist")]
        public string Artist { get; set; }

        [JsonProperty("ReleaseDate")]
        public string ReleaseDate { get; set; }

        [JsonProperty("Labels")]
        public List<string> Labels { get; set; }

        [JsonProperty("Producers")]
        public List<string> Producers { get; set; }

        [JsonProperty("InterestingFacts")]
        public List<string> InterestingFacts { get; set; }

        [JsonProperty("AudioPreviewPath")]
        public string AudioPreviewPath { get; set; }

        // Referente al Patrón de Variantes (Edición estándar, 50th aniversario, etc.)
        [JsonProperty("Variants")]
        public Dictionary<string, AlbumVariant> Variants { get; set; }

        // Secuencia temporal pre-calculada en segundos
        [JsonProperty("TrackSequence")]
        public List<TrackSequence> TrackSequence { get; set; }
    }

    /// <summary>
    /// Define los assets visuales y los targets de Vuforia específicos para una versión del álbum.
    /// Soporta múltiples fuentes de video para permitir alternar estados visuales.
    /// </summary>
    [Serializable]
    public class AlbumVariant
    {
        [JsonProperty("TargetAliases")]
        public List<string> TargetAliases { get; set; }

        [JsonProperty("CoverAssetId")]
        public string CoverAssetId { get; set; }

        [JsonProperty("DiscTextureAssetId")]
        public string DiscTextureAssetId { get; set; }

        // Video 1: Material audiovisual general (videos musicales, videos promocionales, BTS, etc.)
        [JsonProperty("VideoAssetId")] 
        public string VideoAssetId { get; set; }

        // Video 2 (Opcional): Versión animada de la portada para reproducción en loop
        [JsonProperty("AnimatedCoverAssetId")] 
        public string AnimatedCoverAssetId { get; set; }

        // --- AUDIO OVERRIDES ---
        [JsonProperty("AudioPreviewOverride")]
        public string AudioPreviewOverride { get; set; }

        [JsonProperty("TrackSequenceOverride")]
        public List<TrackSequence> TrackSequenceOverride { get; set; }
    }

    /// <summary>
    /// Define los tiempos de inicio y fin de cada track en el audio principal.
    /// </summary>
    [Serializable]
    public class TrackSequence
    {
        [JsonProperty("TrackName")]
        public string TrackName { get; set; }

        [JsonProperty("StartTimeSec")]
        public float StartTimeSec { get; set; }

        [JsonProperty("EndTimeSec")]
        public float EndTimeSec { get; set; }
    }
}
