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
            yield return 0;
        }

        target.needs.SatisfyNeed(subject.needs.carriedFood.type);
        subject.needs.carriedFood = null;
        yield break;
    }
}
