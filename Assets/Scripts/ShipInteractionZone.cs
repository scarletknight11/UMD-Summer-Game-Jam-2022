using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInteractionZone : MonoBehaviour
{
    [SerializeField]
    private Spaceship spaceship;

    private ZeroGMovement player;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            player = other.gameObject.GetComponentInParent<ZeroGMovement>();
            if (player != null)
                player.AssignShip(spaceship);
            //Debug.Log("Player");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (player != null)
                player.RemoveShip();
        }
    }
}
