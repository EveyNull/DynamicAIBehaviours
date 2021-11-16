using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SourceData", menuName = "ScriptableObjects/CreateSourceData")]
public class SourceData : ScriptableObject
{
    public int spawnerMaxStock;
    public int spawnerMaxSpawned;
    public float spawnerRegenRate;
    public float spawnerReleaseRate;
}
