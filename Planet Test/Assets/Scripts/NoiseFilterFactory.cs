using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory 
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings noiseSettings) {
        return noiseSettings.filterType switch {
            NoiseSettings.FilterType.Simple => new SimpleNoiseFilter(noiseSettings.simpleNoiseSettings),
            NoiseSettings.FilterType.Rigid => new RigidNoiseFilter(noiseSettings.rigidNoiseSettings),
            _ => null,
        };
    }
}