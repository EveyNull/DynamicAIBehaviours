using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AskForFood", menuName = "ScriptableObjects/Behaviours/CreateAskForFood")]
public class AskForFood : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        Need lowest = subject.needs.LowestNeed();
        float startSatisfaction = lowest.satisfaction;
        Food targetFood = null;

        Collider[] colliders = Physics.OverlapSphere(subject.transform.position, 5.0f);
        foreach (Collider collider in colliders)
        {
            Food food = collider.GetComponent<Food>();
            if (food != null && collider.GetComponent<Food>().type == lowest.type)
            {
                subject.ChooseNewDestination(food.transform.position);
                targetFood = food;
                break;
            }
        }

        if(targetFood != null)
        {
            while(lowest.satisfaction <= startSatisfaction && targetFood.gameObject.activeInHierarchy == true)
            {
                yield return 0;
            }
            yield break;
        }

        List<Agent> agents = subject.adjacencyChecker.GetAllNearbyAgents();

        foreach(Agent agent in agents)
        {
            if(agent.needs.carriedFood && agent.needs.carriedFood.type == lowest.type)
            {
                subject.ChooseNewDestination(agent.transform.position);

                agent.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.ASKEDFORFOOD), subject);
                yield return new WaitForEndOfFrame();
                if (subject.IsWaitingFor(agent))
                {
                    while (subject.IsWaitingFor(agent))
                    {
                        if (agent.gameObject.activeInHierarchy == false || lowest.satisfaction > startSatisfaction)
                        {
                            subject.StopWaitForAgent(agent);
                            yield break;
                        }
                        yield return 0;
                    }
                }
                else continue;

            }
        }
        yield break;
    }

}
