using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 4f;

    public bool hasFish = true;

    Rigidbody rb;
    Transform cameraAgent;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraAgent = transform.GetChild(0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    Vector2 headRot = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        //Mouse Controls
        float mouseX = Input.GetAxis("Mouse X") * 3;
        float mouseY = Input.GetAxis("Mouse Y") * 3;

        headRot += new Vector2(mouseX, -mouseY);
        headRot.y = Mathf.Clamp(headRot.y, -80, 80);

        transform.GetChild(0).rotation = Quaternion.Euler(headRot.y, headRot.x, 0f);

        //Previous Movement Code
        //Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //transform.position += direction * speed;

        bool onGround = Physics.Raycast(new Ray(transform.position, Vector3.down), 1.1f);

        Vector3 velocity = rb.velocity;

        velocity = Quaternion.Euler(0, -headRot.x, 0f) * velocity;

        Vector3 movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //Quick Turn Around
        /*if (((movementVector.x > 0 && velocity.x < 0) || (movementVector.x < 0 && velocity.x > 0)) && onGround)
            velocity.x *= .9f;
        if (((movementVector.z > 0 && velocity.z < 0) || (movementVector.z < 0 && velocity.z > 0)) && onGround)
            velocity.z *= .9f;*/
        //Accelerate to Max Veloc
        if ((movementVector.x > 0 && velocity.x < maxSpeed) || (movementVector.x < 0 && velocity.x > -maxSpeed))
            velocity.x += movementVector.x * .05f;
        if ((movementVector.z > 0 && velocity.z < maxSpeed) || (movementVector.z < 0 && velocity.z > -maxSpeed))
            velocity.z += movementVector.z * .05f;
        //Decelerate quicker when not moving
        if (movementVector.x == 0 && onGround)
            velocity.x *= .999f;
        if (movementVector.z == 0 && onGround)
            velocity.z *= .999f;

        velocity = Quaternion.Euler(0, headRot.x, 0f) * velocity;

        //If not on ground, movement not as effective
        if (!onGround)
            velocity = (velocity * .25f) + (rb.velocity * .75f);

        if (onGround && Input.GetKeyDown(KeyCode.Space))
            velocity += Vector3.up * 6;

        rb.velocity = velocity;
    }
}
