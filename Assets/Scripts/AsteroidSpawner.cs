using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroid;
    public float height;
    public float range;
    public float initialSpawnInterval = 2.0f;
    public float speedUpFactor = 0.1f;
    public float minSpawnInterval = 0.5f;

    private float currentSpawnInterval;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        StartCoroutine(SpawnAsteroids());
    }

    IEnumerator SpawnAsteroids()
    {
        while (true)
        {
            SpawnAsteroid();
            float randomOffset = Random.Range(-0.5f, 0.5f);
            float waitTime = currentSpawnInterval + randomOffset;
            yield return new WaitForSeconds(waitTime);
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - speedUpFactor * Time.deltaTime);
        }
    }

    void SpawnAsteroid()
    {
        GameObject newAsteroid = Instantiate(asteroid);
        Debug.Log(height);
        newAsteroid.transform.position = new Vector3(Random.Range(-range, range), height, Random.Range(-range, range));
    }
}
