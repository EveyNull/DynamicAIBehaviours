using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSource : MonoBehaviour
{
    public Need.NeedType sourceType;

    [SerializeField]
    private GameObject foodPrefab;

    private List<Food> foodPool;

    private int foodStock;
    private int foodStockMax;

    private int maxSpawnedFood;
    private int spawnedFood = 0;

    private float foodRegenerate;
    private float timeSinceLastRegenerate;

    private float feedCooldown;
    private float timeSinceLastFeed = 0f;

    private void Start()
    {
        SourceData data = GameController.Instance.sourceData;

        foodStockMax = data.spawnerMaxStock;
        maxSpawnedFood = data.spawnerMaxSpawned;
        foodRegenerate = data.spawnerRegenRate;
        feedCooldown = data.spawnerReleaseRate;

        timeSinceLastRegenerate = foodRegenerate;
        foodStock = foodStockMax;

        foodPool = new List<Food>();
        for (int i = 0; i < maxSpawnedFood; ++i)
        {
            GameObject spawnedFood = Instantiate(foodPrefab);
            foodPool.Add(spawnedFood.GetComponent<Food>());
            spawnedFood.transform.SetParent(transform);
            spawnedFood.SetActive(false);
        }
    }

    private void Update()
    {
        // If it has been enough time to spawn another food 'pellet', and there is enough stock, do so
        if((timeSinceLastFeed += Time.deltaTime) >= feedCooldown && foodStock > 0 && spawnedFood < maxSpawnedFood)
        {
            Food food = null;
            foreach(Food itr in foodPool)
            {
                if(itr.gameObject.activeInHierarchy == false)
                {
                    food = itr;
                    break;
                }
            }
            if (food != null)
            {
                food.gameObject.SetActive(true);
                food.transform.position = transform.position;
                food.SetFoodSource(this);
                food.type = sourceType;
                spawnedFood++;
                timeSinceLastFeed = 0.0f;
                foodStock--;
            }
        }

        // If it has been enough time to generate new stock, and it does not already hold the maximum amount, do so
        if((timeSinceLastRegenerate += Time.deltaTime) >= foodRegenerate && foodStock < foodStockMax)
        {
            foodStock++;
            timeSinceLastRegenerate = 0.0f;
        }
    }

    public void DespawnFood()
    {
        spawnedFood--;
    }
}
