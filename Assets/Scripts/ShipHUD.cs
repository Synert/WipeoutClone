using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipHUD : MonoBehaviour
{
    [SerializeField] private Canvas HUD;

    private float speed, momentum;
    private Text speedText, posText, lapText, momentumText;
    private GameManager g_manager;
    private ShipController ship;

	void Start()
    {
        if (!HUD) return;

        foreach (Transform child in HUD.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.name == "Speed")
            {
                speedText = child.GetComponent<Text>();
            }
            else if (child.gameObject.name == "Position")
            {
                posText = child.GetComponent<Text>();
            }
            else if (child.gameObject.name == "Lap")
            {
                lapText = child.GetComponent<Text>();
            }
            else if (child.gameObject.name == "Momentum")
            {
                momentumText = child.GetComponent<Text>();
            }
        }

        g_manager = FindObjectOfType<GameManager>();
        ship = GetComponent<ShipController>();
    }
	
	void Update()
    {
        UpdateHUD();
	}

    void UpdateHUD()
    {
        if (!HUD) return;

        //actual kph conversion is 3.6f but the ships are 60% too big
        speedText.text = (speed * 2.25f).ToString("F2") + " KPH";
        posText.text = "POS " + g_manager.GetPosition(ship.GetID()).ToString() + "/" + g_manager.GetShipCount();
        lapText.text = "LAP " + g_manager.GetLaps(ship.GetID()).ToString();
        momentumText.text = momentum.ToString("F2");
    }

    public void UpdateSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void UpdateMomentum(float newMomentum)
    {
        momentum = newMomentum;
    }
}
