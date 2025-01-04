using UnityEngine;

public class NE_PlayerController : MonoBehaviour
{
    public static bool portaling = false;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera portalCameraA;
    [SerializeField] private Camera portalCameraB;
    [SerializeField] private Vector3 offsetA;
    [SerializeField] private Vector3 offsetB;

    private CharacterController controller;

    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    private Vector2 mouseInput;
    private float yaw = 0f;
    private float pitch = 0f;

    [Header("Æ÷ÅÐ")]
    [SerializeField] private Transform entrancePortal;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    private void Update()
    {
        mouseInput = GetMouse();
        yaw += mouseInput.x * rotationSpeed;
        pitch -= mouseInput.y * rotationSpeed;

        pitch = Mathf.Clamp(pitch, -60f, 50f);
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    private void LateUpdate()
    {
        CameraInput();
    }

    private void PlayerMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 destination = (transform.forward * z + transform.right * x) * Time.deltaTime * speed;

        if (!GetComponent<CharacterController>().isGrounded)
            destination += new Vector3(0, -9.8f, 0);
        

        controller.Move(destination);
        mainCamera.transform.position = transform.position;
    }

    private void CameraInput()
    {
        //Ä«¸Þ¶ó ÀÎÇ²
        

        transform.rotation = Quaternion.Euler(0, yaw, 0);
        mainCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        portalCameraA.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        portalCameraA.transform.position = transform.position + offsetA;

        portalCameraB.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        portalCameraB.transform.position = transform.position + offsetB;
    }

    private Vector2 GetMouse()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}
