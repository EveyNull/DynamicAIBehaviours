using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InsultAgent", menuName = "ScriptableObjects/Behaviours/CreateInsultAgent")]
public class InsultAgent : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        target.ReduceRelationship(subject);
        target.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.INSULTED), subject);
        yield return null;
    }
}
