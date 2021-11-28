using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StimulusType
{
    ADJACENCY = 0,
    GREETED = 1,
    INSULTED = 2,
    NEEDHALF = 3,
    NEEDCRITICAL = 4,
    ASKEDFORFOOD,
    COUNT
}

[CreateAssetMenu(fileName = "NewStimulus", menuName = "ScriptableObjects/CreateStimulus")]
public class Stimulus : ScriptableObject
{
    public StimulusType type;
    public bool overrideCurrentGoal = false;
    public List<Goal> potentialResponses;
}
