using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Agent : MonoBehaviour
{
    private GameController gameController;

    public AgentNeedsTracking needs;
    public AgentAdjacencyChecker adjacencyChecker;
    public List<PersonalityTrait> personalityTraits;

    public StimuliData stimuliData;

    public FoodSource[] sources = new FoodSource[3];

    public List<GoalBehaviour> currentBehaviours;
    private Coroutine goalRoutine = null;
    private Dictionary<Stimulus, float> recentStimuli;

    private NavMeshAgent navAgent;
    private NavMeshObstacle selfObstacle;
    private GameObject currentNavTarget;
    public Dictionary<Agent, float> relationships;
    public List<float> relationshipsPureValues;

    [SerializeField]
    private float wanderRadius = 20.0f;

    private List<Agent> waitingForOthers;

    public GameObject happyThought;
    public GameObject sadThought;

    // Start is called before the first frame update
    void Start()
    {
        if(gameController == null) gameController = GameController.Instance;
        needs = GetComponent<AgentNeedsTracking>();
        adjacencyChecker = GetComponent<AgentAdjacencyChecker>();
        navAgent = GetComponent<NavMeshAgent>();
        selfObstacle = GetComponent<NavMeshObstacle>();
        relationships = new Dictionary<Agent, float>();
        recentStimuli = new Dictionary<Stimulus, float>();
        waitingForOthers = new List<Agent>();
    }

    private void Update()
    {
        List<Stimulus> toRemove = new List<Stimulus>();
        List<KeyValuePair<Stimulus, float>> toUpdate = new List<KeyValuePair<Stimulus, float>>();
        foreach(var entry in recentStimuli)
        {
            float timer = entry.Value;
            if((timer -= Time.deltaTime) <= 0.0f)
            {
                toRemove.Add(entry.Key);
            }
            else
            {
                toUpdate.Add(new KeyValuePair<Stimulus, float>(entry.Key, timer));
            }
        }
        foreach(Stimulus entry in toRemove)
        {
            recentStimuli.Remove(entry);
        }
        foreach(var entry in toUpdate)
        {
            recentStimuli[entry.Key] = entry.Value;
        }
        
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
        if (recentStimuli.ContainsKey(stimulus)) return;
        recentStimuli.Add(stimulus, 5.0f);
        if (stimulus.potentialResponses.Count > 0)
        {
            GoalBehaviour newGoal = stimulus.GetBehaviour(this, other);
            if (newGoal == null) return;
            newGoal.target = other;

            if (currentBehaviours.Find(x => x == newGoal)) return;

            if (currentBehaviours.Count == 0 || stimulus.overrideCurrentGoal)
            {
                currentBehaviours.Add(newGoal);
                goalRoutine = StartCoroutine(AchieveGoal(newGoal, newGoal.target));
            }
            else
            {
                currentBehaviours.Insert(0, newGoal);
            }
        }
    }

    private IEnumerator AchieveGoal(GoalBehaviour behaviour, Agent target)
    {
        yield return new WaitForSeconds(1.0f);
        yield return behaviour.ProcessBehaviour(this, target);

        currentBehaviours.RemoveAt(currentBehaviours.Count-1);

        if(currentBehaviours.Count > 0)
        {
            GoalBehaviour newGoal = currentBehaviours[currentBehaviours.Count - 1];
            goalRoutine = StartCoroutine(AchieveGoal(newGoal, newGoal.target));
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
            relationships.Add(other, 0f);
        }
        relationships[other] += 0.2f;
        relationshipsPureValues = relationships.Values.ToList();
        StartCoroutine(ShowMood(true));
    }

    public void ReduceRelationship(Agent other)
    {
        if (relationships.ContainsKey(other) == false)
        {
            relationships.Add(other, 0f);
        }
        relationships[other] -= 0.2f;
        relationshipsPureValues = relationships.Values.ToList();
        StartCoroutine(ShowMood(false));
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

    public float GetMood()
    {
        float count = 0;
        foreach(Need need in needs.needs)
        {
            count += need.satisfaction;
        }
        return (count / needs.needs.Length) - 0.5f;
    }

    public IEnumerator ShowMood(bool good)
    {
        GameObject thoughtBubble = good ? happyThought : sadThought;

        thoughtBubble.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        thoughtBubble.SetActive(false);
    }
}
