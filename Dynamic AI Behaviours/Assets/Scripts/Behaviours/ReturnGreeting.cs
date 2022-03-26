using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReturnGreeting", menuName = "ScriptableObjects/Behaviours/CreateReturnGreeting")]
public class ReturnGreeting : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        Debug.Log(subject.ToString() + " said hello to " + target.ToString() + ", raising the other's opinion of them.");
        subject.GetComponent<Animator>().SetTrigger("TriggerWave");
        target.IncreaseRelationship(subject);
        target.UpdateMood(1.0f);
        yield return new WaitForSeconds(2.0f);
        yield return null;
    }
}