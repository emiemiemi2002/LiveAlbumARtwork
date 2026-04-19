using System;

namespace LiveAlbumARtwork.App.Core.Events
{
    /// <summary>
    /// Sistema de mensajería global para desacoplar sistemas.
    /// </summary>
    /// <typeparam name="T">El tipo de evento específico que implementa IEvent</typeparam>
    public static class EventBus<T> where T : IEvent
    {
        // El delegado que mantiene la lista de suscriptores
        private static event Action<T> OnEvent;

        /// <summary>
        /// Suscribe un método para que sea llamado cuando ocurra este evento.
        /// </summary>
        public static void Subscribe(Action<T> handler)
        {
            OnEvent += handler;
        }

        /// <summary>
        /// Elimina la suscripción de un método. Crucial para evitar memory leaks en Unity.
        /// </summary>
        public static void Unsubscribe(Action<T> handler)
        {
            OnEvent -= handler;
        }

        /// <summary>
        /// Emite el evento a todos los suscriptores activos.
        /// </summary>
        public static void Publish(T eventToPublish)
        {
            OnEvent?.Invoke(eventToPublish);
        }
    }
}