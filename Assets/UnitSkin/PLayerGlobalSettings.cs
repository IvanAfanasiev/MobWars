using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PLayerSettings", menuName = "Player/Settings")]
public class PLayerGlobalSettings : ScriptableObject
{
    [SerializeField] Color newColor;
    public void SetColor(Color clr)
    {
        newColor = clr;
    }
    public Color GetColor()
    {
        return newColor;
    }
}
