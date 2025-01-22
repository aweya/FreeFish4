using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform Wings;
    private float originalXScale;
    private float originalYScale;
    public float liftMult=1f;
    public float dragMult=0.1f;
    public float spaceInput;
    public float horizontalInput;
    public float verticalInput;
    public float rollInput;

    public float wingInput;
    public float bounceForceMultiplier=3;
    public float speed = 80.0f;
    public float rotSpeed = 3f;
   // public float bounceForceMultiplier = 1.5f; // Multiplier for the bounce force based on impact
    public bool isTipGrounded = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Find the Wings object
    Wings = transform.Find("Wings");

    if (Wings != null)
    {
        // Store the original X and Y scale values
        originalXScale = Wings.localScale.x;
        originalYScale = Wings.localScale.y;
    }
    else
    {
        Debug.LogError("Wings object not found!");
    }
    }

    void Update()
    {
            // inputs

        wingInput= Input.GetAxis("Fire1");
        spaceInput = Input.GetAxis("Jump");
        rollInput = Input.GetAxis("Roll");
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
       // Debug.Log(rollInput);
    }

void FixedUpdate()
{
    // Wing scaling
    Wings.localScale = new Vector3(originalXScale, originalYScale, wingInput * 10);

// Corrected airflow direction (opposite to the movement direction)
Vector3 airflow = rb.linearVelocity.normalized; // Velocity direction (air moves opposite)

// Angle of Attack (AoA) calculation
float angleOfAttack = 0;
    Debug.Log($"Angle of Attack: {angleOfAttack}");

    // Forward speed (airflow magnitude along the local Y-axis)
    Vector3 forwardVelocity = Vector3.Project(rb.linearVelocity, transform.forward);
    float forwardSpeed = forwardVelocity.magnitude;

    // Calculate lift direction (local Z-axis, perpendicular to airflow)
    Vector3 liftDirection = transform.up; // Local Z-axis is up for lift
    float liftCoefficient = Mathf.Clamp01((15f - Mathf.Abs(angleOfAttack)) / 15f); // Simplified lift curve
    Vector3 lift = liftDirection * forwardSpeed * wingInput * liftCoefficient * liftMult;

    // Apply lift force
    rb.AddForce(lift);

    // Calculate drag direction (opposes airflow)
    Vector3 drag = -airflow * forwardSpeed * wingInput * dragMult;

    // Apply drag force
    rb.AddForce(drag);

    // Debug forces for testing
    Debug.Log($"Lift: {lift}, Drag: {drag}");

    // Jump mechanic
    if (isTipGrounded)
    {
        Vector3 bounceDirection = transform.up; // Local "up" direction
        float accumulatedForce = rb.linearVelocity.magnitude;
        rb.AddForce(bounceDirection * accumulatedForce * bounceForceMultiplier, ForceMode.Impulse);
        isTipGrounded = false; // Prevent multiple bounces
    }

    // Character controls (rotations and thrust)
    rb.AddForce(Vector3.up * spaceInput * speed);
    transform.Rotate(verticalInput * rotSpeed, rollInput * rotSpeed, horizontalInput * rotSpeed);

    // Reset functionality
    if (Input.GetKeyDown(KeyCode.R))
    {
        ResetGlider();
    }
}

void ResetGlider()
{
    // Reset position and velocity
    rb.linearVelocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    transform.position = new Vector3(0, 10, 0); // Adjust to your desired reset position
    transform.rotation = Quaternion.identity; // Reset orientation
    Debug.Log("Glider Reset");
}


    public void ApplyBounce(float impactForce)
    {
        // Apply bounce force proportional to the impact force
        Debug.Log(impactForce);
        rb.AddForce(Vector3.up * impactForce*10 , ForceMode.Impulse);
    }
}
