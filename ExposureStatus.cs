namespace TianWen.DAL
{
    public enum ExposureStatus
    {
        /// <summary>
        /// idle states, you can start exposure now
        /// </summary>
        Idle = 0,
        /// <summary>
        /// Exposing
        /// </summary>
        Working,
        /// <summary>
        /// exposure finished and waiting for download
        /// </summary>
        Success,
        /// <summary>
        /// exposure failed, you need to start exposure again
        /// </summary>
        Failed,
    };
}