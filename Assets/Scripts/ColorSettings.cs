using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class ColorSettings : ScriptableObject {
    public Gradient gradient;
    public Material planetMaterial;
}