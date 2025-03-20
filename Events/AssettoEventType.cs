namespace AssettoNet.Events
{
    /// <summary>
    /// Represents the types of telemetry events that can be subscribed to in Assetto Corsa.
    /// </summary>
    public enum AssettoEventType
    {
        /// <summary>
        /// Event triggered when a car completes a lap.
        /// </summary>
        Spot,

        /// <summary>
        /// Event triggered on every physics update.
        /// </summary>
        Update
    }
}
