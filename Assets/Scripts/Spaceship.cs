using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public class Spaceship : MonoBehaviour {

    [SerializeField]
    private float yawTorque = 500f;
    [SerializeField]
    private float pitchTorque = 1000f;
    [SerializeField]
    private float rollTorque = 1000f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f;
    [SerializeField]
    private float strafeThrust = 50f;

    //Boost settings
    [SerializeField]
    private float maxBoostAmount = 2f;
    [SerializeField]
    private float boostDeprecationRate = 0.25f;
    [SerializeField]
    private float boostRechargeRate = 0.5f;
    [SerializeField]
    private float boostMultiplier = 5f;
    public bool boosting = false;
    public float currentBoostAmount;

    [SerializeField]
    private CinemachineVirtualCamera shipThirdPersonCamera;
    [SerializeField]
    private CinemachineVirtualCamera shipFirstPersonCamera;


    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;
    float glide = 0f;
    float verticalGlide = 0f;
    float horizontalGlide = 0f;

    Rigidbody rb;

    // Input Values
    private float thrust1D;
    private float upDown1D;
    private float strafe1D;
    private float roll1D;
    private Vector2 pitchYaw;

    private bool isOccupied = false;

    public bool IsOccupied { get { return isOccupied; } }

    public float CurrentBoostAmount
    {
        get { return currentBoostAmount; }
    }

    public float MaxBoostAmount
    {
        get { return maxBoostAmount; }
    }

    private ZeroGMovement player;

    public delegate void OnRequestShipExit();
    public event OnRequestShipExit onRequestShipExit;


    // Start is called before the first frame update
    void Start()
    {
      rb = GetComponent<Rigidbody>();
      currentBoostAmount = maxBoostAmount;
      player = GameObject.FindGameObjectWithTag("Player").GetComponent<ZeroGMovement>();
      if(player != null)
      {
        Debug.Log("Player Found");
      }
      player.onRequestshipEntry += PlayerEnteredShip;
    }

    private void OnEnable()
    {
        if (shipThirdPersonCamera != null)
        {
            CameraSwitcher.Register(shipThirdPersonCamera);
        }
        else
        {
            Debug.LogError("Ship camera not assigned");
        }
    }

    private void OnDisable()
    {
        if (shipThirdPersonCamera != null)
        {
            CameraSwitcher.UnRegister(shipThirdPersonCamera);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isOccupied)
        {
            HandleBoosting();
            HandleMovement();
        }
    }

    void HandleBoosting()
    {
        if (boosting && currentBoostAmount > 0f)
        {
            currentBoostAmount += boostRechargeRate;
            if(currentBoostAmount <= 0f)
            {
                boosting = false;
            }
        }
        else
        {
            if (currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate;
            }
        }
    }

    void HandleMovement()
    {
        // Rolll computation
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);
        // pitch computation
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        //Yaw computation
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);
   
        // Thrust
        if (thrust1D > 0.1f || thrust1D < -0.1f)
        {
            float currentThrust;

            if (boosting)
            {
                currentThrust = thrust * boostMultiplier;
            } 
            else 
            {
                currentThrust = thrust;
            }

            rb.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.deltaTime);
            glide = thrust;
        } else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;
        }

        // UP/DOWN
        if(upDown1D > 0.1f || upDown1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.forward * upDown1D * upThrust * Time.deltaTime);
            verticalGlide = upDown1D * upThrust;
        } else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.deltaTime);
            verticalGlide *= upDownGlideReduction;
        }

        // STRAFING
        if(strafe1D > 0.1f || strafe1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.right * strafe1D * upThrust * Time.deltaTime);
            horizontalGlide = strafe1D * strafeThrust;
        } else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }
    }

    void PlayerEnteredShip()
    {
        rb.isKinematic = false;
        CameraSwitcher.SwitchCamera(shipThirdPersonCamera);
        isOccupied = true;
    }

    void PlayerExitedShip()
    {
        rb.isKinematic = true;
        isOccupied = false;
        if (onRequestShipExit != null)
            onRequestShipExit();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ring")
        {
            Debug.Log("Hoop collected");
            Destroy(other.gameObject);
            //gameObject.SetActive(false);
        }
    }
    #region Input Methods

    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>();
    }

    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }

    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>();
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>();
    }

    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.performed;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (isOccupied && context.action.triggered)
        {
            PlayerExitedShip();
        }
    }

    public void OnSwitchCamera(InputAction.CallbackContext context)
    {
        if (isOccupied && context.action.triggered)
        {
            if (CameraSwitcher.IsActiveCamera(shipFirstPersonCamera))
            {
                CameraSwitcher.SwitchCamera(shipThirdPersonCamera);
            }
            else if (CameraSwitcher.IsActiveCamera(shipThirdPersonCamera))
            {
                CameraSwitcher.SwitchCamera(shipFirstPersonCamera);
            }
        }
    }

    #endregion

}
