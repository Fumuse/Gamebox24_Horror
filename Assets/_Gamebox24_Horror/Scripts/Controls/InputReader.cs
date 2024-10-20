using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private Controls _controls;
    public Vector2 PlayerMovePosition { get; private set; }
    public bool PlayerSprinted { get; private set; } = false;

    public Action OnJumpPerformed;
    public Action OnSprintStarted;
    public Action OnSprintEnded;

    public delegate void CameraZoom(int modifier);
    public static event CameraZoom OnCameraZoom;

    private void Awake()
    {
        _controls = new();
    }

    private void OnEnable()
    {
        _controls.Enable();

        _controls.PlayerControls.Movement.performed += OnMove;
        _controls.PlayerControls.Sprint.started += OnStartSprint;
        _controls.PlayerControls.Sprint.canceled += OnEndSprint;
        _controls.PlayerControls.Jump.performed += OnJump;
        _controls.PlayerControls.CameraZoom.performed += OnScrollCameraZoom;
    }

    private void OnDisable()
    {
        _controls.Disable();
        
        PlayerMovePosition = Vector2.zero;
        
        _controls.PlayerControls.Movement.performed -= OnMove;
        _controls.PlayerControls.Sprint.started -= OnStartSprint;
        _controls.PlayerControls.Sprint.canceled -= OnEndSprint;
        _controls.PlayerControls.Jump.performed -= OnJump;
        _controls.PlayerControls.CameraZoom.performed -= OnScrollCameraZoom;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        PlayerMovePosition = context.ReadValue<Vector2>();
    }

    private void OnStartSprint(InputAction.CallbackContext context)
    {
        PlayerSprinted = true;
        OnSprintStarted?.Invoke();
    }

    private void OnEndSprint(InputAction.CallbackContext context)
    {
        PlayerSprinted = false;
        OnSprintEnded?.Invoke();
    }
    
    private void OnJump(InputAction.CallbackContext context)
    {
        OnJumpPerformed?.Invoke();
    }

    private void OnScrollCameraZoom(InputAction.CallbackContext context)
    {
        Vector2 scrollValue = context.ReadValue<Vector2>();
        int zoomModifier = scrollValue.y > 0 ? 1 : -1;
        OnCameraZoom?.Invoke(zoomModifier);
    }
}