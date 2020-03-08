using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor {
    private Planet _planet;
    private Editor _shapeEditor;
    private Editor _colorEditor;

    public override void OnInspectorGUI() {
        using (var check = new EditorGUI.ChangeCheckScope()) {
            base.OnInspectorGUI();

            if (check.changed) {
                _planet.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet")) {
            _planet.GeneratePlanet();
        }

        DrawSettingsEditor(_planet.shapeSettings, _planet.OnShapeSettingsUpdated, ref _planet.shapeSettingsFoldout, ref _shapeEditor);
        DrawSettingsEditor(_planet.colorSettings, _planet.OnColorSettingsUpdated, ref _planet.colorSettingsFoldout, ref _colorEditor);
    }

    void DrawSettingsEditor(Object settings, Action onSettingsUpdated, ref bool foldout, ref Editor editor) {
        if(!settings) return;

        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
        
        using (var check = new EditorGUI.ChangeCheckScope()) {
            if (!foldout) return;
            
            CreateCachedEditor(settings, null, ref editor);
            editor.OnInspectorGUI();

            if (check.changed) {
                onSettingsUpdated?.Invoke();
            }
        }
    }

    public void OnEnable() {
        _planet = (Planet) target;
    }
}