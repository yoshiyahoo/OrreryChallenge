using UnityEngine;

public class AsteroidBelt : MonoBehaviour
{
    public int numberOfAsteroids = 1000;
    public float innerRadius = 2.2f;
    public float outerRadius = 3.2f;
    public float height = 0.1f;
    public GameObject asteroidPrefab;

    private GameObject[] asteroids;

    void Start()
    {
        CreateAsteroidBelt();
    }

    public void CreateAsteroidBelt()
    {
        if (asteroids != null)
        {
            foreach (var asteroid in asteroids)
            {
                Destroy(asteroid);
            }
        }

        asteroids = new GameObject[numberOfAsteroids];

        for (int i = 0; i < numberOfAsteroids; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2);
            float distance = Random.Range(innerRadius, outerRadius);
            float height = Random.Range(-this.height / 2, this.height / 2);

            Vector3 position = new Vector3(
                Mathf.Cos(angle) * distance,
                height,
                Mathf.Sin(angle) * distance
            );

            asteroids[i] = Instantiate(asteroidPrefab, position, Quaternion.identity, transform);
            asteroids[i].transform.localScale = Vector3.one * Random.Range(0.01f, 0.05f);
        }
    }

    public void UpdateScale(float scaleFactor)
    {
        transform.localScale = Vector3.one * scaleFactor;
    }
}