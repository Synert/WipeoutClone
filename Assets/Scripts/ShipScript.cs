using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour {

    public Transform ship;
    public Camera cam;
    private Vector3 newGravity = new Vector3(0.0f, -1.0f, 0.0f);
    private float gravityScalar = 19.8f;
    private float desiredHeight = 3.0f;
    private float maxForce = 10.0f;
    private float castDistance = 30.0f;
    private float speed = 75.0f;
    private float steerSpeed = 5.0f;

    private Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float vel = rb.velocity.sqrMagnitude;
        //if (vel > speed) vel = speed;
        //vel += speed;
        vel /= (speed * speed);

        rb.AddForce(newGravity * rb.mass);

        Vector3 proj = ship.forward - (Vector3.Dot(ship.forward, -newGravity)) * -newGravity;
        Quaternion newRot = Quaternion.LookRotation(proj, -newGravity);
        ship.rotation = Quaternion.Lerp(ship.rotation, newRot, 1.25f * Time.deltaTime);

        Vector3 newPos = transform.position - 17.0f * ship.forward + 6.0f * ship.up - 1.0f * ship.right;
        Vector3 camVel = Vector3.zero;
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, newPos, ref camVel, 0.08f * (1.0f - vel * 0.5f));

        Debug.Log(vel);

        Quaternion oldRot = cam.transform.rotation;
        newRot = Quaternion.LookRotation(ship.forward, ship.up);
        cam.transform.rotation = Quaternion.Lerp(oldRot, newRot, 5.0f * (1.0f + vel) * Time.deltaTime);
    }

    // Update is called once per frame
    void Update () {

        //inputs
        float horz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        float accel = Input.GetAxis("Acceleration");
        float drift = Input.GetAxis("Drift");

        RaycastHit hit;
        if(Physics.Raycast(transform.position, newGravity, out hit, castDistance))
        {
            Debug.DrawRay(transform.position, newGravity.normalized * castDistance, Color.blue);

            //adjust gravity to new surface
            newGravity = -hit.normal.normalized;
            newGravity *= gravityScalar;

            float currentUp =  Vector3.Dot(rb.velocity, ship.up);

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

                if(currentUp <= 0.0f)
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

        //apply the inputs
        ship.RotateAroundLocal(ship.up, horz * Time.deltaTime * steerSpeed);

        float desiredSpeed = speed * accel * 1.25f;
        float currentSpeed = Vector3.Dot(rb.velocity, ship.forward);
        float accelForce = (desiredSpeed - currentSpeed);
        rb.AddForce(ship.forward * accelForce * rb.mass);

        //braking to prevent drifts
        desiredSpeed = 0.0f;
        currentSpeed = Vector3.Dot(rb.velocity, ship.right);
        accelForce = (desiredSpeed - currentSpeed);
        accelForce *= (1.0f - drift);
        rb.AddForce(ship.right * accelForce * rb.mass);

        Debug.DrawRay(transform.position, newGravity.normalized * desiredHeight, Color.green);
        Debug.Log(newGravity);
    }
}
