namespace TianWen.DAL
{
    /// <summary>
    /// The controls a CMOS body can carry, named rather than numbered.
    /// </summary>
    /// <remarks>
    /// <b>These take implicit sequential values, so inserting a member renumbers every member after
    /// it.</b> That is a binary break, and a SILENT one: a binding built against the old shape still
    /// loads and then reads every later control as its neighbour, asking for a fan and setting a
    /// cooler target. <see cref="WB_G"/> was added in 3.0 for exactly that reason, a major being the
    /// only vehicle in which the enum may be re-ordered. Nothing persists these as numbers (they are
    /// switch and dictionary keys throughout, never serialised), which is what makes a coordinated
    /// renumbering safe at all; if that ever stops being true, this enum has to be pinned.
    /// </remarks>
    public enum CMOSControlType
    {
        Gain = 1,
        Exposure,
        Gamma,
        WB_R,

        /// <summary>
        /// Green channel of the white balance, on a body that has one. Ask
        /// <see cref="ICMOSNativeInterface.HasThreeChannelWhiteBalance"/> before writing it: a ZWO
        /// body balances red and blue against an implicit green and has no such control.
        /// </summary>
        WB_G,
        WB_B,
        Brightness,
        BandwidthOverload,
        Overclock,
        TemperatureDeci,// return 10*temperature
        Flip,
        AutoMaxGain,
        AutoMaxExposure,
        AutoMaxBrightness,
        HardwareBin,
        HighSpeedMode,
        CoolerPowerPercent,
        TargetTemperature,// not need *10
        CoolerOn,
        MonoBin,
        FanOn,
        PatternAdjust,
        AntiDewHeater,
        Humidity,
        EnableDDR
    }
}