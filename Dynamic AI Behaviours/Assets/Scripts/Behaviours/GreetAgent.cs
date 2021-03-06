using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GreetAgent", menuName = "ScriptableObjects/Behaviours/CreateGreetAgent")]
public class GreetAgent : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        subject.GetComponent<Animator>().SetTrigger("TriggerWave");
        target.WaitForAgent(subject);
        yield return new WaitForSeconds(2.0f);
        target.IncreaseRelationship(subject);
        target.UpdateMood(1.0f);
        target.StopWaitForAgent(subject);
        target.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.GREETED), subject);
        yield return null;
    }
}
