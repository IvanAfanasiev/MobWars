using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Units", menuName = "Assets/Unit")]
public class EnemiesAsset : ScriptableObject
{
    public NetworkObject[] untis;
}
