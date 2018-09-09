using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCustomization : MonoBehaviour
{
    [SerializeField] private Color primary, secondary, trail;

    private ShipSettings colors;
    private Color shipPrimary, shipSecondary, shipTrail;
    private Transform model;
    private ShipController ship;
    private bool doOnce = false;

    public void Init(Transform shipModel)
    {
        model = shipModel;
        colors = model.GetComponentInParent<ShipSettings>();
        ship = model.GetComponentInParent<ShipController>();

        primary = colors.primary;
        secondary = colors.secondary;
        trail = colors.trail;
        shipPrimary = colors.primary;
        shipSecondary = colors.secondary;
        shipTrail = colors.trail;
        UpdateColors();
    }
	
	void Update()
    {
        if (!doOnce)
        {
            UpdateColors();
            doOnce = true;
        }
        CheckColors();
        PulseTrail();
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

        foreach(LineRenderer tR in model.GetComponentsInChildren<LineRenderer>())
        {
            tR.startColor = shipTrail;
        }

        foreach (ParticleSystem pS in model.GetComponentsInChildren<ParticleSystem>())
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

    void PulseTrail()
    {
        float speedRatio = Mathf.Abs(ship.GetCurrentSpeed() / ship.GetMaxSpeed());
        foreach (LineRenderer tR in model.GetComponentsInChildren<LineRenderer>())
        {
            Color newColor = shipTrail * (0.75f + 0.15f * Mathf.Sin(Time.time * 4.0f) + speedRatio * 0.4f);
            newColor.a = 1.0f;
            tR.startColor = newColor;
        }
    }
}
