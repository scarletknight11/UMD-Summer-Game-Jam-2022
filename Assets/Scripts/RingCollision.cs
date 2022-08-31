using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCollision : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Hoop collected");
            Destroy(this.gameObject);
            gameObject.SetActive(false);
        }
    }


}
