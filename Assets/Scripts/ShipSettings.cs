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
    public float acceleration = 80.0f;
    public float accelerationMin = 10.0f;
    public float brake = 60.0f;
    public float deceleration = 0.5f;
    public float airBrake = 2.0f;
    public float steerSpeed = 3.0f;
    public float steerMomentum = 0.0f;
    public float forwardMomentum = 0.35f;
    public float pitchLimit = 15.0f; //degrees
    public bool driftForward = true;
    [Space(10)]
    public float mass = 1000.0f;
    public float drag = 0.25f;
    public float angularDrag = 5.0f;

    //default colors for customization
    [Header("Ship Colors")]
    public Color primary = Color.white;
    public Color secondary = Color.gray;
    public Color trail = Color.white;
}
