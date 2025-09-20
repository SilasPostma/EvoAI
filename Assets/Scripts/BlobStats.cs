using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BlobStats
{
    [Header("Stats")]
    public float speed;
    public float senseRadius;
    public float size;
    public float stomachSize;
    public float turnSpeed;
    public float metabolism;
    public float repThreshhold;
    public float repChance;
    public int kids = 0;

    [Header("Family")]
    public List<string> heritage = new List<string> { "God" };
    public string name;

    public BlobStats Produce(float mutationRate, float mutationAmount)
    {
        BlobStats child = new BlobStats();
        child.speed = MutateValue(speed, mutationRate, mutationAmount);
        child.senseRadius = MutateValue(senseRadius, mutationRate, mutationAmount);
        child.size = Mathf.Min(MutateValue(size, mutationRate, mutationAmount), 2f);
        child.stomachSize = MutateValue(stomachSize, mutationRate, mutationAmount);
        child.turnSpeed = MutateValue(turnSpeed, mutationRate, mutationAmount);
        child.metabolism = CalculateMetabolism();
        child.repThreshhold = MutateValue(repThreshhold, mutationRate, mutationAmount);
        child.repChance = MutateValue(repChance, mutationRate, mutationAmount);
        child.kids = 0;
        child.heritage = new List<string>(heritage);
        child.heritage.Add(name);
        return child;
    }

    private float MutateValue(float value, float mr, float mp)
    {
        if (Random.value < mr)
        {
            return Mathf.Max(0.1f, value * (1 + Random.Range(-mp, mp)));
        }

        else
        {
            return value;
        }
    }

    public float CalculateMetabolism()
    {
        float baseCost = 0.5f;
        float sizeCost = Mathf.Pow(size, 0.75f) * 0.2f;
        float speedCost = Mathf.Pow(speed, 1.5f) * 0.3f;
        float senseCost = senseRadius * 0.05f;
        float turnCost = turnSpeed * 0.1f;
        float stomachCost = stomachSize * 0.05f;

        float total = 0.15f * (baseCost + sizeCost + speedCost + senseCost + turnCost + stomachCost);
        return Mathf.Max(0.1f, total);
    }

}
