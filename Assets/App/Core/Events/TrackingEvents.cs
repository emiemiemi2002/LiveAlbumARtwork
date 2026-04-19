namespace LiveAlbumARtwork.App.Core.Events
{
    /// <summary>
    /// Evento emitido cuando Vuforia detecta un target en la nube.
    /// </summary>
    public class TargetDetectedEvent : IEvent
    {
        public string TargetName { get; }

        public TargetDetectedEvent(string targetName)
        {
            TargetName = targetName;
        }
    }

    /// <summary>
    /// Evento emitido cuando Vuforia pierde el target actual.
    /// </summary>
    public class TargetLostEvent : IEvent
    {
        public string TargetName { get; }

        public TargetLostEvent(string targetName)
        {
            TargetName = targetName;
        }
    }
}