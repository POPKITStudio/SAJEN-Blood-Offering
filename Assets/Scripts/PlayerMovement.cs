using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header ("Movements")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    private CharacterController controller;

    [Header("Camera")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float fovSmoothSpeed = 8f;
    public Transform cameraTransform;
    public float crouchCamHeight = 0.5f;
    public float standCamHeight = 0.9f;

    [Header("Crouching")] 
    public float crouchHeight = 0.9f;
    public float normalHeight = 1.8f;
    public float crouchSpeed = 1.5f;

    private bool isCrouching = false;
    private Vector3 targetCameraPosition;
    public float crouchTransitionSpeed = 5f;
    [SerializeField] private Transform cameraRoot;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float moveSpeed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed);

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        if (move.magnitude > 1f)
            move = move.normalized;

        controller.Move(move * moveSpeed * Time.deltaTime);

        // FOV Zoom saat Sprint
        if (playerCamera != null)
        {
            float targetFOV = isSprinting ? sprintFOV : normalFOV;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
        }

        HandleCrouch();
    }
    void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = crouchHeight;
        }
        else
        {
            isCrouching = false;
            controller.height = normalHeight;
        }

        float targetHeight = isCrouching ? crouchCamHeight : standCamHeight;
        Vector3 targetPosition = new Vector3(cameraRoot.localPosition.x, targetHeight, cameraRoot.localPosition.z);
        cameraRoot.localPosition = Vector3.Lerp(cameraRoot.localPosition, targetPosition, Time.deltaTime * crouchTransitionSpeed);
    }
}
