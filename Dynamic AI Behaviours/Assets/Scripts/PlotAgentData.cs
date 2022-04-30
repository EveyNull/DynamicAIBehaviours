using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlotAgentData : MonoBehaviour
{
    private float timeExpired = 0.0f;
    
    [SerializeField]
    SpawnAgentsUI agentsUI;

    [SerializeField]
    DD_DataDiagram dataDiagram;

    List<GameObject> lines;

    private void Start()
    {
        lines = new List<GameObject>();
        lines.Add(dataDiagram.AddLine("Mood", Color.green));
        lines.Add(dataDiagram.AddLine("Relationships", Color.blue));
        lines.Add(dataDiagram.AddLine("Health", Color.red));
    }

    public void ShowHide()
    {
        GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
    }

    void Update()
    {
        if (Time.deltaTime > 0.0f && agentsUI.agents.Count > 0)
        {
            dataDiagram.InputPoint(lines[0], new Vector2(Time.deltaTime, averageMood()));
            dataDiagram.InputPoint(lines[1], new Vector2(Time.deltaTime, averageRelationship()));
            dataDiagram.InputPoint(lines[2], new Vector2(Time.deltaTime, averageHealth()));
        }
    }

    float averageMood()
    { 
        return agentsUI.agents.Average<Agent>(
            agent => agent.GetMood()
                );
    }

    float averageRelationship()
    {
        return agentsUI.agents.Average<Agent>(
            // Return zero for this agent if its relationships is empty or hasn't been instantiated yet
            agent => 
                agent.relationships?.Count > 0 ? 
                (agent.relationships.Average<KeyValuePair<Agent, float>>(
                    agentRelationship => agentRelationship.Value
                    ))
                    :
                    0
            );
    }

    float averageHealth()
    {
        return agentsUI.agents.Average<Agent>(
            agent => agent.GetHealth()
            );
    }
}
