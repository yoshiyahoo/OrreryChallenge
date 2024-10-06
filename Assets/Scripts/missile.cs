using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missile : MonoBehaviour
{
    public float moveSpeed;
    public short killDistance;
    Vector3 distanceTraveled;
    // Start is called before the first frame update
    void Start()
    {
        distanceTraveled = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = this.transform.up * moveSpeed * Time.deltaTime;
        this.transform.position += moveDirection;       
        distanceTraveled += moveDirection;
        if (distanceTraveled.magnitude > killDistance) 
        {
            // destoryes the whole thing!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Destroy(gameObject);
        }
    }
}
