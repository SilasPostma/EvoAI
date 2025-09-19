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

    [SerializeField] private int foodAmount;
    public int kidCost = 5;

    public List<GameObject> allFood = new List<GameObject>();
    public List<GameObject> allBlobs = new List<GameObject>();

    private float timer;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartGeneration();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 || allFood.Count == 0)
        {
            EndGeneration();
        }
    }

    void StartGeneration()
    {
        ClearAll();
        allFood = SpawnFood(foodAmount);
        allBlobs = SpawnBlobs(startPopulation);
        timer = generationTimer;
    }

    void EndGeneration()
    {
        List<BlobStats> survivorStats = CollectSurvivorStats();
        ClearAll();
        allFood = SpawnFood(foodAmount);
        allBlobs = SpawnBlobs(startPopulation, survivorStats);
        timer = generationTimer;
    }

    List<GameObject> SpawnFood(int amount)
    {
        List<GameObject> foods = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            float randX = Random.Range(-width, width);
            float randY = Random.Range(-height, height);
            GameObject food_instance = Instantiate(foodPrefab, new Vector2(randX, randY), Quaternion.identity, foodFolder);
            food_instance.name = $"Food_{i}";

            FoodStats stats = food_instance.GetComponent<FoodStats>();
            if (stats != null)
            {
                stats.nutrition = Random.Range(1f, foodNutritionMax);
                food_instance.transform.localScale *= stats.nutrition;
            }

            foods.Add(food_instance);
        }
        return foods;
    }

    List<GameObject> SpawnBlobs(int count, List<BlobStats> parents = null)
    {
        List<GameObject> blobs = new List<GameObject>();

        if (parents != null && parents.Count > 0)
        {
            for (int i = 0; i < parents.Count; i++)
            {
                float randX = Random.Range(-width, width);
                float randY = Random.Range(-height, height);
                Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

                GameObject blob_instance = Instantiate(blobPrefab, new Vector2(randX, randY), rotation, blobFolder);
                blob_instance.name = $"{parents[i].name}_{i}";


                // set stats from parent + mutation
                BlobClass blobClass = blob_instance.GetComponent<BlobClass>();
                blobClass.stats = parents[i].Produce(0.2f, 0.2f);
                blobClass.stats.name = blob_instance.name;

                blob_instance.transform.localScale *= blobClass.stats.size;


                blobs.Add(blob_instance);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                float randX = Random.Range(-width, width);
                float randY = Random.Range(-height, height);
                Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

                GameObject blob_instance = Instantiate(blobPrefab, new Vector2(randX, randY), rotation, blobFolder);
                blob_instance.name = $"Blob_{i}";

                BlobClass blobClass = blob_instance.GetComponent<BlobClass>();
                blobClass.stats.name = blob_instance.name;

                blob_instance.transform.localScale *= blobClass.stats.size;


                blobs.Add(blob_instance);
            }
        }

        return blobs;
    }

    List<BlobStats> CollectSurvivorStats()
    {
        List<BlobStats> survivorsStats = new List<BlobStats>();

        foreach (GameObject blob in allBlobs)
        {
            BlobClass blobClass = blob.GetComponent<BlobClass>();
            if (blobClass == null) continue;

            int kidsCount = Mathf.FloorToInt(blobClass.energy / kidCost);

            for (int k = 0; k < kidsCount; k++)
            {
                survivorsStats.Add(blobClass.stats);
            }
        }

        return survivorsStats;
    }

    void ClearAll()
    {
        foreach (GameObject food in allFood)
            Destroy(food);
        allFood.Clear();

        foreach (GameObject blob in allBlobs)
            Destroy(blob);
        allBlobs.Clear();
    }
}
