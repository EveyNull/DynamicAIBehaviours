using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReturnGreeting", menuName = "ScriptableObjects/Behaviours/CreateReturnGreeting")]
public class ReturnGreeting : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        target.IncreaseRelationship(subject);
        yield return null;
    }
}