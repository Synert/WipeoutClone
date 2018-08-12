using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipHUD : MonoBehaviour
{
    [SerializeField] private Canvas HUD;

    private float speed = 0.0f;
    private Text speedText, posText, lapText;
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
        speedText.text = (speed * 3.6f).ToString("F2") + " KPH";
        posText.text = "POS " + g_manager.GetPosition(ship.GetID()).ToString() + "/" + g_manager.GetShipCount();
        lapText.text = "LAP " + g_manager.GetLaps(ship.GetID()).ToString();
    }

    public void UpdateSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
