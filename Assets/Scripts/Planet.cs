using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Planet : MonoBehaviour {
    [Range(2, 256)] 
    public int resolution = 10;

    public bool autoUpdate = true;

    public enum FaceRenderMask {
        All,
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back
    }

    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    
    [HideInInspector]
    public bool colorSettingsFoldout;

    private ShapeGenerator _shapeGenerator;

    [SerializeField, HideInInspector] private MeshFilter[] meshFilters;
    private TerrainFace[] _terrainFaces;
    
    void Initialize() {
        _shapeGenerator = new ShapeGenerator(shapeSettings);
        
        if (meshFilters == null || meshFilters.Length == 0) {
            meshFilters = new MeshFilter[6];
        }

        _terrainFaces = new TerrainFace[6];

        Vector3[] directions =
            {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};

        for (int i = 0; i < 6; i++) {
            if (meshFilters[i] == null) {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));

                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            _terrainFaces[i] = new TerrainFace(_shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int) faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    public void GeneratePlanet() {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated() {
        if (!autoUpdate) return;
        
        Initialize();
        GenerateMesh();
    }

    public void OnColorSettingsUpdated() {
        if(!autoUpdate) return;

        Initialize();
        GenerateColors();
    }

    void GenerateMesh() {
        for (int i = 0; i < 6; i++) {
            if (meshFilters[i].gameObject.activeSelf) {
                _terrainFaces[i].ConstructMesh();
            }
        }
    }

    void GenerateColors() {
        foreach (var meshFilter in meshFilters) {
            meshFilter.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColor;
        }
    }
}