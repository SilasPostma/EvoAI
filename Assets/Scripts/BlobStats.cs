using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BlobStats
{
    [Header("Genes")]
    public float speed;
    public float senseRadius;
    public float size;
    public float stomachSize;
    public float turnSpeed;
    public float repThreshhold;
    public float repChance;
    public float kidCost;

    [Header("Stats")]
    public int kids = 0;
    public float metabolism;

    public float energy;

    [Header("Family")]
    public List<string> heritage = new List<string> { "God" };
    public string name;

    public BlobStats Produce(float mutationRate, MutationAmounts mutationAmounts)
    {
        BlobStats child = new()
        {
            speed = MutateValue(speed, mutationRate, mutationAmounts.speed),
            senseRadius = MutateValue(senseRadius, mutationRate, mutationAmounts.senseRadius),
            size = Mathf.Min(MutateValue(size, mutationRate, mutationAmounts.size), 3f),
            stomachSize = MutateValue(stomachSize, mutationRate, mutationAmounts.stomachSize),
            turnSpeed = MutateValue(turnSpeed, mutationRate, mutationAmounts.turnSpeed),
            repThreshhold = Mathf.Max((kidCost + 0.1f) / stomachSize, MutateValue(repThreshhold, mutationRate, mutationAmounts.repThreshhold)),
            repChance = Mathf.Min(MutateValue(repChance, mutationRate, mutationAmounts.repChance), 1f),
            kidCost = MutateValue(kidCost, mutationRate, mutationAmounts.kidCost),
            energy = this.kidCost,
            kids = 0,
            heritage = new List<string>(heritage)
        };
        child.metabolism = child.CalculateMetabolism();
        heritage.Add(name);
        child.heritage = heritage;
        return child;
    }

    private float MutateValue(float value, float mRate, float mAmount)
    {
        if (Random.value < mRate)
        {
            return Mathf.Max(0.001f, value + Random.Range(-mAmount, mAmount));
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

        float total = 0.05f * (baseCost + sizeCost + speedCost + senseCost + turnCost + stomachCost);
        return Mathf.Max(0.1f, total);
    }

}
