using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnAgentsUI : MonoBehaviour
{
    [SerializeField]
    private List<TraitSlider> traitSliders;

    [SerializeField]
    private Slider numAgentsSlider;

    [SerializeField]
    private GameObject agentPrefab;

    public List<Agent> agents { get; private set; }

    [SerializeField]
    private Transform agentParent;

    private void Start()
    {
        agents = new List<Agent>();
        UpdateTraitMax();
    }

    public void SpawnAgents()
    {
        ClearAgents();
        for(int i = 0; i < numAgentsSlider.value; ++i)
        {
            var newAgent = Instantiate(agentPrefab, GameController.Instance.agentSpawnLocation.position, Quaternion.identity, agentParent);
            agents.Add(newAgent.GetComponent<Agent>());
        }
        foreach(TraitSlider traitSlider in traitSliders)
        {
            int startIndex = Random.Range(0, agents.Count);
            for(int i = 0; i < traitSlider.slider.value; ++i)
            {
                agents[(startIndex + i) % agents.Count].personalityTraits.Add(traitSlider.trait);
            }
        }
    }

    public void ClearAgents()
    {
        foreach(Agent agent in agents)
        {
            Destroy(agent.gameObject);
        }
        agents.Clear();
    }

    public void UpdateTraitMax()
    {
        foreach(TraitSlider slider in traitSliders)
        {
            slider.slider.maxValue = numAgentsSlider.value;
            if(slider.slider.value > slider.slider.maxValue)
            {
                slider.slider.value = slider.slider.maxValue;
                slider.UpdateLabel();
            }
        }
    }

    public void ShowHideUI()
    {
        GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
    }
}
