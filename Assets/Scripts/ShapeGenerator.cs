using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator {
    private ShapeSettings _settings;
    private INoiseFilter[] _noiseFilters;
    public MinMax elevationMinMax;

    public void UpdateSettings(ShapeSettings settings) {
        _settings = settings;
        _noiseFilters = new INoiseFilter[_settings.noiseLayers.Length];

        for (int i = 0; i < _noiseFilters.Length; i++) {
            _noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(_settings.noiseLayers[i].noiseSettings);
        }
        
        elevationMinMax = new MinMax();
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere) {
        float firstLayerValue = 0;
        float elevation = 0;

        if (_noiseFilters.Length > 0) {
            firstLayerValue = _noiseFilters[0].Evaluate(pointOnUnitSphere);

            if (_settings.noiseLayers[0].enabled) {
                elevation = firstLayerValue;
            }
        }
        
        for (int i = 1; i < _noiseFilters.Length; i++) {
            if (_settings.noiseLayers[i].enabled) {
                float mask = _settings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
                
                elevation += _noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }

        elevation = (_settings.planetRadius * (1 + elevation));
        elevationMinMax.AddValue(elevation);
        
        return pointOnUnitSphere * elevation;
    }
}