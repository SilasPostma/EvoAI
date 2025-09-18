using UnityEngine;

[System.Serializable]
public class BlobStats
{
    public float speed;
    public float senseRadius;
    public float size;
    public float stomachSize;
    public float turnSpeed;
    public float metabolism;


    public BlobStats Produce(int mutationRate, float mutationAmount)
    {
        BlobStats child = new BlobStats();
        child.speed = MutateValue(speed, mutationRate, mutationAmount);
        child.metabolism = 1 / (5 - child.speed);
        child.senseRadius = MutateValue(senseRadius, mutationRate, mutationAmount);
        child.size = MutateValue(size, mutationRate, mutationAmount);
        child.stomachSize = MutateValue(stomachSize, mutationRate, mutationAmount);
        child.turnSpeed = MutateValue(turnSpeed, mutationRate, mutationAmount);
        return child;
    }

    private float MutateValue(float value, int mr, float ma)
    {
        if (mr >= Random.Range(0, 100))
        {
            return value + Random.Range(-ma, ma);
        }

        else
        {
            return value;
        }
    }
}
