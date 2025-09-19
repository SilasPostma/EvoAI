using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobClass : MonoBehaviour
{
    [Header("Stats")]
    public BlobStats stats;
    public float energy;

    [Header("Wandering Settings")]
    private float noiseOffset;
    private float noiseScale = 0.5f;

    private void Start()
    {
        noiseOffset = Random.value * 100f;
        stats.metabolism = 1 / (5 - stats.speed);
    }

    private void Update()
    {
        DrainEnergy();
        CheckAlive();
        HandleMovement();
        WrapPosition();
        CheckFoodNearby();
    }

    #region Energy & Death
    private void DrainEnergy()
    {
        energy -= Time.deltaTime * stats.metabolism;
    }

    private void CheckAlive()
    {
        if (energy <= 0)
        {
            SimManager.Instance.blobs.Remove(this);
            Destroy(gameObject);
        }
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        GameObject targetFood = FindClosestFood();

        if (targetFood != null)
        {
            MoveToward(targetFood.transform.position);
        }
        else
        {
            MoveRandom();
        }
    }

    private GameObject FindClosestFood()
    {
        GameObject closestFood = null;
        float closestDist = Mathf.Infinity;

        foreach (GameObject food in SimManager.Instance.allFood)
        {
            float dist = Vector3.Distance(transform.position, food.transform.position);

            if (dist < 0.5f)
            {
                FoodStats foodStats = food.GetComponent<FoodStats>();
                if (foodStats != null)
                    Eat(foodStats.nutrition, food);
                break;
            }
            else if (dist < stats.senseRadius && dist < closestDist)
            {
                closestDist = dist;
                closestFood = food;
            }
        }

        return closestFood;
    }

    private void MoveToward(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 50 * stats.turnSpeed * Time.deltaTime);

        transform.Translate(Vector3.up * stats.speed * Time.deltaTime);
    }

    private void MoveRandom()
    {
        float noise = Mathf.PerlinNoise(Time.time * noiseScale, noiseOffset) * 2f - 1f;
        transform.Rotate(Vector3.forward, noise * stats.turnSpeed * 40f * Time.deltaTime);
        transform.Translate(Vector3.up * stats.speed * Time.deltaTime);
    }

    private void WrapPosition()
    {
        float width = SimManager.Instance.width;
        float height = SimManager.Instance.height;

        Vector3 pos = transform.position;

        if (pos.x < -width) pos.x = width;
        else if (pos.x > width) pos.x = -width;

        if (pos.y < -height) pos.y = height;
        else if (pos.y > height) pos.y = -height;

        transform.position = pos;
    }
    #endregion

    #region Eating
    private void CheckFoodNearby()
    {
        // Already handled in FindClosestFood and MoveToward
    }

    public void Eat(float nutrition, GameObject food)
    {
        energy = Mathf.Clamp(energy + nutrition, 0f, stats.stomachSize);
        Destroy(food);
        SimManager.Instance.allFood.Remove(food);
    }
    #endregion
}