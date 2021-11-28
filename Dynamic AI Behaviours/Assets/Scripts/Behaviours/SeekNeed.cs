using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SeekNeed", menuName = "ScriptableObjects/Behaviours/CreateSeekNeed")]
public class SeekNeed : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        Need lowestNeed = subject.needs.LowestNeed();
        float needValue = lowestNeed.satisfaction;
        FoodSource foodSource = null;

        foreach (FoodSource source in subject.sources)
        {
            if(source.sourceType == lowestNeed.type)
            {
                subject.ChooseNewDestination(source.transform.position);
                foodSource = source;
            }
        }

        while(Vector3.Distance(subject.transform.position, foodSource.transform.position) > 3.0f) 
        {
            yield return 0;
        }

        Food targetFood = null;
        while(true)
        {
            Collider[] colliders = Physics.OverlapSphere(subject.transform.position, 5.0f);
            foreach(Collider collider in colliders)
            {
                Food food = collider.GetComponent<Food>();
                if(food != null && collider.GetComponent<Food>().type == lowestNeed.type)
                {
                    subject.ChooseNewDestination(food.transform.position);
                    targetFood = food;
                    break;
                }
            }
            if(lowestNeed.satisfaction > needValue)
            {
                yield break;
            }
            yield return 0;
        }
    }
}
