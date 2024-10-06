using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : MonoBehaviour
{
    public float cometSpeed;
    Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {
        movement = new Vector3(Random.Range(-cometSpeed, cometSpeed), 0, Random.Range(-cometSpeed, cometSpeed));
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += movement;   
    }
     
    void onCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    } 
}
