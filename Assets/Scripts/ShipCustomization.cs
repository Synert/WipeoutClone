using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCustomization : MonoBehaviour {

    [SerializeField] private Color primary, secondary, trail;
    private Color shipPrimary, shipSecondary, shipTrail;
    private Transform model;

    // Use this for initialization
    void Start () {
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.name == "ShipModel")
            {
                model = child;
                break;
            }
        }

        shipPrimary = primary;
        shipSecondary = secondary;
        shipTrail = trail;
        UpdateColors();
    }
	
	// Update is called once per frame
	void Update () {
        if (shipPrimary != primary || shipSecondary != secondary || shipTrail != trail)
        {
            shipPrimary = primary;
            shipSecondary = secondary;
            shipTrail = trail;
            UpdateColors();
        }
	}

    void NewColors(Color newPrimary, Color newSecondary, Color newTrail)
    {
        primary = newPrimary;
        secondary = newSecondary;
        trail = newTrail;
    }

    void UpdateColors()
    {
        foreach (Material mat in model.GetComponent<Renderer>().materials)
        {
            Debug.Log(mat.name);
            if (mat.name == "PrimaryColor (Instance)")
            {
                mat.color = shipPrimary;
            }
            else if (mat.name == "SecondaryColor (Instance)")
            {
                mat.color = shipSecondary;
            }
        }

        foreach(Light light in model.parent.GetComponentsInChildren<Light>())
        {
            light.color = shipTrail;
        }

        model.GetComponentInChildren<TrailRenderer>().startColor = shipTrail;

        model.GetComponentInChildren<ParticleSystem>().startColor = shipTrail;
    }
}
