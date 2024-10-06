using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometSpawner : MonoBehaviour
{
    public GameObject comet;
    public float height;
    public float range;
    public float initialSpawnInterval = 2.0f;
    public float speedUpFactor = 0.1f;
    public float minSpawnInterval = 0.5f;
    private float currentSpawnInterval;
    // Start is called before the first frame update
    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        StartCoroutine(SpawnComets());
    }

    IEnumerator SpawnComets()
    {
        while (true)
        {
            SpawnComet();
            float randomOffset = Random.Range(-0.5f, 0.5f);
            float waitTime = currentSpawnInterval + randomOffset;
            yield return new WaitForSeconds(waitTime);
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - speedUpFactor * Time.deltaTime);
        }
    }

    void SpawnComet() 
    {
        GameObject newAsteroid = Instantiate(comet);
        Debug.Log(height);
       
        Vector3 position = new Vector3(Random.Range(-range, range), height, Random.Range(-range, range));

        position = new Vector3(position.x * (position.magnitude / range), position.y * (position.magnitude / range), position.z * (position.magnitude / range)); 
        newAsteroid.transform.position = position; 
    }
}
