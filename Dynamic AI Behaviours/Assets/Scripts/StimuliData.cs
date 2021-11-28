using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StimuliData", menuName = "ScriptableObjects/CreateStimuliData")]
public class StimuliData : ScriptableObject
{
    public static StimuliData Instance;
    public List<Stimulus> stimuli;

    private void OnEnable()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public Stimulus GetStimulusByType(StimulusType type)
    {
        foreach(Stimulus stimulus in stimuli)
        {
            if(stimulus.type == type)
            {
                return stimulus;
            }
        }
        return null;
    }
}
