using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform Wings;
    private float originalXScale;
    private float originalYScale;
    public float liftMult = 1f;
    public float dragMult = 0.1f;
    public float spaceInput;
    public float horizontalInput;
    public float verticalInput;
    public float rollInput;

    public float wingInput;
    public float bounceForceMultiplier = 3;
    public float speed = 80.0f;
    public float rotSpeed = 3f;
    public bool isTipGrounded = false;

    private Rigidbody rb;

    // Debug variables
    public float debugArrowScale = 10f; // Scale for lift arrow
    public Vector3 arrowOffset = Vector3.up * 2; // Offset for better visibility

    private Vector3 lift; // Store lift force
    private Vector3 airflow; // Store airflow direction
    private float angleOfAttack; // Store AoA value

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
        // Inputs
        wingInput = Input.GetAxis("Fire1");
        spaceInput = Input.GetAxis("Jump");
        rollInput = Input.GetAxis("Yaw+");
        horizontalInput = Input.GetAxis("Roll");
        verticalInput = Input.GetAxis("Vertical");

        
    }

    void FixedUpdate()
    {
        // Wing scaling
        Wings.localScale = new Vector3(originalXScale, originalYScale, wingInput * 10);

       // Calculate airflow (opposite to velocity)
airflow = rb.linearVelocity.normalized;

// Project airflow onto YX plane
Vector3 projectedAirflow = Vector3.ProjectOnPlane(airflow, Wings.forward);

// Calculate AoA relative to the YX plane
angleOfAttack = Vector3.SignedAngle(Wings.up, projectedAirflow, Wings.right);

        Debug.Log(angleOfAttack);

      

        // Forward speed
        Vector3 forwardVelocity = Vector3.Project(rb.linearVelocity, transform.forward);
        float forwardSpeed = forwardVelocity.magnitude;

        // Calculate lift direction (perpendicular to airflow)
        Vector3 liftDirection = Vector3.Cross(airflow, -transform.right).normalized;
       
       //lift cals
       float optimalAoA = 15f; // AoA for max lift
float stallAoA = 30f;   // AoA where stall begins

float normalizedAoA = angleOfAttack / optimalAoA;
float liftCoefficient;

// Parabolic lift curve with stall behavior
if (Mathf.Abs(angleOfAttack) <= stallAoA)
{
    liftCoefficient = Mathf.Max(0f, 1f - Mathf.Pow(normalizedAoA, 2));
}
else
{
    // Post-stall: Lift drops rapidly
    liftCoefficient = Mathf.Max(0f, 1f - ((Mathf.Abs(angleOfAttack) - stallAoA) / stallAoA));
}

        // Calculate lift force
        lift = liftDirection * forwardSpeed * wingInput * liftCoefficient * liftMult;

        // Apply lift force
        rb.AddForce(lift);

        // Calculate drag force (opposes airflow)
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
        transform.Rotate(verticalInput * rotSpeed, rollInput * rotSpeed /2f, horizontalInput * rotSpeed );

        // Reset functionality
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGlider();
        }
    }

    void OnDrawGizmos()
    {
        if (rb == null) return;

        // Draw lift arrow
        Gizmos.color = Color.green;
        Vector3 startPosition = transform.position + arrowOffset;
        Vector3 endPosition = startPosition + lift * debugArrowScale;
        Gizmos.DrawLine(startPosition, endPosition);
        Gizmos.DrawSphere(endPosition, 0.1f);

        // Draw airflow arrow
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + airflow * debugArrowScale);
    }

    public void ResetGlider()
    {
        // Reset position and velocity
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = new Vector3(416,75 , 123); // Adjust to your desired reset position
        transform.rotation = Quaternion.identity; // Reset orientation
        Debug.Log("Glider Reset");
    }

    public void ApplyBounce(float impactForce)
    {
        // Apply bounce force proportional to the impact force
        Debug.Log(impactForce);
        rb.AddForce(Vector3.up * impactForce * 10, ForceMode.Impulse);
    }
}
