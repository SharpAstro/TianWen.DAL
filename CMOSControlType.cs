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
        EnableDDR
    }
}