namespace Rocket.Libraries.ConsulHelper.Configuration
{
    /// <summary>
    /// Enum for supported time measurement units.
    /// </summary>
    public enum TimeUnit : int
    {
        /// <summary>
        /// Second. Will be translated to 's' when used in configs.
        /// </summary>
        Seconds = 1,

        /// <summary>
        /// Minutes. Will be translated to 'm' when used in configs.
        /// </summary>
        Minutes = 2,
    }
}