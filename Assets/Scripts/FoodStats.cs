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

    private void Start()
    {
        lastSpreadTime = -Mathf.Infinity;
        maxSize = false;
        growthRateReached = Random.Range(SimManager.Instance.foodGrowthRate * 0.225f, SimManager.Instance.foodGrowthRate * 0.325f);
        maxNutrition = SimManager.Instance.foodNutritionMax;
        spreadRate = SimManager.Instance.foodSpreadRate;
        foodPrefab = SimManager.Instance.foodPrefab;
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

        if (Random.value <= spreadRate * 0.0008f)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(nutrition * 0.2f + 0.2f, 1f);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            Vector2 position = (Vector2)transform.position + offset;


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
