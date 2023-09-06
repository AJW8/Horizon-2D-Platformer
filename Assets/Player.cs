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
    private float walkInput;
    private bool jumpInput;
    private bool onGround;
    private bool flipInput;
    private bool horizonFlipped;

    // Start is called before the first frame update
    void Start()
    {
        colliderWhite.SetActive(false);
        backgroundBlack.SetActive(false);
        rigidBody = GetComponent<Rigidbody2D>();
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
        if (Mathf.Abs(walkInput) > 0.1f) rigidBody.AddForce(Vector2.right * walkInput * walkSpeed, ForceMode2D.Impulse);
        if (onGround && jumpInput) rigidBody.AddForce(Vector2.up * jumpForce * (horizonFlipped ? -1 : 1), ForceMode2D.Impulse);
        if (horizonFlipped ? rigidBody.velocity.y > 0 : rigidBody.velocity.y < 0) rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * (horizonFlipped ? -1 : 1) * Time.deltaTime;
        else if (horizonFlipped ? rigidBody.velocity.y < 0 : rigidBody.velocity.y > 0) rigidBody.velocity += Vector2.up * Physics2D.gravity.y * (jumpMultiplier - 1) * (horizonFlipped ? -1 : 1) * Time.deltaTime;
        if (onGround && flipInput)
        {
            Transform cam = Camera.main.transform;
            cam.position = new Vector3(cam.position.x, cam.position.y, -cam.position.z);
            if (horizonFlipped)
            {
                cam.rotation = Quaternion.LookRotation(Vector3.forward, Vector2.up);
                colliderWhite.SetActive(false);
                colliderBlack.SetActive(true);
                backgroundBlack.SetActive(false);
                backgroundWhite.SetActive(true);
            }
            else
            {
                cam.rotation = Quaternion.LookRotation(-Vector3.forward, -Vector2.up);
                colliderBlack.SetActive(false);
                colliderWhite.SetActive(true);
                backgroundBlack.SetActive(true);
                backgroundWhite.SetActive(false);
            }
            rigidBody.gravityScale *= -1;
            horizonFlipped = !horizonFlipped;
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
