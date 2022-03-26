using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InsultAgent", menuName = "ScriptableObjects/Behaviours/CreateInsultAgent")]
public class InsultAgent : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        Debug.Log(subject.ToString() + " insulted " + target.ToString() + ", lowering the other's opinion of them.");
        subject.GetComponent<Animator>().SetTrigger("TriggerInsult");
        target.ReduceRelationship(subject);
        target.UpdateMood(-1.0f);
        target.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.INSULTED), subject);
        yield return new WaitForSeconds(5.0f);
        yield return null;
    }
}
