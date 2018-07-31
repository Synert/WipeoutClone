using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipScript : MonoBehaviour {

    public Transform ShipModel;
    public Camera cam;
    private Vector3 newGravity = new Vector3(0.0f, -1.0f, 0.0f);
    private float gravityScalar = 9.8f;
    private float desiredHeight = 4.0f;
    private float maxForce = 10.0f;

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
        if(Physics.Raycast(transform.position, newGravity, out hit, 30.0f))
        {
            Debug.DrawRay(transform.position, newGravity.normalized * 30.0f, Color.blue);

            //first of all, make the ship face the track
            newGravity = -hit.normal.normalized;
            newGravity *= gravityScalar;
            //ShipModel.LookAt(ShipModel.position + ShipModel.forward - hit.normal);
            //ShipModel.rotation = Quaternion.FromToRotation(Vector3.up, -newGravity);

            float currentUp = GetComponent<Rigidbody>().velocity.y;
            currentUp = Vector3.Dot(GetComponent<Rigidbody>().velocity, ShipModel.up);
            float force = desiredHeight - hit.distance;

            force *= (maxForce / desiredHeight);

            //force = Mathf.Pow(force, 1.1f);
            //force *= force;

            force += 1.0f;
            //force *= force;

            //force *= GetComponent<Rigidbody>().mass;

            if (hit.distance <= desiredHeight)
            {
                if (force < 0) force *= -1.0f;
                if (currentUp <= 0.0f)
                {
                    force *= force;
                }
                else
                {
                    force = Mathf.Sqrt(force);
                }
            }
            else if (hit.distance > desiredHeight)
            {
                force = desiredHeight - hit.distance;

                if (force > 0) force *= -1.0f;

                //force *= 0.15f;

                if (currentUp >= 0.0f)
                {
                    //force *= 2.0f;
                }
                else
                {
                    //force *= 0.15f;
                    force = 0.0f;
                }

                //force = 0.0f;
            }

            //force *= force;
            //force *= 0.7f;
            GetComponent<Rigidbody>().AddForce(force * -newGravity * GetComponent<Rigidbody>().mass);
        }
        else
        {
            //reset to defaults
            newGravity = new Vector3(0.0f, -1.0f, 0.0f);
            newGravity *= gravityScalar;
        }

        //ShipModel.rotation = Quaternion.FromToRotation(Vector3.up, -newGravity);

        var quatHit = Quaternion.FromToRotation(Vector3.up, -newGravity);
        var quatForward = Quaternion.FromToRotation(ShipModel.forward, ShipModel.forward);
        var quatC = quatHit * ShipModel.rotation;
        //ShipModel.rotation = quatC;

        Vector3 proj = ShipModel.forward - (Vector3.Dot(ShipModel.forward, -newGravity)) * -newGravity;
        Quaternion newRot = Quaternion.LookRotation(proj, -newGravity);
        ShipModel.rotation = Quaternion.Lerp(ShipModel.rotation, newRot, 3.0f * Time.deltaTime);

        Quaternion newVel = Quaternion.LookRotation(ShipModel.forward);


        //GetComponent<Rigidbody>().velocity = ShipModel.forward * GetComponent<Rigidbody>().velocity.magnitude;
        GetComponent<Rigidbody>().AddForce(ShipModel.forward * 3.0f * GetComponent<Rigidbody>().mass);

        Debug.DrawRay(transform.position, newGravity.normalized * desiredHeight, Color.green);

        Debug.Log(newGravity);

        Vector3 newPos = transform.position - 8.0f * ShipModel.forward + 5.0f * ShipModel.up;
        Vector3 oldPos = cam.transform.position;
        cam.transform.position = Vector3.Lerp(oldPos, newPos, 4.0f * Time.deltaTime);

        //cam.transform.position = transform.position - 6.0f * ShipModel.forward + 3.0f * ShipModel.up;
        Quaternion oldRot = cam.transform.rotation;
        newRot = Quaternion.LookRotation(ShipModel.forward, ShipModel.up);
        cam.transform.rotation = Quaternion.Lerp(oldRot, newRot, 5.0f * Time.deltaTime);
    }
}
