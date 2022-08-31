using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour {

    public float health = 100f;
    public GameObject explode;

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Taking Damage new health {health}");

        if (health <= 0)
        {
            Instantiate(explode, transform.position, transform.rotation);
            Destroy(this.gameObject);
            Destroy(explode, 1);
        }

        //if(explode)
        //{
        //    explode.SetActive(false);
        //}
    }

    protected virtual void OnDestroy()
    {
        
    }

}
