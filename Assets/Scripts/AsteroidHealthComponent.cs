using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHealthComponent : HealthComponent {

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected virtual void OnDestroy()
    {
        // Instantiate destruction particles
        // Play explosion sound
        base.OnDestroy();
    }


}
