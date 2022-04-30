using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReturnGreeting", menuName = "ScriptableObjects/Behaviours/CreateReturnGreeting")]
public class ReturnGreeting : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        subject.GetComponent<Animator>().SetTrigger("TriggerWave");
        target.WaitForAgent(subject);
        yield return new WaitForSeconds(2.0f);
        target.IncreaseRelationship(subject);
        target.UpdateMood(1.0f);
        target.StopWaitForAgent(subject);
        yield return null;
    }
}