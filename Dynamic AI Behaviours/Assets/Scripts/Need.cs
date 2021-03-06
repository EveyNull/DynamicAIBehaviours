using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Need
{
    public enum NeedType
    {
        FOOD = 0,
    }

    public NeedType type;
    private float decayPerSecond = 0.1f;
    public float satisfaction;

    public Need(NeedType needType)
    {
        type = needType;
    }

    public void Init(NeedType need, float decayRate)
    {
        type = need;
        satisfaction = Random.Range(0.6f, 1.0f);
        decayPerSecond = decayRate;
    }

    public void Decay()
    {
        satisfaction -= decayPerSecond * Time.deltaTime;
    }
}
