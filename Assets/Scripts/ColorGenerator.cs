using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator {
    private ColorSettings _settings;
    private Texture2D _texture;
    private const int TextureResolution = 50;
    private INoiseFilter biomeNoiseFilter;

    public void UpdateSettings(ColorSettings settings){
        _settings = settings;

        if (_texture == null || _texture.height != _settings.biomeColorSettings.biomes.Length) {
            _texture = new Texture2D(TextureResolution, _settings.biomeColorSettings.biomes.Length, TextureFormat.RGBA32, false);
        }

        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    public void UpdateElevation(MinMax elevationMinMax){
        _settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere){
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - _settings.biomeColorSettings.noiseOffset) * _settings.biomeColorSettings.noiseStrength;
        
        float biomeIndex = 0;
        int numBiomes = _settings.biomeColorSettings.biomes.Length;
        float blendRange = _settings.biomeColorSettings.blendAmount / 2f + .001f;
        
        for (int i = 0; i < numBiomes; i++) {
            float dist = heightPercent - _settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dist);

            biomeIndex *= 1 - weight;
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    public void UpdateColors(){
        Color[] colors = new Color[_texture.width * _texture.height];
        int colorIndex = 0;

        foreach (var biome in _settings.biomeColorSettings.biomes) {
            for (int i = 0; i < TextureResolution; i++) {
                Color gradientColor = biome.gradient.Evaluate(i / (TextureResolution - 1f));
                Color tintColor = biome.tint;
                colors[colorIndex] = gradientColor * (1 - biome.tintPercent) + tintColor * biome.tintPercent;

                colorIndex++;
            }
        }

        _texture.SetPixels(colors);
        _texture.Apply();
        _settings.planetMaterial.SetTexture("_texture", _texture);
    }
}