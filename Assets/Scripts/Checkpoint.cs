using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointID = 0;

    private GameManager g_manager;

	void Start()
    {
        g_manager = FindObjectOfType<GameManager>();
        g_manager.RegisterCheckpoint(checkpointID);
	}
	
	// Update is called once per frame
	void Update()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "ShipModel") return;
        ShipController ship = other.gameObject.GetComponentInParent<ShipController>();
        if (ship)
        {
            g_manager.Checkpoint(ship.GetID(), checkpointID);
        }
    }
}
