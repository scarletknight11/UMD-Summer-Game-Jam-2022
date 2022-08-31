using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour {
   
    Rigidbody rb;
    public float initalForce = 10f;
    public float initalTorque = 10f;

    private void Awake()
    {
        float randomScale = Random.Range(1f, 3f);
        transform.localScale = Vector3.one * randomScale;
        
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.AddForce(Vector3.forward * initalForce);
        rb.AddTorque(Vector3.forward * initalTorque);
    }
}
