using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCustomization : MonoBehaviour
{
    [SerializeField] private Color primary, secondary, trail;

    private Color shipPrimary, shipSecondary, shipTrail;
    private Transform model;

    public void Init(Transform shipModel)
    {
        model = shipModel;

        shipPrimary = primary;
        shipSecondary = secondary;
        shipTrail = trail;
        UpdateColors();
    }
	
	void Update()
    {
        CheckColors();
	}

    void CheckColors()
    {
        if (shipPrimary != primary || shipSecondary != secondary || shipTrail != trail)
        {
            shipPrimary = primary;
            shipSecondary = secondary;
            shipTrail = trail;
            UpdateColors();
        }
    }

    void UpdateColors()
    {
        foreach (Material mat in model.GetComponent<Renderer>().materials)
        {
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

        foreach(TrailRenderer tR in model.GetComponentsInChildren<TrailRenderer>())
        {
            tR.startColor = shipTrail;
        }

        foreach(ParticleSystem pS in model.GetComponentsInChildren<ParticleSystem>())
        {
            pS.startColor = shipTrail;
        }
    }

    public void SetColors(Color newPrimary, Color newSecondary, Color newTrail)
    {
        primary = newPrimary;
        secondary = newSecondary;
        trail = newTrail;

        CheckColors();
    }
}
