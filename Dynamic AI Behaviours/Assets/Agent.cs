using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    private GameController gameController;

    public NeedSource[] sources = new NeedSource[3];

    private NavMeshAgent navAgent;

    private Need[] needs = new Need[3];
    private bool seekingNeed = false;

    [SerializeField]
    private float[] needStatus = new float[3];

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
        foreach(Need need in needs)
        {
            need.Decay();
            needStatus[(int)need.type] = need.satisfaction;
            if(need.satisfaction <= 0.5f && !seekingNeed)
            {
                seekingNeed = true;
                navAgent.SetDestination(sources[(int)need.type].transform.position);
            }
            if(need.satisfaction <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
        Debug.Log(needs[2].satisfaction);
    }

    private void OnTriggerStay(Collider other)
    {
        NeedSource source = other.GetComponent<NeedSource>();
        if (other)
        {
            needs[(int)source.sourceType].satisfaction = 1.0f;
            seekingNeed = false;
        }
    }
}
