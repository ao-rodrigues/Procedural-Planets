using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgedNoiseFilter : INoiseFilter {
    private NoiseSettings.RidgedNoiseSettings _settings;
    private Noise _noise = new Noise();

    public RidgedNoiseFilter(NoiseSettings.RidgedNoiseSettings settings) {
        _settings = settings;
    }

    public float Evaluate(Vector3 point) {
        float noiseValue = 0;
        float frequency = _settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;

        for (int i = 0; i < _settings.numLayers; i++) {
            float v = Mathf.Pow(1 - Mathf.Abs(_noise.Evaluate(point * frequency + _settings.center)), 2);
            v *= weight;
            weight = v * Mathf.Clamp01(_settings.weightMultiplier);

            noiseValue += v * amplitude;
            frequency *= _settings.roughness;
            amplitude *= _settings.persistence;
        }

        noiseValue = noiseValue - _settings.minValue;
        return noiseValue * _settings.strength;
    }
}