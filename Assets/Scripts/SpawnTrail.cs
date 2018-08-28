using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrail : MonoBehaviour
{
    [SerializeField] private Transform trailPrefab;

	void Start()
    {
        Transform trail = Instantiate(trailPrefab, transform.parent);
        trail.position = transform.position;
        Destroy(gameObject);
	}
}
