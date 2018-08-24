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
    [SerializeField] private float steerMomentum = 0.5f;
    [SerializeField] private float forwardMomentum = 0.0f; //this option's a bit broken right now
    [SerializeField] private float pitchSpeedLimit = 10.0f; //the maximum up/downforce you can get from pitching the ship
    [SerializeField] private bool driftForward = true;

    [Header("Ship Model")]
    [SerializeField] private GameObject shipPrefab;

    [Header("Camera Settings")]
    [SerializeField] private float camBackInit = 15.0f; //how much distance you start with
    [SerializeField] private float camBackExtra = 12.0f; //how much more you get at full speed
    [SerializeField] private float camRight = -1.0f;
    [SerializeField] private float camUp = 6.0f;
    [SerializeField] private float camRotate = 0.25f; //how much it looks up/down towards the ship
    [SerializeField] private float camSmoothing = 0.5f;
    [SerializeField] private GameObject camPrefab;
    [SerializeField] private Transform camSpot;

    private Transform ship, model, camSmooth, camSnappy;
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

    //camera controls
    private float inputAngle, cameraAngle;
    private Vector3 prevPos, prevUp;

    //HUD
    ShipHUD HUD;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Instantiate(camPrefab, transform.position, transform.rotation).GetComponentInChildren<Camera>();
        HUD = GetComponent<ShipHUD>();

        ship = Instantiate(shipPrefab, transform).transform;
        ship.localPosition = Vector3.zero;

        camSmooth = Instantiate(camSpot, transform.position, transform.rotation);
        camSnappy = Instantiate(camSpot, transform);

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
        prevPos = ship.position;
        prevUp = ship.up;
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

        inputAngle = Mathf.Atan2(Input.GetAxis("RightX"), Input.GetAxis("RightY")) * Mathf.Rad2Deg;
    }

    void HoverLogic()
    {
        Debug.DrawLine(transform.position, transform.position + newGravity.normalized * desiredHeight, Color.blue);
        //get the current surface
        RaycastHit hit;
        if (Physics.Raycast(transform.position, newGravity, out hit, castDistance))
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
        rollDegrees -= Time.fixedDeltaTime * 1.0f + (360.0f - rollDegrees) / 180.0f;
        if (rollDegrees < 0.0f) rollDegrees = 0.0f;
        model.RotateAroundLocal(Vector3.forward, (Mathf.Deg2Rad * rollDegrees * rollDir));
    }

    void RotationLogic()
    {
        if (rotatePercentage < vert)
        {
            rotatePercentage += Time.fixedDeltaTime * 3.5f;
        }
        else if (rotatePercentage > vert)
        {
            rotatePercentage -= Time.fixedDeltaTime * 3.5f;
        }

        if (leanPercentage < horz)
        {
            if (horz <= 0.0f)
            {
                leanPercentage += Time.fixedDeltaTime * 1.1f;
            }
            else
            {
                leanPercentage += Time.fixedDeltaTime * 1.5f;
                if (leanPercentage < 0.0f)
                {
                    leanPercentage += Time.fixedDeltaTime * 1.1f;
                }
            }
        }
        else if (leanPercentage > horz)
        {
            if (horz >= 0.0f)
            {
                leanPercentage -= Time.fixedDeltaTime * 1.1f;
            }
            else
            {
                leanPercentage -= Time.fixedDeltaTime * 1.5f;
                if (leanPercentage > 0.0f)
                {
                    leanPercentage -= Time.fixedDeltaTime * 1.1f;
                }
            }
        }

        ship.RotateAround(ship.forward, -prevLean);
        ship.RotateAround(ship.right, -prevRotate);

        //keep the forward momentum after turning
        float currentSpeed = Vector3.Dot(rb.velocity, ship.forward);
        Vector3 oldPos = ship.position;
        Quaternion oldRot = ship.rotation;

        ship.position = rb.velocity;
        ship.RotateAround(Vector3.zero, ship.up, horz * steerSpeed);
        rb.velocity = Vector3.Lerp(rb.velocity, ship.position, steerMomentum);
        ship.position = oldPos;
        ship.rotation = oldRot;

        ship.RotateAround(ship.up, horz * steerSpeed * Time.fixedDeltaTime);

        Vector3 vel = rb.velocity;
        Vector3 speedUp = Vector3.Project(vel, ship.up);
        Vector3 speedRight = Vector3.Project(vel, ship.right);
        Vector3 speedFwd = Vector3.Project(vel, ship.forward);

        speedFwd = speedFwd.normalized * Mathf.Abs(currentSpeed);
        vel = speedUp + speedRight + speedFwd;
        rb.velocity = Vector3.Lerp(rb.velocity, vel, forwardMomentum);

        Vector3 proj = ship.forward.normalized - (Vector3.Dot(ship.forward, -newGravity.normalized)) * -newGravity.normalized;
        Quaternion newRot = Quaternion.LookRotation(proj.normalized, -newGravity.normalized);
        Quaternion finalRot = Quaternion.Lerp(ship.rotation, newRot, 6.0f * Time.fixedDeltaTime);
        ship.rotation = finalRot;

        AirBrake();

        prevRotate = (Mathf.Deg2Rad * rotatePercentage) * pitchLimit;
        prevLean = (Mathf.Deg2Rad * leanPercentage) * (30.0f * (currentSpeed / speed) + 20.0f);
        ship.RotateAround(ship.right, prevRotate);
        ship.RotateAround(ship.forward, prevLean);
    }

    void CameraFollow()
    {
        cam.transform.RotateAround(prevPos, prevUp, -cameraAngle);
        float dist = inputAngle - cameraAngle;
        if (dist >= 180.0f) cameraAngle += 360.0f;
        if (dist <= -180.0f) cameraAngle -= 360.0f;
        cameraAngle = Mathf.Lerp(cameraAngle, inputAngle, 0.15f);

        float vel = rb.velocity.sqrMagnitude;
        vel /= (speed * speed);

        Vector3 diff = cam.transform.position - ship.transform.position;
        float x = diff.magnitude;
        float y = Vector3.Dot(diff, ship.up);
        int mult = 1;
        if (y < 0.0f) mult = -1;

        Vector3 newPos = transform.position - (camBackInit + vel * camBackExtra) * ship.forward + camUp * ship.up + camRight * ship.right;
        Vector3 camVel = Vector3.zero;
        camSmooth.position = Vector3.SmoothDamp(camSmooth.position, newPos, ref camVel, 0.06f);
        camVel = Vector3.zero;
        camSnappy.position = Vector3.SmoothDamp(camSnappy.position, newPos, ref camVel, 0.06f);
        cam.transform.position = Vector3.Lerp(camSnappy.position, camSmooth.position, camSmoothing);

        Quaternion oldRot = cam.transform.rotation;
        Quaternion newRot = Quaternion.LookRotation(ship.forward, ship.up);

        float angle = Mathf.Asin(y / x) * Mathf.Rad2Deg;
        angle *= camRotate * mult;

        newRot = Quaternion.Lerp(newRot, Quaternion.LookRotation(-ship.up * mult, ship.forward * mult), angle / 90.0f);
        cam.transform.rotation = Quaternion.Lerp(oldRot, newRot, 5.0f * (1.0f + vel) * Time.fixedDeltaTime);

        cam.transform.RotateAround(ship.position, prevUp, cameraAngle);
        prevPos = ship.position;
        prevUp = Vector3.Lerp(prevUp, ship.up, 0.15f);
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
        rb.AddForce(newGravity * rb.mass * (1.0f + rb.drag));

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
