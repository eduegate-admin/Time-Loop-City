using UnityEngine;

namespace TimeLoopCity.Player
{
    /// <summary>
    /// Enhanced player controller with smooth movement, camera follow, and improved controls.
    /// Replaces basic PlayerController with professional-grade movement.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class EnhancedPlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float crouchSpeed = 2.5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 15f;
        [SerializeField] private float rotationSpeed = 12f;

        [Header("Camera")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float cameraDistance = 7f;
        [SerializeField] private float cameraHeight = 2f;
        [SerializeField] private float cameraSmoothTime = 0.1f;
        [SerializeField] private Vector2 pitchMinMax = new Vector2(-30f, 60f);

        [Header("Head Bob")]
        [SerializeField] private bool enableHeadBob = true;
        [SerializeField] private float bobFrequency = 1.5f;
        [SerializeField] private float bobAmplitude = 0.05f;

        [Header("Gravity & Ground")]
        [SerializeField] private float gravity = -15f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private LayerMask groundMask = -1;

        // Components
        private CharacterController controller;
        private Camera mainCamera;

        // State
        private Vector3 velocity;
        private Vector3 currentMovement;
        private float cameraPitch;
        private float cameraYaw;
        private Vector3 cameraVelocity;
        private float headBobTimer;

        // Input
        private bool isSprinting;
        private bool isCrouching;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;

            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Initialize camera angles from current rotation
            if (cameraTarget != null)
            {
                cameraYaw = cameraTarget.eulerAngles.y;
                cameraPitch = 0f;
            }
        }

        private void Update()
        {
            HandleInput();
            HandleMovement();
            HandleCamera();
            HandleGravity();

            // Debug: Unlock cursor with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        private void HandleInput()
        {
            isSprinting = Input.GetKey(KeyCode.LeftShift);
            isCrouching = Input.GetKey(KeyCode.LeftControl);
        }

        private void HandleMovement()
        {
            // Get input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Calculate movement direction relative to camera
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 targetMovement = (forward * vertical + right * horizontal).normalized;

            // Determine speed
            float targetSpeed = walkSpeed;
            if (isSprinting && !isCrouching) targetSpeed = sprintSpeed;
            if (isCrouching) targetSpeed = crouchSpeed;

            targetMovement *= targetSpeed;

            // Smooth acceleration/deceleration
            if (targetMovement.magnitude > 0.1f)
            {
                currentMovement = Vector3.Lerp(currentMovement, targetMovement, acceleration * Time.deltaTime);
            }
            else
            {
                currentMovement = Vector3.Lerp(currentMovement, Vector3.zero, deceleration * Time.deltaTime);
            }

            // Apply movement
            controller.Move(currentMovement * Time.deltaTime);

            // Rotate player to face movement direction
            if (currentMovement.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentMovement);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Head bob
            if (enableHeadBob && currentMovement.magnitude > 0.1f && mainCamera != null)
            {
                headBobTimer += Time.deltaTime * bobFrequency * currentMovement.magnitude;
                float bobOffset = Mathf.Sin(headBobTimer) * bobAmplitude;
                Vector3 camPos = mainCamera.transform.localPosition;
                camPos.y = cameraHeight + bobOffset;
                mainCamera.transform.localPosition = camPos;
            }
        }

        private void HandleCamera()
        {
            if (cameraTarget == null || mainCamera == null) return;

            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Update camera angles
            cameraYaw += mouseX;
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, pitchMinMax.x, pitchMinMax.y);

            // Calculate desired camera position
            Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
            Vector3 targetPosition = cameraTarget.position - rotation * Vector3.forward * cameraDistance;
            targetPosition.y += cameraHeight;

            // Smooth camera movement
            mainCamera.transform.position = Vector3.SmoothDamp(
                mainCamera.transform.position,
                targetPosition,
                ref cameraVelocity,
                cameraSmoothTime
            );

            // Look at target
            mainCamera.transform.LookAt(cameraTarget.position + Vector3.up * cameraHeight);
        }

        private void HandleGravity()
        {
            // Ground check
            bool isGrounded = controller.isGrounded ||
                Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small negative to keep grounded
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }

            controller.Move(velocity * Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize ground check
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.down * groundCheckDistance);

            // Visualize camera target
            if (cameraTarget != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(cameraTarget.position, 0.3f);
            }
        }

        // Public API for other scripts
        public bool IsMoving => currentMovement.magnitude > 0.1f;
        public bool IsSprinting => isSprinting;
        public float MovementSpeed => currentMovement.magnitude;
    }
}
