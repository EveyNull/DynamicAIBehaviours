using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InsultAgent", menuName = "ScriptableObjects/Behaviours/CreateInsultAgent")]
public class InsultAgent : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        subject.GetComponent<Animator>().SetTrigger("TriggerInsult");
        target.WaitForAgent(subject);
        yield return new WaitForSeconds(5.0f);
        target.ReduceRelationship(subject);
        target.UpdateMood(-1.0f);
        target.StopWaitForAgent(subject);
        target.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.INSULTED), subject);
        yield return null;
    }
}
