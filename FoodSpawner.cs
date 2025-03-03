using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab; // ?a??v???n?u
    public int foodCount = 10; // ????????a???
    public float spawnAreaSize = 10f; // ?a??????????

    void Start()
    {
        SpawnFood();
    }

    void SpawnFood()
    {
        for (int i = 0; i < foodCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2),
                foodPrefab.transform.localScale.y / 2, // ???????
                Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2)
            );
            Instantiate(foodPrefab, position, Quaternion.identity);
        }
    }
}
