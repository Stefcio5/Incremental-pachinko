using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject, InputSystem_Actions.IGameplayActions
{
    private InputSystem_Actions _inputActions;

    public event Action SpawnBallEvent;
    public event Action SpawnBallCancelEvent;

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Gameplay.SetCallbacks(this);
        }
        SetGameplay();
    }
    private void OnDisable()
    {
        _inputActions.Gameplay.Disable();
    }

    public void SetGameplay()
    {
        _inputActions.Gameplay.Enable();
    }
    public void OnSpawnBall(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            SpawnBallEvent?.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            SpawnBallCancelEvent?.Invoke();
        }
    }
}
