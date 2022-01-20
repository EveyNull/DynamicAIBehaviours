using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SourceData", menuName = "ScriptableObjects/CreateGoal")]
public class Goal : ScriptableObject
{
    public List<GoalBehaviour> behaviours;
    public Agent target;
}