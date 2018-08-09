using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Track Settings")]
    [SerializeField] private float gravityScalar = 19.8f;

    private int numShips = 0;
    private List<int> numCheckPoints;
    private List<ShipController> ships;

    void Start()
    {
        numCheckPoints = new List<int>();
        ships = new List<ShipController>();
	}
	
	void Update()
    {
		
	}

    public float GetGravity()
    {
        return gravityScalar;
    }

    public int RegisterShip(ShipController ship)
    {
        numCheckPoints.Add(0);
        ships.Add(ship);
        numShips++;
        return numShips - 1;
    }

    public int GetPosition(int shipID)
    {
        int position = 1;
        List<int> samePos;
        samePos = new List<int>();
        for(int i = 0; i < numShips; i++)
        {
            if (i == shipID) continue;
            if (numCheckPoints[i] > numCheckPoints[shipID]) position++;
            else if(numCheckPoints[i] == numCheckPoints[shipID]) samePos.Add(i);
        }

        for(int i = 0; i < samePos.Count; i++)
        {
            //do the distance checking here when checkpoints actually exist
        }

        return position;
    }
}
