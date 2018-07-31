using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour {

    public Transform ShipModel;
    public Camera cam;
    private Vector3 newGravity = new Vector3(0.0f, -1.0f, 0.0f);
    private float gravityScalar = 9.8f;
    private float desiredHeight = 3.0f;
    private float maxForce = 10.0f;
    private float castDistance = 30.0f;

    // Use this for initialization
    void Start () {
		
	}

    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(newGravity * GetComponent<Rigidbody>().mass);
    }

    // Update is called once per frame
    void Update () {

        RaycastHit hit;
        if(Physics.Raycast(transform.position, newGravity, out hit, castDistance))
        {
            Debug.DrawRay(transform.position, newGravity.normalized * castDistance, Color.blue);

            //adjust gravity to new surface
            newGravity = -hit.normal.normalized;
            newGravity *= gravityScalar;

            float currentUp = GetComponent<Rigidbody>().velocity.y;
            currentUp = Vector3.Dot(GetComponent<Rigidbody>().velocity, ShipModel.up);

            float force = desiredHeight - hit.distance;

            if (hit.distance <= desiredHeight)
            {
                force *= (maxForce / desiredHeight);
                if (force < 0) force *= -1.0f;
                force += 1.0f;

                if (currentUp <= 0.0f)
                {
                    force *= force;
                }
                else
                {
                    force = Mathf.Sqrt(force);
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

            GetComponent<Rigidbody>().AddForce(force * -newGravity * GetComponent<Rigidbody>().mass);
        }
        else
        {
            //reset to defaults
            newGravity = new Vector3(0.0f, -1.0f, 0.0f);
            newGravity *= gravityScalar;
        }

        Vector3 proj = ShipModel.forward - (Vector3.Dot(ShipModel.forward, -newGravity)) * -newGravity;
        Quaternion newRot = Quaternion.LookRotation(proj, -newGravity);
        ShipModel.rotation = Quaternion.Lerp(ShipModel.rotation, newRot, 1.5f * Time.deltaTime);
        GetComponent<Rigidbody>().AddForce(ShipModel.forward * 3.0f * GetComponent<Rigidbody>().mass);

        Vector3 newPos = transform.position - 15.0f * ShipModel.forward + 6.0f * ShipModel.up - 1.0f * ShipModel.right;
        Vector3 camVel = Vector3.zero;
        cam.transform.position = Vector3.SmoothDamp(cam.transform.position, newPos, ref camVel, 0.12f);

        Quaternion oldRot = cam.transform.rotation;
        newRot = Quaternion.LookRotation(ShipModel.forward, ShipModel.up);
        cam.transform.rotation = Quaternion.Lerp(oldRot, newRot, 1.0f * Time.deltaTime);

        Debug.DrawRay(transform.position, newGravity.normalized * desiredHeight, Color.green);
        Debug.Log(newGravity);
    }
}
