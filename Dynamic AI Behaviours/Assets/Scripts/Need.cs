using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Need
{
    public enum NeedType
    {
        RED = 0,
        GREEN = 1,
        BLUE = 2,
        COUNT
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
        satisfaction = 1.0f;
        decayPerSecond = decayRate;
    }

    public void Decay()
    {
        satisfaction -= decayPerSecond * Time.deltaTime;
    }
}
