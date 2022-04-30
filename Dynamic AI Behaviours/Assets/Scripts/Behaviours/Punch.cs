using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Punch", menuName = "ScriptableObjects/Behaviours/CreatePunchAgent")]
public class Punch : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        subject.GetComponent<Animator>().SetTrigger("TriggerPunch");
        target.WaitForAgent(subject);
        yield return new WaitForSeconds(1.0f);
        target.ReduceRelationship(subject);
        target.UpdateMood(-2.0f);
        target.ReduceHealth(0.3f);
        target.StopWaitForAgent(subject);
        target.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.PUNCHED), subject);
        yield return null;
    }
}