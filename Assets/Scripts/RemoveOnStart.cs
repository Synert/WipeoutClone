using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnStart : MonoBehaviour
{
	void Start()
    {
        Destroy(this.gameObject);
	}
}
