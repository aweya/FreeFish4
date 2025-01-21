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
        // wing stuff

        Wings.localScale = new Vector3(originalXScale,originalYScale,wingInput*10);

       Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
float localVerticalSpeed = localVelocity.y; // Speed along the local Y-axis
Debug.Log("Local Vertical Speed: " + localVerticalSpeed);


        Vector3 lift = transform.forward * localVerticalSpeed * wingInput * liftMult;
rb.AddForce(lift);
        
        // Vector3 drag = transform.forward * -dragMult*wingInput;
       

      
       if (isTipGrounded) // jump mechanic
{
    // Get the magnitude of the velocity
    var accumulatedForce = rb.linearVelocity.magnitude;

    // Apply force in the local Y-axis direction (relative to the pogo stick's orientation)
    Vector3 bounceDirection = transform.up; // Local "up" direction
    rb.AddForce(bounceDirection * accumulatedForce * bounceForceMultiplier, ForceMode.Impulse);

    

    isTipGrounded = false; // Prevent multiple bounces
}

        // char cotrolls

        rb.AddForce(Vector3.up * spaceInput * speed);
        transform.Rotate(verticalInput * rotSpeed, rollInput*rotSpeed/2, horizontalInput * rotSpeed);

        
    }

    void OnDrawGizmos()
{
    Gizmos.color = Color.green;
    Gizmos.DrawLine(transform.position, transform.position + transform.up * 2); // Visualize the local up direction
}


    public void ApplyBounce(float impactForce)
    {
        // Apply bounce force proportional to the impact force
        Debug.Log(impactForce);
        rb.AddForce(Vector3.up * impactForce*10 , ForceMode.Impulse);
    }
}
