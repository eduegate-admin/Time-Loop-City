using UnityEngine;

namespace TimeLoopCity.Player
{
    /// <summary>
    /// Third-person player controller with basic movement.
    /// Supports WASD movement, camera rotation, and interaction.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Camera")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float cameraSensitivity = 2f;
        [SerializeField] private float cameraDistance = 5f;

        [Header("Interaction")]
        [SerializeField] private float interactionRange = 3f;
        [SerializeField] private LayerMask interactableLayer;

        private CharacterController controller;
        private Vector3 velocity;
        private float cameraPitch = 0f;
        private Camera mainCamera;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;

            // Lock cursor for better controls
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleMovement();
            HandleCamera();
            HandleGravity();
        }

        private void HandleMovement()
        {
            // Get input using legacy Input Manager
            float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
            float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down arrows

            // Get camera direction
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            // Flatten the direction vectors (ignore Y)
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Calculate movement direction relative to camera
            Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

            // Apply speed (sprint if holding Shift)
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

            // Move the character
            if (moveDirection.magnitude >= 0.1f)
            {
                // Rotate player to face movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Apply movement
                controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            }
        }

        private void HandleCamera()
        {
            if (cameraTarget == null) return;

            // Get mouse input using legacy Input Manager
            float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

            // Adjust pitch (up/down)
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -40f, 70f);

            // Rotate camera horizontally
            cameraTarget.Rotate(Vector3.up * mouseX);

            // Apply pitch
            if (mainCamera != null)
            {
                mainCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
            }
        }

        private void HandleGravity()
        {
            // Apply gravity
            if (controller.isGrounded)
            {
                velocity.y = -2f; // Small negative value to keep grounded
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            controller.Move(velocity * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize interaction range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
