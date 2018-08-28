using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTrail : MonoBehaviour
{
    [SerializeField] private int numPoints = 10;
    private ShipController ship;
    private LineRenderer trail, trailCenter;
    private float prevSpeed = 0.0f;
    private float prevSideSpeed = 0.0f;

	void Start()
    {
        foreach (LineRenderer child in GetComponentsInChildren<LineRenderer>())
        {
            if (child.gameObject.name == "Trail")
            {
                trail = child;
            }
            else if (child.gameObject.name == "Trail center")
            {
                trailCenter = child;
            }
        }
        trail.positionCount = numPoints;
        trailCenter.positionCount = numPoints;

        ship = GetComponentInParent<ShipController>();
	}
	
	void Update()
    {
        if (!ship) return;

        float thrust = ship.GetAcceleration();
        float steer = ship.GetSteer();
        float curSpeed = Mathf.Abs(ship.GetCurrentSpeed()) / ship.GetMaxSpeed();
        float curSideSpeed = ship.GetSidewaysSpeed() / ship.GetMaxSpeed();
        float speed = (Mathf.Abs(curSideSpeed) * 10.0f + curSpeed * 10.0f + curSpeed * Mathf.Abs(thrust) * 10.0f + Mathf.Abs(steer) * 5.0f) * 1.0f;
        float sideSpeed = (curSideSpeed * 5.0f - steer * 3.0f - steer * curSideSpeed * 5.0f - steer * curSpeed * 4.0f) * 1.0f;
        if (thrust < 0.0f)
        {
            speed *= 0.5f;
            sideSpeed *= -1.0f;
        }

        speed = Mathf.Lerp(prevSpeed, speed, Time.deltaTime * 7.0f);
        sideSpeed = Mathf.Lerp(prevSideSpeed, sideSpeed, Time.deltaTime * 7.0f);

        prevSpeed = speed;
        prevSideSpeed = sideSpeed;

        trail.positionCount = numPoints;
        trailCenter.positionCount = numPoints;

        Vector3 right = ship.GetRight();
        Vector3 forward = ship.GetForward();

        Vector3 pos = trail.transform.position;
        Vector3 endPos = pos - forward * speed - right * sideSpeed;
        Vector3 p1 = pos - forward * speed * 0.5f - right * sideSpeed * 0.25f;
        Vector3 p2 = pos - forward * speed * 0.75f - right * sideSpeed * 0.75f;

        trail.SetPosition(0, pos);
        trail.SetPosition(numPoints - 1, endPos);

        trailCenter.SetPosition(0, pos);
        trailCenter.SetPosition(numPoints - 1, endPos);

        for (int i = 1; i < (numPoints - 1); i++)
        {
            float t = (i) / (float)(numPoints - 1.0f);
            //Vector3 p = t * p1 + (1.0f - t) * p2;
            //ugh
            Vector3 p = Mathf.Pow((1.0f - t), 3.0f) * pos + 3 * Mathf.Pow((1.0f - t), 2.0f) * t * p1 + 3.0f * (1 - t) * Mathf.Pow(t, 2.0f) * p2 + Mathf.Pow(t, 3.0f) * endPos;
            trail.SetPosition(i, p);
            trailCenter.SetPosition(i, p);
        }
    }
}
