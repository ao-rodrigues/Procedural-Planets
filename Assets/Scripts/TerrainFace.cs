using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace {
    private ShapeGenerator _shapeGenerator;
    private Mesh _mesh;
    private int _resolution;
    private Vector3 _localUp;
    private Vector3 _xAxis;
    private Vector3 _yAxis;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp){
        _shapeGenerator = shapeGenerator;
        _mesh = mesh;
        _resolution = resolution;
        _localUp = localUp;

        _xAxis = new Vector3(_localUp.y, _localUp.z, _localUp.x);
        _yAxis = Vector3.Cross(localUp, _xAxis);
    }

    public void ConstructMesh(){
        Vector3[] vertices = new Vector3[_resolution * _resolution];
        int[] triangles = new int[(_resolution - 1) * (_resolution - 1) * 6];
        int triangleIndex = 0;
        Vector2[] uv = _mesh.uv;

        for (int y = 0; y < _resolution; y++) {
            for (int x = 0; x < _resolution; x++) {
                Vector2 percent = new Vector2(x, y) / (_resolution - 1);
                Vector3 pointOnUnitCube = _localUp + (percent.x - .5f) * 2 * _xAxis + (percent.y - .5f) * 2 * _yAxis;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                int i = x + y * _resolution;
                vertices[i] = _shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if (x != _resolution - 1 && y != _resolution - 1) {
                    triangles[triangleIndex] = i;
                    triangles[triangleIndex + 1] = i + _resolution + 1;
                    triangles[triangleIndex + 2] = i + _resolution;

                    triangles[triangleIndex + 3] = i;
                    triangles[triangleIndex + 4] = i + 1;
                    triangles[triangleIndex + 5] = i + _resolution + 1;

                    triangleIndex += 6;
                }
            }
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();

        if (_mesh.uv.Length == uv.Length) {
            _mesh.uv = uv;
        }
    }

    public void UpdateUVs(ColorGenerator colorGenerator){
        Vector2[] uv = new Vector2[_resolution * _resolution];

        for (int y = 0; y < _resolution; y++) {
            for (int x = 0; x < _resolution; x++) {
                Vector2 percent = new Vector2(x, y) / (_resolution - 1);
                Vector3 pointOnUnitCube = _localUp + (percent.x - .5f) * 2 * _xAxis + (percent.y - .5f) * 2 * _yAxis;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                int i = x + y * _resolution;
                uv[i] = new Vector2(colorGenerator.BiomePercentFromPoint(pointOnUnitSphere), 0);
            }
        }

        _mesh.uv = uv;
    }
}