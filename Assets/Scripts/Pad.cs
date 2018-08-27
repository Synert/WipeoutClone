using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pad : MonoBehaviour
{
    [SerializeField] private float flatBoost = 25.0f;
    [SerializeField] private float percentBoost = 25.0f;
    [SerializeField] private bool isBooster = true;

	void Start()
    {

	}
	
	void Update()
    {

	}

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "ShipModel") return;
        ShipController ship = other.gameObject.GetComponentInParent<ShipController>();
        if(ship)
        {
            if (isBooster) BoosterPad(ship);
            else WeaponPad(ship);
        }
    }

    void BoosterPad(ShipController ship)
    {
        ship.GetComponent<Rigidbody>().AddForce(transform.forward * (flatBoost + ship.GetMaxSpeed() * (percentBoost * 0.01f)), ForceMode.VelocityChange);
    }

    void WeaponPad(ShipController ship)
    {

    }
}
