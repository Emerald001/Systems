using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor shapeEditor;
    Editor colorEditor;

    public override void OnInspectorGUI() {
        using (EditorGUI.ChangeCheckScope check = new()) {
            base.OnInspectorGUI();
            if (check.changed)
                planet.GeneratePlanet();
        }

        if (GUILayout.Button("Generate Planet"))
            planet.GeneratePlanet();

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdated, ref planet.colorSettingsFoldout, ref colorEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action OnSettingsUpdated, ref bool foldOut, ref Editor editor) {
        if (settings == null)
            return;

        foldOut = EditorGUILayout.InspectorTitlebar(foldOut, settings);
        using (EditorGUI.ChangeCheckScope check = new()) {

            if (!foldOut)
                return;

            CreateCachedEditor(settings, null, ref editor);
            editor.OnInspectorGUI();

            if (check.changed) {
                if (OnSettingsUpdated != null)
                    OnSettingsUpdated();
            }
        }
    }

    private void OnEnable() {
        planet = (Planet)target;
    }
}