using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManager : MonoBehaviour
{
    public int startPopulation;
    public float generationTimer = 30f;
    public float width;
    public float height;
    public float foodNutritionMax;
    public GameObject foodPrefab;
    public Transform foodFolder;
    public GameObject blobPrefab;
    public Transform blobFolder;
    public static SimManager Instance;



    [SerializeField]
    private int foodAmount;
    public List<GameObject> allFood;
    private float timer;
    public List<BlobClass> blobs = new List<BlobClass>();

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartGeneration();
    }

    void StartGeneration()
    {
        allFood = SpawnFood(foodAmount);
        SpawnBlobs(startPopulation);
        timer = generationTimer;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            EndGeneration();
        }
    }

    static void EndGeneration()
    {
        // List<BlobStats> survivors = CollectSurvivorStats();
        // SpawnNextGeneration(survivors);
    }

    List<GameObject> SpawnFood(int amount)
    {
        List<GameObject> foods = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            float randX = Random.Range(-width, width);
            float randY = Random.Range(-height, height);
            GameObject food_instance = Instantiate(foodPrefab, new Vector2(randX, randY), Quaternion.identity, foodFolder);
            food_instance.name = $"food_{i}";

            FoodStats stats = food_instance.GetComponent<FoodStats>();
            if (stats != null)
            {
                stats.nutrition = Random.Range(1f, foodNutritionMax);
                Vector3 scale = food_instance.transform.localScale;
                scale.x *= 2 * stats.nutrition;
                scale.y *= 2 * stats.nutrition;
                food_instance.transform.localScale = scale;
                foods.Add(food_instance);
            }
        }
        return foods;
    }


    void SpawnBlobs(int sp)
    {
        for (int i = 0; i < sp; i++)
        {
            float randX = Random.Range(-width, width);
            float randY = Random.Range(-height, height);
            GameObject blob_instance = Instantiate(blobPrefab, new Vector2(randX, randY), Quaternion.identity, blobFolder);
            blob_instance.name = $"Blob_{i}";

            BlobClass blobClass = blob_instance.GetComponent<BlobClass>();
            if (blobClass != null)
                blobs.Add(blobClass);
        }
    }
}
