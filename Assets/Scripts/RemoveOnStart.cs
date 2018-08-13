using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnStart : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 0.0f;

	void Start()
    {
        if(destroyDelay == 0.0f) Destroy(this.gameObject);
	}

    void Update()
    {
        destroyDelay -= Time.deltaTime;
        if (destroyDelay <= 0.0f) Destroy(this.gameObject);
    }
}
