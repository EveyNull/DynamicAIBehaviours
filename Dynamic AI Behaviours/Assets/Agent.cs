using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    private GameController gameController;

    public FoodSource[] sources = new FoodSource[3];

    private NavMeshAgent navAgent;
    private GameObject currentNavTarget;

    private Need[] needs = new Need[3];

    [SerializeField]
    private float[] needStatus = new float[3];

    [SerializeField]
    private float wanderRadius = 20.0f;

    Food carriedFood = null;

    [SerializeField]
    GameObject carriedFoodObject;

    // Start is called before the first frame update
    void Start()
    {
        if(gameController == null) gameController = GameController.Instance;
        navAgent = GetComponent<NavMeshAgent>();
        for(int i = 0; i < needs.Length; ++i)
        {
            needs[i] = new Need((Need.NeedType)i);
            needs[i].Init((Need.NeedType)i, gameController.needData.GetDecayRate(needs[i].type));
        }
    }

    private void Update()
    {
        float lowestNeedVal = 1.0f;
        int lowestNeedIndex = 0;
        for(int i = 0; i < needs.Length; ++i)
        {
            Need need = needs[i];
            if (need.satisfaction <= 0.0f)
            {
                Destroy(gameObject);
                return;
            }
            need.Decay();
            needStatus[(int)need.type] = need.satisfaction;
            if(need.satisfaction < lowestNeedVal)
            {
                lowestNeedVal = need.satisfaction;
                lowestNeedIndex = i;
            }
        }
        if(lowestNeedVal <= 0.5f)
        {
            SeekNeed(lowestNeedIndex);
        }
        else
        {
            Wander();
        }
    }

    public void SatisfyNeed(Need.NeedType type)
    {
        needs[(int)type].satisfaction = 1.0f;
        currentNavTarget = null;
    }

    private void SeekNeed(int needIndex)
    {
        if(carriedFood != null)
        {
            if((int)carriedFood.type == needIndex)
            {
                SatisfyNeed(carriedFood.type);
                carriedFood = null;
                carriedFoodObject.SetActive(false);
                return;
            }
        }

        Collider[] nearby = Physics.OverlapSphere(transform.position, 5.0f);
        foreach(Collider other in nearby)
        {
            if(other.GetComponent<Food>())
            {
                if((int)other.GetComponent<Food>().type == needIndex)
                {
                    navAgent.SetDestination(other.transform.position);
                    currentNavTarget = other.gameObject;
                    return;
                }
            }
        }

        if (navAgent.destination != sources[needIndex].transform.position)
        {
            navAgent.SetDestination(sources[needIndex].transform.position);
            currentNavTarget = sources[needIndex].gameObject;
            return;
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
            navAgent.SetDestination(finalPosition);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<Food>())
        {
            Food food = collision.collider.GetComponent<Food>();
            if (needs[(int)food.type].satisfaction < 0.6f)
            {
                food.ConsumeFood();
                SatisfyNeed(food.type);
            }
            else if(carriedFood == null)
            {
                carriedFood = food;
                food.ConsumeFood();
                carriedFoodObject.SetActive(true);
            }
        }
    }
}
