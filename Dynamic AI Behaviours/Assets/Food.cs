using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public Need.NeedType type;

    private FoodSource spawner;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.GetComponent<Food>())
        {
            Vector3 pos = transform.position;
            pos.x += Random.Range(-0.5f, 0.5f);
            pos.z += Random.Range(-0.5f, 0.5f);

            transform.position = pos;
            return;
        }
    }

    public void SetFoodSource(FoodSource owner)
    {
        spawner = owner;
    }
    
    public void ConsumeFood()
    {
        spawner.DespawnFood();
        gameObject.SetActive(false);
    }
}
