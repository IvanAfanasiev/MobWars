using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Defence", menuName = "Assets/Defence")]
public class DefenceAsset : ScriptableObject
{
    public NetworkObject[] defence;
}
