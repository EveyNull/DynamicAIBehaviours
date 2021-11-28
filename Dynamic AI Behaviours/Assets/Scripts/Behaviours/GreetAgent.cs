using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GreetAgent", menuName = "ScriptableObjects/Behaviours/CreateGreetAgent")]
public class GreetAgent : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        target.IncreaseRelationship(subject);
        target.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.GREETED), subject);
        yield return null;
    }
}
