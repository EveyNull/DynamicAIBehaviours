using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAdjacencyChecker : MonoBehaviour
{
    public float checkRadius = 5.0f;
    public float checkFrequencyMin = 3.0f;
    public float checkFrequencyMax = 15.0f;

    private float checkFrequency;
    private float timeSinceLastCheck;

    public Stimulus adjacencyStimulus;

    private Agent agent;
    private void Start()
    {
        agent = GetComponent<Agent>();
        checkFrequency = NewAdjacencyCheckTime();
        timeSinceLastCheck = checkFrequency;
    }

    void Update()
    {
        if((timeSinceLastCheck += Time.deltaTime) >= checkFrequency)
        {
            
            Agent otherAgent = GetAgentNearby();
            if (otherAgent != null)
            {
                agent.ProcessStimulus(adjacencyStimulus, otherAgent);
                timeSinceLastCheck = 0.0f;
                checkFrequency = NewAdjacencyCheckTime();
            }
        }
    }

    private float NewAdjacencyCheckTime()
    {
        return Random.Range(checkFrequencyMin, checkFrequencyMax);
    }

    public Agent GetAgentNearby()
    {
        Agent nearest = null;
        float nearestDistance = float.PositiveInfinity;
        Collider[] collisions = Physics.OverlapSphere(transform.position, checkRadius);
        foreach (Collider collider in collisions)
        {
            if (collider.gameObject != gameObject && collider.GetComponent<Agent>())
            {
                Agent foundAgent = collider.GetComponent<Agent>();
                if(Vector3.Distance(agent.transform.position, foundAgent.transform.position) < nearestDistance)
                {
                    nearest = foundAgent;
                }
            }
        }
        return nearest;
    }

    public List<Agent> GetAllNearbyAgents()
    {
        List<Agent> agents = new List<Agent>();
        Collider[] collisions = Physics.OverlapSphere(transform.position, checkRadius);
        foreach (Collider collider in collisions)
        {
            if (collider.gameObject != gameObject && collider.GetComponent<Agent>())
            {
                agents.Add(collider.GetComponent<Agent>());
            }
        }
        return agents;
    }
}
