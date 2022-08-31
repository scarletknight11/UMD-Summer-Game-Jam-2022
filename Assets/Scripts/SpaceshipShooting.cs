using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipShooting : MonoBehaviour {

    [Header("=== Spaceship Settings")]
    [SerializeField] private Spaceship spaceship;

    //Hardpoint Settings
    [SerializeField]
    private Transform[] hardpoints;
    [SerializeField]
    private Transform hardpointmiddle;
    [SerializeField]
    private LayerMask shootableMask;
    [SerializeField]
    private float hardpointRange = 100f;

    private bool targetInRange = false;

    //Laser Settings
    [SerializeField]
    private LineRenderer[] lasers;
    [SerializeField]
    private ParticleSystem laserHitParticles;
    [SerializeField]
    private float miningPower = 1f;
    [SerializeField]
    private float timeBetweenDamageApplication = 0.25f;
    private float currentTimeBetweenDamageApplication;
    [SerializeField]
    private float laserHeatThreshold = 2f;
    [SerializeField]
    private float laserHeatRate = 0.25f;
    [SerializeField]
    private float laserCoolRate = 0.5f;

    private float currentLaserHeat = 0f;
    private bool overHeated = false;

    private bool firing = false;

    private Camera cam;

    public float CurrentLaserHeat
    {
        get { return currentLaserHeat; }
    }

    public float LaserHeatThreshold
    {
        get { return laserHeatThreshold; }
    }

    private void Awake()
    {
        cam = Camera.main;
        spaceship = GetComponent<Spaceship>();
    }

    private void FixedUpdate()
    {
        if (spaceship.IsOccupied)
        {
            //Handle laser Firing
            HandleLaserString();
        }
    }

    private void HandleLaserString()
    {
        if (firing && !overHeated)
        {
            FireLaser();
        } 
        else
        {
            foreach(var laser in lasers)
            {
                laser.gameObject.SetActive(false);
            }

            CoolLaser();
        }
    }

    void ApplyDamage(HealthComponent healthComponent)
    {
        currentTimeBetweenDamageApplication += Time.deltaTime;
        if (currentTimeBetweenDamageApplication > timeBetweenDamageApplication)
        {
            currentTimeBetweenDamageApplication = 0f;
            Debug.Log("Applying Damage to: " + healthComponent.gameObject.name);
            healthComponent.TakeDamage(miningPower);
        }
    }

    void FireLaser()
    {
        RaycastHit hitinfo;

        if (TargetInfo.IsTargetInRange(hardpointmiddle.transform.position, hardpointmiddle.transform.forward, out hitinfo, hardpointRange, shootableMask))
        {
            if (hitinfo.collider.GetComponent<HealthComponent>())
            
                ApplyDamage(hitinfo.collider.GetComponent<HealthComponent>());
            
            //Instantiate our laser hit particle
            Instantiate(laserHitParticles, hitinfo.point, Quaternion.LookRotation(hitinfo.normal));

            foreach(var laser in lasers)
            {
                Vector3 localHitPosition = laser.transform.InverseTransformPoint(hitinfo.point);
                laser.gameObject.SetActive(true);
                laser.SetPosition(1, localHitPosition);
            }
        }
        else
        {
            foreach(var laser in lasers)
            {
                laser.gameObject.SetActive(true);
                laser.SetPosition(1, new Vector3(0,0, hardpointRange));
            }
        }
        HeatLaser();
    }


    void HeatLaser()
    {
        if(firing && currentLaserHeat < laserHeatThreshold)
        {
            currentLaserHeat += laserHeatRate * Time.deltaTime;

            if(currentLaserHeat >= laserHeatThreshold)
            {
                overHeated = true;
                firing = false;
            }
        }
    }

    void CoolLaser()
    {
        // Cool Down Laser
        if (overHeated)
        {
            if (currentLaserHeat / laserHeatThreshold <= 0.5f)
            {
                overHeated = false;
            }
        }
        
        if (currentLaserHeat > 0f)
        {
            currentLaserHeat -= laserCoolRate * Time.deltaTime;
        }
    }

    #region Input
    public void OnFire(InputAction.CallbackContext context)
    {
        firing = context.performed;
    }
    #endregion
}
