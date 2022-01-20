using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    private GameController gameController;

    public AgentNeedsTracking needs;
    public AgentAdjacencyChecker adjacencyChecker;

    public StimuliData stimuliData;

    public FoodSource[] sources = new FoodSource[3];

    public List<Goal> currentGoals;
    private Coroutine goalRoutine = null;

    private NavMeshAgent navAgent;
    private NavMeshObstacle selfObstacle;
    private GameObject currentNavTarget;
    private Dictionary<Agent, float> relationships;

    [SerializeField]
    private float wanderRadius = 20.0f;

    private List<Agent> waitingForOthers;

    // Start is called before the first frame update
    void Start()
    {
        if(gameController == null) gameController = GameController.Instance;
        needs = GetComponent<AgentNeedsTracking>();
        adjacencyChecker = GetComponent<AgentAdjacencyChecker>();
        navAgent = GetComponent<NavMeshAgent>();
        selfObstacle = GetComponent<NavMeshObstacle>();
        relationships = new Dictionary<Agent, float>();
        waitingForOthers = new List<Agent>();
    }

    private void Update()
    {
        if (goalRoutine == null)
        {
            Wander();
        }
    }

    private void Wander()
    {
        if (navAgent.remainingDistance < 0.2f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
            Vector3 finalPosition = hit.position;
            ChooseNewDestination(finalPosition);
        }
    }

    public void ChooseNewDestination(Vector3 destination)
    {
        selfObstacle.enabled = false;
        navAgent.SetDestination(destination);
        selfObstacle.enabled = true;

    }

    public void ProcessStimulus(Stimulus stimulus, Agent other)
    {
        if (stimulus.potentialResponses.Count > 0)
        {
            Goal newGoal = stimulus.potentialResponses[Random.Range(0, stimulus.potentialResponses.Count)];
            newGoal.target = other;

            if (currentGoals.Find(x => x.behaviours == newGoal.behaviours)) return;

            if (currentGoals.Count == 0 || stimulus.overrideCurrentGoal)
            {
                currentGoals.Add(newGoal);
                goalRoutine = StartCoroutine(AchieveGoal(newGoal.behaviours, newGoal.target));
            }
            else
            {
                currentGoals.Insert(0, newGoal);
            }
        }
    }

    private IEnumerator AchieveGoal(List<GoalBehaviour> behaviours, Agent target)
    {
        foreach(GoalBehaviour behaviour in behaviours)
        {
            yield return behaviour.ProcessBehaviour(this, target);
        }
        currentGoals.RemoveAt(currentGoals.Count-1);
        if(currentGoals.Count > 0)
        {
            Goal newGoal = currentGoals[currentGoals.Count - 1];
            goalRoutine = StartCoroutine(AchieveGoal(newGoal.behaviours, newGoal.target));
        }
        else
        {
            goalRoutine = null;
        }
    }

    public void IncreaseRelationship(Agent other)
    {
        if (relationships.ContainsKey(other) == false)
        {
            relationships.Add(other, 0.5f);
        }
        relationships[other] += 0.05f;
        Debug.Log(this + " relationship with " + other + " increased! Is now " + relationships[other]);
    }

    public void ReduceRelationship(Agent other)
    {
        if (relationships.ContainsKey(other) == false)
        {
            relationships.Add(other, 0.5f);
        }
        relationships[other] -= 0.05f;
        Debug.Log(this + " relationship with " + other + " decreased! Is now " + relationships[other]);
    }

    public void WaitForAgent(Agent agent)
    {
        if (waitingForOthers.Contains(agent) == false)
        {
            waitingForOthers.Add(agent);
        }
    }

    public void StopWaitForAgent(Agent agent)
    {
        if(waitingForOthers.Contains(agent))
        {
            waitingForOthers.Remove(agent);
        }
    }

    public bool IsWaitingFor(Agent agent)
    {
        return waitingForOthers.Contains(agent);
    }
}
