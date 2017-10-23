namespace CognitiveServicesUWP.Services
{
    /// <summary>
    /// Tracks the application application usage.
    /// </summary>
    public interface ITrackingService
    {
        /// <summary>
        /// Tracks custom events.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        void TrackEvent(string eventName);
    }
}