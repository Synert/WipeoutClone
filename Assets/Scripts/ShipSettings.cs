using UnityEngine;

public class ShipSettings : MonoBehaviour
{
    //controls how the ship handles
    [Header("Ship Handling")]
    public float desiredHeight = 5.0f;
    public float maxHoverForce = 20.0f;
    public float castDistance = 30.0f;
    public float speed = 100.0f;
    public float reverseSpeed = 75.0f;
    public float acceleration = 0.8f;
    public float deceleration = 0.5f;
    public float airBrake = 2.0f;
    public float steerSpeed = 3.0f;
    public float steerMomentum = 0.0f;
    public float forwardMomentum = 0.35f;
    public float pitchLimit = 15.0f; //degrees
    public bool driftForward = true;

    //default colors for customization
    [Header("Ship Colors")]
    public Color primary = Color.white;
    public Color secondary = Color.gray;
    public Color trail = Color.white;
}
