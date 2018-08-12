using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    //controls how the ship handles
    [Header("Ship Handling")]
    [SerializeField] private float desiredHeight = 5.0f;
    [SerializeField] private float maxHoverForce = 20.0f;
    [SerializeField] private float castDistance = 30.0f;
    [SerializeField] private float speed = 75.0f;
    [SerializeField] private float reverseSpeed = 75.0f;
    [SerializeField] private float acceleration = 1.0f;
    [SerializeField] private float deceleration = 0.5f;
    [SerializeField] private float airBrake = 1.0f;
    [SerializeField] private float steerSpeed = 3.0f;
    [SerializeField] private float pitchSpeedLimit = 10.0f; //the maximum up/downforce you can get from pitching the ship
    [SerializeField] private bool driftForward = true;

    [Header("Ship Model")]
    [SerializeField] private GameObject shipPrefab;

    [Header("Camera Settings")]
    [SerializeField] private float camBackInit = 15.0f; //how much distance you start with
    [SerializeField] private float camBackExtra = 12.0f; //how much more you get at full speed
    [SerializeField] private float camRight = -1.0f;
    [SerializeField] private float camUp = 6.0f;

    private Transform ship, model;
    private Camera cam;
    private GameManager g_manager;
    private Rigidbody rb;

    private Vector3 newGravity = new Vector3(0.0f, -1.0f, 0.0f);
    private float gravityScalar = 9.8f;
    private float pitchLimit;
    private int shipID;

    //leaning stuff while moving
    private float prevRotate, prevLean, rotatePercentage, leanPercentage;

    //do a barrel roll
    private float rollDegrees = 0.0f;
    private int rollDir = 0;

    //inputs
    private float horz, vert, accel, drift, stunt;

    //HUD
    ShipHUD HUD;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        HUD = GetComponent<ShipHUD>();

        ship = Instantiate(shipPrefab, transform).transform;
        ship.localPosition = Vector3.zero;

        foreach (Transform child in ship.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.name == "ShipModel")
            {
                model = child;
            }
        }

        GetComponent<ShipCustomization>().Init(model);

        pitchLimit = Mathf.Rad2Deg * Mathf.Asin(pitchSpeedLimit / speed);

        g_manager = FindObjectOfType<GameManager>();
        shipID = g_manager.RegisterShip(this);
        gravityScalar = g_manager.GetGravity();
    }

    void Update()
    {
        GetInputs();
    }

    void FixedUpdate()
    {
        HoverLogic();
        StuntLogic();
        RotationLogic();
        Acceleration();
        //AirBrake();
        CameraFollow();
    }

    void GetInputs()
    {
        horz = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");
        accel = Input.GetAxis("Acceleration");
        drift = Input.GetAxis("Drift");
        stunt = Input.GetAxis("Stunt");
    }

    void HoverLogic()
    {
        //get the current surface
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -ship.up, out hit, castDistance))
        {
            //adjust gravity to new surface
            newGravity = -hit.normal.normalized;
            newGravity *= gravityScalar;

            float currentUp = Vector3.Dot(rb.velocity, ship.up);
            float force = desiredHeight - hit.distance;

            if (hit.distance <= desiredHeight)
            {
                force *= (maxHoverForce / desiredHeight);
                if (force < 0) force *= -1.0f;
                force += 1.0f;

                if (currentUp <= 0.0f)
                {
                    force *= 2.0f;
                }
                else
                {
                    force *= 0.5f;
                }
            }
            else
            {
                if (force > 0) force *= -1.0f;

                if (currentUp <= 0.0f)
                {
                    force = 0.0f;
                }
            }

            rb.AddForce(force * -newGravity * rb.mass);
        }
        else
        {
            //reset to defaults
            newGravity = new Vector3(0.0f, -gravityScalar, 0.0f);
        }
    }

    void StuntLogic()
    {
        if (stunt > 0.0f && rollDegrees <= 0.0f)
        {
            rollDegrees = 360.0f;
            if (horz < 0.0f) rollDir = 1;
            else rollDir = -1;
        }

        model.RotateAroundLocal(Vector3.forward, -(Mathf.Deg2Rad * rollDegrees * rollDir));
        rollDegrees *= (0.99f - (360.0f - rollDegrees) / 1800.0f);
        rollDegrees -= Time.deltaTime * 1.0f + (360.0f - rollDegrees) / 180.0f;
        if (rollDegrees < 0.0f) rollDegrees = 0.0f;
        model.RotateAroundLocal(Vector3.forward, (Mathf.Deg2Rad * rollDegrees * rollDir));
    }

    void RotationLogic()
    {
        if (rotatePercentage < vert)
        {
            rotatePercentage += Time.deltaTime * 3.5f;
        }
        else if (rotatePercentage > vert)
        {
            rotatePercentage -= Time.deltaTime * 3.5f;
        }

        if (leanPercentage < horz)
        {
            if (horz <= 0.0f)
            {
                leanPercentage += Time.deltaTime * 1.1f;
            }
            else
            {
                leanPercentage += Time.deltaTime * 1.5f;
                if (leanPercentage < 0.0f)
                {
                    leanPercentage += Time.deltaTime * 1.1f;
                }
            }
        }
        else if (leanPercentage > horz)
        {
            if (horz >= 0.0f)
            {
                leanPercentage -= Time.deltaTime * 1.1f;
            }
            else
            {
                leanPercentage -= Time.deltaTime * 1.5f;
                if (leanPercentage > 0.0f)
                {
                    leanPercentage -= Time.deltaTime * 1.1f;
                }
            }
        }

        ship.RotateAroundLocal(ship.forward, -prevLean);
        ship.RotateAroundLocal(ship.right, -prevRotate);

        float currentSpeed = Vector3.Dot(rb.velocity, ship.forward);

        ship.RotateAroundLocal(ship.up, horz * Time.deltaTime * steerSpeed);

        Vector3 proj = ship.forward.normalized - (Vector3.Dot(ship.forward, -newGravity.normalized)) * -newGravity.normalized;
        Quaternion newRot = Quaternion.LookRotation(proj.normalized, -newGravity.normalized);
        Quaternion finalRot = Quaternion.Lerp(ship.rotation, newRot, 6.0f * Time.deltaTime);
        ship.rotation = finalRot;

        AirBrake();

        prevRotate = (Mathf.Deg2Rad * rotatePercentage) * pitchLimit;
        prevLean = (Mathf.Deg2Rad * leanPercentage) * (30.0f * (currentSpeed / speed) + 20.0f);
        ship.RotateAroundLocal(ship.right, prevRotate);
        ship.RotateAroundLocal(ship.forward, prevLean);
    }

    void CameraFollow()
    {
        float vel = rb.velocity.sqrMagnitude;
        vel /= (speed * speed);

        Vector3 newPos = transform.position - (camBackInit + vel * camBackExtra) * ship.forward + camUp * ship.up + camRight * ship.right;
        Vector3 camVel = Vector3.zero;
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, newPos, ref camVel, 0.06f);

        Quaternion oldRot = cam.transform.rotation;
        Quaternion newRot = Quaternion.LookRotation(ship.forward, ship.up);
        cam.transform.rotation = Quaternion.Lerp(oldRot, newRot, 5.0f * (1.0f + vel) * Time.deltaTime);
    }

    void AirBrake()
    {
        //braking to prevent drifts
        float accelForce = -Vector3.Dot(rb.velocity, ship.right) * airBrake;
        accelForce *= (1.0f - drift);
        rb.AddForce(ship.right * accelForce * rb.mass);
    }

    void Acceleration()
    {
        rb.AddForce(newGravity * rb.mass);

        float currentSpeed = Vector3.Dot(rb.velocity, ship.forward);
        HUD.UpdateSpeed(currentSpeed);

        if (accel == 0.0f && drift > 0.0f && driftForward) return;
        if ((accel > 0.0f && currentSpeed > speed) || (accel < 0.0f && currentSpeed < -speed)) return; //for booster pads

        float desiredSpeed = 0.0f;
        if(accel > 0.0f) desiredSpeed = speed * accel * (1.0f + rb.drag / acceleration);
        else desiredSpeed = reverseSpeed * accel * (1.0f + rb.drag / acceleration);

        float accelForce = (desiredSpeed - currentSpeed);
        if (accelForce > 0.0f || accel < 0.0f) accelForce *= acceleration;
        else
        {
            if (drift > 0.0f && driftForward) accelForce = 0.0f;
            accelForce *= deceleration;
        }

        rb.AddForce(ship.forward * accelForce * rb.mass);
    }

    public int GetID()
    {
        return shipID;
    }

    public float GetMaxSpeed()
    {
        return speed;
    }
}
