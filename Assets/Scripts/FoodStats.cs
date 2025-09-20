using UnityEngine;

public class FoodStats : MonoBehaviour
{
    [Header("Stats")]

    public float nutrition;
    private float growthRateReached;
    private bool maxSize;
    private float maxNutrition;
    private float spreadRate;

    [Header("Spread Settings")]
    private float lastSpreadTime;
    private float spreadCooldown = 2f;

    private GameObject foodPrefab;
    private float simScaleMult;

    private void Start()
    {
        lastSpreadTime = -Mathf.Infinity;
        maxSize = false;
        growthRateReached = Random.Range(SimManager.Instance.foodGrowthRate * 0.225f, SimManager.Instance.foodGrowthRate * 0.325f);
        maxNutrition = SimManager.Instance.foodNutritionMax;
        spreadRate = SimManager.Instance.foodSpreadRate;
        foodPrefab = SimManager.Instance.foodPrefab;
        simScaleMult = SimManager.Instance.simScaleMult;

    }

    void Update()
    {
        Grow();
        Spread();
    }

    public void Grow()
    {
        maxSize = nutrition >= maxNutrition;
        if (!maxSize)
        {
            nutrition += Time.deltaTime * growthRateReached;
        }
    }

    #region Spread
    public void Spread()
    {
        if (Time.time - lastSpreadTime < spreadCooldown)
            return;

        if (Random.value <= spreadRate * 0.00015f * simScaleMult)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(nutrition * 0.2f + 0.2f, simScaleMult);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            Vector2 position = (Vector2)transform.position + offset;

            float width = SimManager.Instance.width;
            float height = SimManager.Instance.height;

            float minX = -width;
            float maxX = width;
            float minY = -height;
            float maxY = height;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            GameObject food_instance = Instantiate(
                foodPrefab,
                position,
                Quaternion.identity,
                SimManager.Instance.foodFolder
            );

            food_instance.name = $"Food";

            FoodStats food_stats = food_instance.GetComponent<FoodStats>();
            if (food_stats != null)
            {
                food_stats.nutrition = Random.Range(0.3f, 0.6f);
            }

            SimManager.Instance.allFood.Add(food_instance);

            lastSpreadTime = Time.time;
        }
    }
    #endregion

}
