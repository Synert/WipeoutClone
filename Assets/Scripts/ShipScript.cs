using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    private Transform ship;
    private Transform model;
    private Camera cam;

    //controls how the ship handles
    [SerializeField] private float gravityScalar = 19.8f;
    [SerializeField] private float desiredHeight = 3.0f;
    [SerializeField] private float maxForce = 20.0f;
    [SerializeField] private float castDistance = 30.0f;
    [SerializeField] private float speed = 75.0f;
    [SerializeField] private float steerSpeed = 5.0f;

    private Vector3 newGravity = new Vector3(0.0f, -1.0f, 0.0f);

    //leaning stuff while moving
    private float prevRotate = 0.0f;
    private float prevLean = 0.0f;
    private float rotatePercentage = 0.0f;
    private float leanPercentage = 0.0f;

    //do a barrel roll
    private float rollDegrees = 0.0f;
    private int rollDir = 0;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if(child.gameObject.name == "ShipContainer")
            {
                ship = child;
            }
            else if (child.gameObject.name == "ShipModel")
            {
                model = child;
            }
        }
    }

    void FixedUpdate()
    {
        float vel = rb.velocity.sqrMagnitude;
        vel /= (speed * speed);

        rb.AddForce(newGravity * rb.mass);

        //inputs
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        if (vert > 1.0f) vert = 1.0f;
        if (vert < -1.0f) vert = -1.0f;
        float accel = Input.GetAxis("Acceleration");
        float drift = Input.GetAxis("Drift");
        float stunt = Input.GetAxis("Stunt");

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
                force *= (maxForce / desiredHeight);
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
            newGravity = new Vector3(0.0f, -1.0f, 0.0f);
            newGravity *= gravityScalar;
        }

        //ship rotation shenanigans

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
            }
        }

        ship.RotateAroundLocal(ship.forward, -prevLean);
        ship.RotateAroundLocal(ship.right, -prevRotate);
        ship.RotateAroundLocal(ship.up, horz * Time.deltaTime * steerSpeed);

        Vector3 proj = ship.forward.normalized - (Vector3.Dot(ship.forward, -newGravity.normalized)) * -newGravity.normalized;
        Quaternion newRot = Quaternion.LookRotation(proj.normalized, -newGravity.normalized);
        Quaternion finalRot = Quaternion.Lerp(ship.rotation, newRot, 6.0f * Time.deltaTime);
        ship.rotation = finalRot;

        float currentSpeed = Vector3.Dot(rb.velocity, ship.forward);

        prevRotate = (Mathf.Deg2Rad * rotatePercentage) * 10.0f;
        prevLean = (Mathf.Deg2Rad * leanPercentage) * (30.0f * (currentSpeed / speed) + 20.0f);
        ship.RotateAroundLocal(ship.right, prevRotate);
        ship.RotateAroundLocal(ship.forward, prevLean);

        Vector3 newPos = transform.position - (15.0f + vel * 12.0f) * ship.forward + 6.0f * ship.up - 1.0f * ship.right;
        Vector3 camVel = Vector3.zero;
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, newPos, ref camVel, 0.06f);

        Quaternion oldRot = cam.transform.rotation;
        newRot = Quaternion.LookRotation(ship.forward, ship.up);
        cam.transform.rotation = Quaternion.Lerp(oldRot, newRot, 5.0f * (1.0f + vel) * Time.deltaTime);

        float desiredSpeed = speed * accel * 1.25f;
        currentSpeed = Vector3.Dot(rb.velocity, ship.forward);
        float accelForce = (desiredSpeed - currentSpeed);
        rb.AddForce(ship.forward * accelForce * rb.mass);

        //braking to prevent drifts
        desiredSpeed = 0.0f;
        currentSpeed = Vector3.Dot(rb.velocity, ship.right);
        accelForce = (desiredSpeed - currentSpeed);
        accelForce *= (1.0f - drift);
        rb.AddForce(ship.right * accelForce * rb.mass);

        /*Debug.DrawRay(transform.position, -ship.up * desiredHeight, Color.green);
        Debug.DrawRay(transform.position + ship.up * 5.0f, ship.forward * 5.0f, Color.red);
        Debug.Log(newGravity);
        Debug.Log(vel);*/
    }

    void Update()
    {

    }
}
