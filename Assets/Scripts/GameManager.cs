using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Track Settings")]
    [SerializeField] private float gravityScalar = 19.8f;

    private int numShips = 0;
    private int checkpointTotal = 0;
    private int checkpointHighest = 0;
    private List<ShipController> ships;
    private List<int> shipCheckpoints, shipPositions;
    private List<Checkpoint> checkpoints;
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
        shipCheckpoints = new List<int>();
        shipPositions = new List<int>();
        checkpoints = new List<Checkpoint>();
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
        shipCheckpoints.Add(0);
        shipPositions.Add(numShips);
        numShips++;
        return numShips - 1;
    }

    public int GetPosition(int shipID)
    {
        for(int i = 0; i < numShips; i++)
        {
            if(shipPositions[i] == shipID) return (numShips - i);
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
        if(shipCheckpoints[x] > shipCheckpoints[y]) return 1;
        else if(shipCheckpoints[x] == shipCheckpoints[y])
        {
            float distX = 900000.0f;
            float distY = 900000.0f;
            foreach(Checkpoint cp in checkpoints)
            {
                if(cp.GetID() == (shipCheckpoints[x] + 1) % checkpointHighest)
                {
                    float dist = Vector3.Distance(ships[x].transform.position, cp.transform.position);
                    if (dist < distX) distX = dist;
                    dist = Vector3.Distance(ships[y].transform.position, cp.transform.position);
                    if (dist < distY) distY = dist;
                }
            }
            if (distX < distY) return 1;
        }
        return -1;
    }

    public int GetShipCount()
    {
        return numShips;
    }
    
    public void RegisterCheckpoint(int checkpointID, Checkpoint checkpoint)
    {
        Init();
        checkpointTotal++;
        if (checkpointID > checkpointHighest) checkpointHighest = checkpointID;
        checkpoints.Add(checkpoint);
    }

    public void Checkpoint(int shipID, int checkpointID)
    {
        if(shipCheckpoints[shipID] % checkpointHighest == checkpointID - 1)
        {
            shipCheckpoints[shipID]++;
        }
    }

    public int GetLaps(int shipID)
    {
        return (int)Mathf.Floor(shipCheckpoints[shipID] / checkpointHighest) + 1;
    }
}
