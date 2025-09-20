using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManager : MonoBehaviour
{
    [Header("Simulation Settings")]
    public int startPopulation;
    public float width;
    public float height;
    public float foodNutritionMax;
    public float foodGrowthRate = 0.1f;
    public float foodSpreadRate = 0.1f;
    [SerializeField] private int foodAmount;
    public int kidCost = 5;


    [Header("Simulation Setup")]
    public GameObject foodPrefab;
    public Transform foodFolder;
    public GameObject blobPrefab;
    public Transform blobFolder;
    public static SimManager Instance;


    [Header("Simulation Info")]

    public List<GameObject> allFood = new List<GameObject>();
    public List<GameObject> allBlobs = new List<GameObject>();

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
        GrowFood();
    }

    void StartGeneration()
    {
        ClearAll();
        allFood = SpawnFood(foodAmount);
        allBlobs = SpawnBlobs(startPopulation);
    }


    List<GameObject> SpawnFood(int amount)
    {
        List<GameObject> foods = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            float randX = Random.Range(-width, width);
            float randY = Random.Range(-height, height);
            GameObject food_instance = Instantiate(foodPrefab, new Vector2(randX, randY), Quaternion.identity, foodFolder);
            food_instance.name = $"Food";

            FoodStats food_stats = food_instance.GetComponent<FoodStats>();
            if (food_stats != null)
            {
                food_stats.nutrition = Random.Range(0.3f, 0.6f);
            }

            foods.Add(food_instance);
        }
        return foods;
    }

    void GrowFood()
    {
        foreach (GameObject food in allFood)
        {
            FoodStats food_stats = food.GetComponent<FoodStats>();
            float scale = food_stats.nutrition * 0.3f;
            food.transform.localScale = new Vector2(scale, scale);
        }
        if (Random.value <= 0.001f)
        {
            List<GameObject> foodSpawned = SpawnFood(1);
            allFood.Add(foodSpawned[0]);
        }
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
