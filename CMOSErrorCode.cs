namespace TianWen.DAL
{
    public enum CMOSErrorCode
    {
        Success = 0,
        /// <summary>
        /// no camera connected or index value out of boundary
        /// </summary>
        InvalidIndex,
        InvalidId,
        InvalidControlType,
        /// <summary>
        /// Camera is not connected, or couldn't be opened
        /// </summary>
        CameraClosed,
        /// <summary>
        /// Failed to find the camera, maybe the camera has been removed
        /// </summary>
        CameraRemoved,
        /// <summary>
        /// cannot find the path of the file.
        /// </summary>
        InvalidPath,
        InvalidFileFormat,
        /// <summary>
        /// Wrong video format size
        /// </summary>
        InvalidSize,
        /// <summary>
        /// Unsupported image format
        /// </summary>
        InvalidImageFormat,
        /// <summary>
        /// Start position is out of boundary
        /// </summary>
        OutOfBoundary,
        Timeout,
        /// <summary>
        /// Stop capture first
        /// </summary>
        InvalidSequence,
        BufferTooSmall,
        VideoModeActive,
        ExposureInProgress,
        /// <summary>
        /// general error, eg: value is out of valid range
        /// </summary>
        GeneralError
    };
}