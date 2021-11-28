using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentNeedsTracking : MonoBehaviour
{
    private Agent agent;
    private GameController gameController;
    private Need[] needs = new Need[3];

    public Food carriedFood = null;
    [SerializeField]
    GameObject carriedFoodObject;


    // Start is called before the first frame update
    void Start()
    {
        if (gameController == null) gameController = GameController.Instance;
        agent = GetComponent<Agent>();
        for (int i = 0; i < needs.Length; ++i)
        {
            needs[i] = new Need((Need.NeedType)i);
            needs[i].Init((Need.NeedType)i, gameController.needData.GetDecayRate(needs[i].type));
            needs[i].satisfaction = Random.Range(0.6f, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float lowestNeedVal = 1.0f;
        int lowestNeedIndex = 0;
        for (int i = 0; i < needs.Length; ++i)
        {
            Need need = needs[i];
            if (need.satisfaction <= 0.0f)
            {
                Destroy(gameObject);
                return;
            }
            need.Decay();
            if (need.satisfaction < lowestNeedVal)
            {
                lowestNeedVal = need.satisfaction;
                lowestNeedIndex = i;
            }
        }
        if (lowestNeedVal <= 0.2f)
        {
            agent.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.NEEDCRITICAL), null);
        }
        else if (lowestNeedVal <= 0.5f)
        {
            agent.ProcessStimulus(StimuliData.Instance.GetStimulusByType(StimulusType.NEEDHALF), null);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Food>())
        {
            Food food = collision.collider.GetComponent<Food>();
            if (needs[(int)food.type].satisfaction < 0.6f)
            {
                food.ConsumeFood();
                SatisfyNeed(food.type);
            }
            else if (carriedFood == null)
            {
                carriedFood = food;
                food.ConsumeFood();
                carriedFoodObject.SetActive(true);
            }
        }
    }

    public void SatisfyNeed(Need.NeedType type)
    {
        needs[(int)type].satisfaction = 1.0f;
    }

    public Need LowestNeed()
    {
        float lowestNeedVal = 1.0f;
        int lowestNeedIndex = 0;
        for (int i = 0; i < needs.Length; ++i)
        {
            Need need = needs[i];
            if (need.satisfaction < lowestNeedVal)
            {
                lowestNeedVal = need.satisfaction;
                lowestNeedIndex = i;
            }
        }
        return needs[lowestNeedIndex];
    }
}
