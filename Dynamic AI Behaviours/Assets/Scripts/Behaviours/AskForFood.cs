using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AskForFood", menuName = "ScriptableObjects/Behaviours/CreateAskForFood")]
public class AskForFood : GoalBehaviour
{
    public override IEnumerator ProcessBehaviour(Agent subject, Agent target)
    {
        List<Agent> agents = subject.adjacencyChecker.GetAllNearbyAgents();

        foreach (Agent agent in agents)
        {
            if (agent.needs.carriedFood && agent.needs.carriedFood)
            {
                Debug.Log("Agent " + subject + " is asking " + agent + " for food!");
                subject.ChooseNewDestination(agent.transform.position);

                agent.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.ASKEDFORFOOD), subject);
                yield return new WaitForEndOfFrame();
                if (subject.IsWaitingFor(agent))
                {
                    while (subject.IsWaitingFor(agent))
                    {
                        if (agent.gameObject.activeInHierarchy == false || subject.needs.carriedFood == true)
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
    }

}
