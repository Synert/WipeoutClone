using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterPad : MonoBehaviour
{
    [SerializeField] private float flatBoost = 25.0f;
    [SerializeField] private float percentBoost = 25.0f;

	void Start()
    {
		
	}
	
	void Update()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("AAAAAAAA");
        Debug.Log(other.name);
        ShipController ship = other.gameObject.GetComponentInParent<ShipController>();
        if(ship)
        {
            ship.GetComponent<Rigidbody>().AddForce(transform.forward * (flatBoost + ship.GetMaxSpeed() * percentBoost), ForceMode.Impulse);
        }
    }
}
