using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GiveFood", menuName = "ScriptableObjects/Behaviours/CreateGiveFood")]
public class GiveFood : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        target.WaitForAgent(subject);
        subject.ChooseNewDestination(subject.transform.position);
        
        while(Vector3.Distance(subject.transform.position, target.transform.position) > 1.0f)
        {
            if(target.IsWaitingFor(subject) == false)
            {
                yield break;
            }
            yield return 0;
        }

        if (target.needs.carriedFood == true)
        {
            Debug.Log("Agent " + subject + " did not give food to " + target + " as it has food already.");
            yield break;
        }
        Debug.Log("Agent " + subject + " gave food to " + target + ", increasing the other's opinion of them.");

        target.needs.GiveFood();
        target.IncreaseRelationship(subject);
        subject.needs.RemoveFood();
        yield break;
    }
}
