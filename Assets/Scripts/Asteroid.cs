using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public GameObject explosionPrefab;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            DestroyAsteroid();
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            DealDamageToEarth();
            DestroyAsteroid();
        }
    }

    void DestroyAsteroid()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void DealDamageToEarth()
    {
        World earth = FindObjectOfType<World>();
        if (earth != null)
        {
            earth.TakeDamage(earth.damagePerAsteroid);
        }
    }
}
