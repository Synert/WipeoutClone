using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipHUD : MonoBehaviour
{
    [SerializeField] private Canvas HUD;
    private float speed = 0.0f;
    private Text speedText;

	void Start()
    {
		if(HUD)
        {
            foreach (Transform child in HUD.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.name == "Speed")
                {
                    speedText = child.GetComponent<Text>();
                }
            }
        }
	}
	
	void Update()
    {
        UpdateHUD();
	}

    void UpdateHUD()
    {
        if (!HUD) return;
        speedText.text = (speed * 3.6f).ToString("F2") + " KPH";
    }

    public void UpdateSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
