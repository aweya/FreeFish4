using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float spaceInput;
    public float horizontalInput;
    public float verticalInput;

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
    }

    void Update()
    {

     


       // Debug.Log("Axis 3: " + Input.GetAxis("Axis 3"));
      //  Debug.Log("Axis 10: " + Input.GetAxis("Axis 10"));
      wingInput= Input.GetAxis("Fire1");
      
      Debug.Log(wingInput);
      

      if (spaceInput>1.0f){
      Debug.Log("Fire 1");
      }
   

        spaceInput = Input.GetAxis("Jump");
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
       //  Debug.Log( rb.linearVelocity.magnitude);
       if (isTipGrounded)
{
    // Get the magnitude of the velocity
    var accumulatedForce = rb.linearVelocity.magnitude;

    // Apply force in the local Y-axis direction (relative to the pogo stick's orientation)
    Vector3 bounceDirection = transform.up; // Local "up" direction
    rb.AddForce(bounceDirection * accumulatedForce * bounceForceMultiplier, ForceMode.Impulse);

    

    isTipGrounded = false; // Prevent multiple bounces
}

        //Debug.Log("Current Velocity (Vector3): " + rb.linearVelocity);

        rb.AddForce(Vector3.up * spaceInput * speed);
        transform.Rotate(verticalInput * rotSpeed, 0, horizontalInput * rotSpeed);

        
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
