using UnityEngine;

namespace TimeLoopCity.Player
{
    /// <summary>
    /// Simple third-person controller that handles locomotion,
    /// camera look, and basic interaction raycasts.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 1.25f;

        [Header("Camera")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float cameraSensitivity = 2f;
        [SerializeField] private float minPitch = -35f;
        [SerializeField] private float maxPitch = 65f;

        [Header("Interaction")]
        [SerializeField] private float interactionRange = 3f;
        [SerializeField] private LayerMask interactableLayer = ~0;

        private CharacterController controller;
        private Camera mainCamera;
        private Vector3 verticalVelocity;
        private float cameraPitch;
        private float yaw;
        private InteractableObject focusedInteractable;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            mainCamera = Camera.main;
            yaw = transform.eulerAngles.y;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleCamera();
            HandleMovement();
            HandleInteraction();
        }

        private void HandleMovement()
        {
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputZ = Input.GetAxisRaw("Vertical");
            Vector3 inputVector = new Vector3(inputX, 0f, inputZ);

            bool hasInput = inputVector.sqrMagnitude > 0.01f;
            float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

            if (hasInput)
            {
                inputVector.Normalize();
                Vector3 moveDirection = Quaternion.Euler(0f, yaw, 0f) * inputVector;
                controller.Move(moveDirection * (targetSpeed * Time.deltaTime));
            }

            if (controller.isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -2f;
            }

            if (Input.GetButtonDown("Jump") && controller.isGrounded)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            verticalVelocity.y += gravity * Time.deltaTime;
            controller.Move(verticalVelocity * Time.deltaTime);
        }

        private void HandleCamera()
        {
            if (cameraTarget == null) return;

            float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

            yaw += mouseX;
            cameraPitch = Mathf.Clamp(cameraPitch - mouseY, minPitch, maxPitch);

            cameraTarget.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        private void HandleInteraction()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null) return;

            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer, QueryTriggerInteraction.Collide))
            {
                InteractableObject interactable = hit.collider.GetComponentInParent<InteractableObject>();
                if (interactable != null)
                {
                    FocusInteractable(interactable);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactable.Interact(this);
                    }
                    return;
                }
            }

            FocusInteractable(null);
        }

        private void FocusInteractable(InteractableObject interactable)
        {
            if (focusedInteractable == interactable) return;

            if (focusedInteractable != null)
            {
                focusedInteractable.ShowInteractionUI(false);
            }

            focusedInteractable = interactable;

            if (focusedInteractable != null)
            {
                focusedInteractable.ShowInteractionUI(true);
            }
        }
    }
}
