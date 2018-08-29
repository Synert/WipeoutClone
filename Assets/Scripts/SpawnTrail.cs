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
        trail.GetComponentInChildren<ParticleSystem>().transform.rotation = Quaternion.LookRotation(-transform.parent.parent.forward, transform.parent.parent.up);
        Destroy(gameObject);
	}
}
