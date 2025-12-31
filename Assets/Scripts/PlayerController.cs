using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5;
    public float rotationSpeed = 10;
    public Camera playerCamera;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamera = Camera.main;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move the player horizontally based on input
        var horizontal = Input.GetAxis("Horizontal");

        //Move the player vertically based on input
        var vertical = Input.GetAxis("Vertical");

        // Movements are relative to camera direction
        var movement = new Vector3(horizontal, 0, vertical);
        var appliedMovement = movement.normalized * Time.deltaTime * speed;

        if (horizontal != 0 || vertical != 0)
        {
            // Lerp into the direction of movement
            var targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        transform.Translate(appliedMovement, Space.World);
        animator.SetFloat("Movement", movement.magnitude);

        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Debug.Log("Clicked on: " + hitInfo.collider.gameObject.name);
            }
        }
    }
}
