using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeedValues
{
    public Need.NeedType type;
    public float decayRate;
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CreateNeedData")]
public class NeedData : ScriptableObject
{
    [SerializeField]
    private NeedValues[] needvalues = new NeedValues[3];

    public float GetDecayRate(Need.NeedType type)
    {
        NeedValues values = needvalues[(int)type];
        if(values.type != type)
        {
            throw new System.Exception("Incorrectly ordered need data, ensure all entries are in the order defined by enum");
        }

        return values.decayRate;
    }
}
