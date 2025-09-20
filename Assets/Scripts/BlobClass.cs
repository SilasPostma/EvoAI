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

    [Header("Reproduction Settings")]
    private float lastReproduceTime;
    private float reproduceCooldown = 5f;

    private void Start()
    {
        noiseOffset = Random.value * 100f;
        stats.metabolism = stats.CalculateMetabolism();
        lastReproduceTime = Time.time;
    }

    private void Update()
    {
        DrainEnergy();
        CheckAlive();
        Reproduce();
        HandleMovement();
        WrapPosition();
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
            SimManager.Instance.allBlobs.Remove(gameObject);
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
            FoodStats foodStats = food.GetComponent<FoodStats>();

            float eatDist = dist - foodStats.nutrition * 0.075f;
            if (eatDist < stats.size / 2f)
            {
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
    public void Eat(float nutrition, GameObject food)
    {
        energy = Mathf.Clamp(energy + nutrition, 0f, stats.stomachSize);
        Destroy(food);
        SimManager.Instance.allFood.Remove(food);
    }
    #endregion

    #region Reproducing
    public void Reproduce()
    {
        if (Time.time - lastReproduceTime < reproduceCooldown)
            return;

        float fullness = energy / stats.stomachSize;
        if (fullness >= stats.repThreshhold)
        {
            if (Random.value <= stats.repChance)
            {
                Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

                GameObject blob_instance = Instantiate(
                    SimManager.Instance.blobPrefab,
                    transform.position,
                    rotation,
                    SimManager.Instance.blobFolder
                );

                BlobClass blobClass = blob_instance.GetComponent<BlobClass>();
                blobClass.stats = blobClass.stats.Produce(0.2f, 0.2f);
                blobClass.stats.name = $"{stats.name}_{stats.kids}";

                blob_instance.name = blobClass.stats.name;

                blob_instance.transform.localScale *= blobClass.stats.size;

                SimManager.Instance.allBlobs.Add(blob_instance);

                energy -= SimManager.Instance.kidCost;
                stats.kids++;

                lastReproduceTime = Time.time;
            }
        }
    }
    #endregion
}