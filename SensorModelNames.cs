using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TianWen.DAL;

/// <summary>
/// Maps camera product names to sensor die model names (e.g. "ZWO ASI533MC Pro" → "IMX533").
/// Camera SDKs only expose product names; the sensor die model is not available from native
/// SDKs and must be maintained here. New entries should be added as new camera models are released.
/// </summary>
public static class SensorModelNames
{
    /// <summary>
    /// Lookup keyed by normalised camera model substring (e.g. "asi533" → "IMX533").
    /// Normalisation: lowercase alphanumeric only.
    /// </summary>
    private static readonly Dictionary<string, string> _nameToSensor = new()
    {
        // ZWO / ASI
        ["asi120"] = "AR0130",
        ["asi174"] = "IMX174",
        ["asi178"] = "IMX178",
        ["asi183"] = "IMX183",
        ["asi224"] = "IMX224",
        ["asi2600"] = "IMX571",
        ["asi290"] = "IMX290",
        ["asi294"] = "IMX492",
        ["asi385"] = "IMX385",
        ["asi432"] = "IMX432",
        ["asi462"] = "IMX462",
        ["asi482"] = "IMX482",
        ["asi533"] = "IMX533",
        ["asi585"] = "IMX585",
        ["asi6200"] = "IMX455",
        ["asi662"] = "IMX662",
        ["asi664"] = "IMX664",
        ["asi676"] = "IMX676",
        ["asi678"] = "IMX678",
        ["asi715"] = "IMX715",
        ["asi220"] = "IMX662", // ASI220 shares IMX662
        ["asi2400"] = "IMX410",
        // QHY (add as models are confirmed)
    };

    /// <summary>
    /// Extracts a sensor die model name from a camera product name string.
    /// Returns null if the product name is unrecognised.
    /// </summary>
    public static bool TryGetSensorModel(string productName, [NotNullWhen(true)] out string? sensorModel)
    {
        sensorModel = null;
        if (string.IsNullOrWhiteSpace(productName)) return false;

        var normalised = Normalise(productName);

        foreach (var (key, model) in _nameToSensor)
        {
            if (normalised.Contains(key))
            {
                sensorModel = model;
                return true;
            }
        }

        return false;
    }

    private static string Normalise(string s)
    {
        Span<char> buffer = stackalloc char[s.Length];
        var w = 0;
        foreach (var c in s)
        {
            if (char.IsAsciiLetterOrDigit(c))
                buffer[w++] = char.ToLowerInvariant(c);
        }
        return new string(buffer[..w]);
    }
}
