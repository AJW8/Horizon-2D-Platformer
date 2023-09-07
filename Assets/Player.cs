using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject colliderBlack;
    public GameObject colliderWhite;
    public GameObject backgroundBlack;
    public GameObject backgroundWhite;
    public float walkSpeed;
    public float jumpForce;
    public float jumpMultiplier;
    public float fallMultiplier;
    private Rigidbody2D rigidBody;
    private float camZ;
    private float walkInput;
    private bool jumpInput;
    private bool onGround;
    private bool flipInput;
    private bool horizonFlipped;
    private float flipFactor;
    private bool flipMiddle;

    // Start is called before the first frame update
    void Start()
    {
        colliderWhite.SetActive(false);
        backgroundBlack.SetActive(false);
        rigidBody = GetComponent<Rigidbody2D>();
        camZ = Mathf.Abs(Camera.main.transform.position.z);
        onGround = true;
        horizonFlipped = false;
    }

    // Update is called once per frame
    void Update()
    {
        walkInput = Input.GetAxisRaw("Horizontal");
        jumpInput = Input.GetKeyDown(KeyCode.UpArrow);
        flipInput = Input.GetKeyDown(KeyCode.DownArrow);
    }

    void FixedUpdate()
    {
        if (horizonFlipped ? flipFactor < 1 : flipFactor > 0)
        {
            Transform cam = Camera.main.transform;
            if (horizonFlipped)
            {
                flipFactor += Time.deltaTime;
                if (flipFactor >= 1) flipFactor = 1;
                else if (!flipMiddle && flipFactor >= 0.5f)
                {
                    colliderBlack.SetActive(false);
                    colliderWhite.SetActive(true);
                    backgroundBlack.SetActive(true);
                    backgroundWhite.SetActive(false);
                    rigidBody.gravityScale *= -1;
                    flipMiddle = true;
                }
            }
            else
            {
                flipFactor -= Time.deltaTime;
                if (flipFactor <= 0) flipFactor = 0;
                else if (!flipMiddle && flipFactor <= 0.5f)
                {
                    colliderWhite.SetActive(false);
                    colliderBlack.SetActive(true);
                    backgroundBlack.SetActive(false);
                    backgroundWhite.SetActive(true);
                    rigidBody.gravityScale *= -1;
                    flipMiddle = true;
                }
            }
            cam.position = new Vector3(cam.position.x, cam.position.y, camZ * Mathf.Sin(Mathf.PI * (flipFactor * 2 - 1) / 2));
            cam.rotation = Quaternion.Slerp(Quaternion.LookRotation(Vector3.forward, Vector2.up), Quaternion.LookRotation(-Vector3.forward, -Vector2.up), flipFactor);
            return;
        }
        if (Mathf.Abs(walkInput) > 0.1f) rigidBody.AddForce(Vector2.right * walkInput * walkSpeed, ForceMode2D.Impulse);
        if (onGround && jumpInput) rigidBody.AddForce(Vector2.up * jumpForce * (horizonFlipped ? -1 : 1), ForceMode2D.Impulse);
        if (horizonFlipped ? rigidBody.velocity.y > 0 : rigidBody.velocity.y < 0) rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * (horizonFlipped ? -1 : 1) * Time.deltaTime;
        else if (horizonFlipped ? rigidBody.velocity.y < 0 : rigidBody.velocity.y > 0) rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (jumpMultiplier - 1) * (horizonFlipped ? -1 : 1) * Time.deltaTime;
        if (onGround && flipInput)
        {
            horizonFlipped = !horizonFlipped;
            flipMiddle = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject != colliderBlack && collider.gameObject != colliderWhite) onGround = true;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject != colliderBlack && collider.gameObject != colliderWhite) onGround = false;
    }
}
