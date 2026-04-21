using LiveAlbumARtwork.Content.Catalog.DataModels;

namespace LiveAlbumARtwork.App.Core.Events
{
    public class AlbumSessionStartedEvent : IEvent 
    {
        public AlbumDefinition Album { get; }
        public string VariantId { get; }

        public AlbumSessionStartedEvent(AlbumDefinition album, string variantId)
        {
            Album = album;
            VariantId = variantId;
        }
    }

    public class AlbumSessionEndedEvent : IEvent { }
}