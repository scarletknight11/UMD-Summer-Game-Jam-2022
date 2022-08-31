using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Image laserHeatImage;
    [SerializeField] private Image boostImage;

    [SerializeField] private SpaceshipShooting spaceshipshooting;
    [SerializeField] private Spaceship spaceship;

    private void Start()
    {
        spaceshipshooting = FindObjectOfType<SpaceshipShooting>();
        spaceship = FindObjectOfType<Spaceship>();
    }

    private void Update()
    {
        if (spaceshipshooting != null)
        {
            laserHeatImage.fillAmount = spaceshipshooting.CurrentLaserHeat / spaceshipshooting.LaserHeatThreshold;
        }

        if (spaceship != null)
        {
            boostImage.fillAmount = spaceship.currentBoostAmount / spaceship.MaxBoostAmount;
        }
    }

}
