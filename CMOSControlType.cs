namespace TianWen.DAL
{

    public enum CMOSControlType
    {
        Gain = 1,
        Exposure,
        Gamma,
        WB_R,
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
        EnableDDR,

        /// <summary>
        /// Green channel of the white balance, on a body that has one.
        /// </summary>
        /// <remarks>
        /// <b>Appended, not slotted in beside <see cref="WB_R"/> and <see cref="WB_B"/> where it
        /// reads better.</b> These members take implicit sequential values, so inserting one
        /// renumbers every member after it. That is a binary break for any binding compiled against
        /// the old shape, and a SILENT one: an old binding paired with a new DAL would still load
        /// and would then read every later control as its neighbour, setting a cooler target when
        /// asked for a fan. The ordering here has never carried meaning, so tidiness buys nothing
        /// against that.
        /// </remarks>
        WB_G
    }
}