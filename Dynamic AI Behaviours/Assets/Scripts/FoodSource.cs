using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSource : MonoBehaviour
{
    public Need.NeedType sourceType;

    private void OnTriggerEnter(Collider other)
    {
        AgentNeedsTracking agentNeeds = other.GetComponent<AgentNeedsTracking>();
        if (agentNeeds)
        {
            agentNeeds.GiveFood();
        }
    }
}
