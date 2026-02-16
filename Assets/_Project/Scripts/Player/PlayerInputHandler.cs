using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour {

    public Vector2 MoveInput { get; private set; }
    public bool JumpHeld { get; private set; }

    public event Action OnJumpPressed;
    public event Action OnJumpReleased;
    public event Action OnAttackPressed;
    public event Action OnAttackReleased;
    public event Action OnInteractPressed;
    public event Action OnPausePressed;
    public event Action OnInventoryPressed;

    public void OnMove(InputAction.CallbackContext context) => MoveInput = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context) {
        if (context.started) OnJumpPressed?.Invoke();
        if (context.canceled) OnJumpReleased?.Invoke();

        JumpHeld = context.ReadValueAsButton();
    }

    public void OnAttack(InputAction.CallbackContext context) {
        if (context.started) OnAttackPressed?.Invoke();
        if (context.canceled) OnAttackReleased?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.started) OnInteractPressed?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext context) {
        if (context.started) OnPausePressed?.Invoke();
    }

    public void OnInventory(InputAction.CallbackContext context) {
        if (context.started) OnInventoryPressed?.Invoke();
    }
}
