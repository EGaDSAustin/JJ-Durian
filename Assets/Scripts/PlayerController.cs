using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static bool GameStarted = false;

    [SerializeField]
    private float maxSpeed = 4f;

    public bool hasFish = true;

    Rigidbody rb;

    public RectTransform compassLocation;
    public Transform goalLocation;

    public TextMeshProUGUI snowStormTxt;
    public TextMeshProUGUI survivorNum;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = !GameStarted;

        if (GameStarted)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(timer());
        }
    }

    public void startGame()
    {
        GameObject.Find("Canvas").GetComponent<Animator>().SetTrigger("StartGame");
        GameStarted = true;

        foreach (Rigidbody rbs in FindObjectsOfType<Rigidbody>())
            rbs.isKinematic = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(timer());
    }

    Vector2 headRot = Vector2.zero;

    // Update is called once per frame
    void Update()
    {
        if (GameStarted)
        {
            //Compass Locator
            float targetRot = Mathf.Atan2((goalLocation.position.x - transform.position.x), (goalLocation.position.z - transform.position.z)) * Mathf.Rad2Deg;
            compassLocation.rotation = Quaternion.Euler(0, 0, transform.GetChild(0).rotation.eulerAngles.y - targetRot);


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

    IEnumerator timer()
    {
        int time = 20;
        while (time > 0)
        {
            snowStormTxt.SetText("THE SNOWSTORM IS COMING - " + (time / 60) + ":" + (time % 60 < 10 ? "0" : "") + (time % 60));
            time--;
            yield return new WaitForSeconds(1);
        }

        GameStarted = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        survivorNum.SetText("\n" + FindObjectOfType<PenguinDetection>().penguinCount);
        GameObject.Find("Canvas").GetComponent<Animator>().SetTrigger("EndGame");
    }

    public void restart()
    {
        SceneManager.LoadScene(0);
    }
}
