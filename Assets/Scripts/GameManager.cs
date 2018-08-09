using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Track Settings")]
    [SerializeField] private float gravityScalar = 19.8f;

    private int numShips = 0;
    private List<ShipController> ships;
    private List<int> shipLapCheckPoints, shipCheckPoints, shipPositions;
    private bool hasInit = false;

    void Start()
    {
        Init();
    }

    void Init()
    {
        if (hasInit) return;
        else hasInit = true;
        ships = new List<ShipController>();
        shipLapCheckPoints = new List<int>();
        shipCheckPoints = new List<int>();
        shipPositions = new List<int>();
    }
	
	void Update()
    {
        SortPositions();
	}

    public float GetGravity()
    {
        return gravityScalar;
    }

    public int RegisterShip(ShipController ship)
    {
        Init();
        ships.Add(ship);
        shipCheckPoints.Add(0);
        shipLapCheckPoints.Add(0);
        shipPositions.Add(numShips);
        numShips++;
        return numShips - 1;
    }

    public int GetPosition(int shipID)
    {
        for(int i = 0; i < numShips; i++)
        {
            if(shipPositions[i] == shipID) return i + 1;
        }
        return -1;
    }

    void SortPositions()
    {
        shipPositions.Sort(ComparePositions);
    }

    int ComparePositions(int x, int y)
    {
        if (x == y) return 0;
        if(shipCheckPoints[x] > shipCheckPoints[y]) return 1;
        return -1;
    }

    public int GetShipCount()
    {
        return numShips;
    }
}
