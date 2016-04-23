using UnityEngine;
using System;
using System.Collections.Generic;
using XInputDotNetPure;
using System.Collections;


/// <summary>
/// Player Input.
/// Receives inputs for ther player.
/// </summary>
public class PlayerInput : MonoBehaviour
{
    public PlayerIndex playerIndex;

    public Axis movementAxis;
    public Axis lookAxis;

    public ButtonMappingName[] buttonsNames;

    public Vector2 MovementInput { get { return movementAxis.Value; } }
    public Vector2 LookInput { get { return lookAxis.Value; } }

    private GamePadState currentGamepadState;
    private GamePadState prevGamepadState;

    private Dictionary<string, string> keyButtonsNames;
    private Dictionary<string, GamePadButtonsName> gamepadButtonsNames;


    void Start()
    {
        RegisterButtons();
        currentGamepadState = GamePad.GetState(playerIndex);
    }

    private void RegisterButtons()
    {
        keyButtonsNames = new Dictionary<string, string>();
        gamepadButtonsNames = new Dictionary<string, GamePadButtonsName>();
        for (int i = 0; i < buttonsNames.Length; i++)
        {
            keyButtonsNames.Add(buttonsNames[i].VirtualName, buttonsNames[i].KeyName);
        }

        for (int i = 0; i < buttonsNames.Length; i++)
        {
            gamepadButtonsNames.Add(buttonsNames[i].VirtualName, buttonsNames[i].GamePadButton);
        }
    }

    public void Stop()
    {
        movementAxis.Value = Vector2.zero;
        lookAxis.Value = Vector2.zero;
    }

    public bool IsMoving
    {
        get { return movementAxis.Value.sqrMagnitude > 0f; }
    }
    public bool IsMovingCamera
    {
        get { return lookAxis.Value.sqrMagnitude > 0f; }
    }

    public float MovementIntensity
    {
        get
        {
            return Mathf.Abs(Mathf.Clamp(movementAxis.Value.sqrMagnitude, 0f, 1f));
        }
    }

    public float LookIntensity
    {
        get
        {
            return Mathf.Abs(Mathf.Clamp(lookAxis.Value.sqrMagnitude, 0f, 1f));
        }
    }

    void Update()
    {
        prevGamepadState = currentGamepadState;
        currentGamepadState = GamePad.GetState(playerIndex);

        movementAxis.Value = new Vector2(Input.GetAxis(movementAxis.VirtualHorizontal),
            Input.GetAxis(movementAxis.VirtualVertical));
        if (movementAxis.Value.sqrMagnitude < 0.01f)
        {
            movementAxis.Value = new Vector2(currentGamepadState.ThumbSticks.Left.X, currentGamepadState.ThumbSticks.Left.Y);
        }

        lookAxis.Value = new Vector2(Input.GetAxis(lookAxis.VirtualHorizontal),
            Input.GetAxis(lookAxis.VirtualVertical));
        if (lookAxis.Value.sqrMagnitude < 0.01f)
        {
            lookAxis.Value = new Vector2(currentGamepadState.ThumbSticks.Right.X, currentGamepadState.ThumbSticks.Right.Y);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="name">Button's name</param>
    /// <returns>True during the frame the player pressed down the virtual button identified by buttonName</returns>
    public bool GetButtonDown(string name)
    {
        return Input.GetKeyDown(keyButtonsNames[name]) ||
            CheckState(name, prevGamepadState, ButtonState.Released) &&
            CheckState(name, currentGamepadState, ButtonState.Pressed);
    }

    /// <summary> 
    /// </summary>
    /// <param name="name">Virtual button's name</param>
    /// <returns>True the first frame the player releases the virtual button identified</returns>
    public bool GetButtonUp(string name)
    {
        return Input.GetKeyUp(keyButtonsNames[name]) ||
            CheckState(name, prevGamepadState, ButtonState.Pressed) &&
            CheckState(name, currentGamepadState, ButtonState.Released);
    }

    /// <summary> 
    /// </summary>
    /// <param name="name">Virtual button's name</param>
    /// <returns>True while the virtual button identified by buttonName is held down by the player.</returns>
    public bool GetButton(string name)
    {
        return Input.GetKey(keyButtonsNames[name]) ||
            CheckState(name, currentGamepadState, ButtonState.Pressed);
    }

    public Vector2 GetDpadAxis()
    {
        return new Vector2(GetDPadHorizontal(), GetDPadVertical());
    }

    public float GetDPadHorizontal()
    {
        if (currentGamepadState.DPad.Left == ButtonState.Pressed)
            return -1f;
        else if (currentGamepadState.DPad.Right == ButtonState.Pressed)
            return 1f;

        return 0f; 
    }

    public float GetDPadVertical()
    {
        if (currentGamepadState.DPad.Down == ButtonState.Pressed)
            return -1f;
        else if (currentGamepadState.DPad.Up == ButtonState.Pressed)
            return 1f;

        return 0f;
    }

    public Vector2 GetTriggers()
    {
        return new Vector2(GetLeftTrigger(), GetRightTrigger());
    }

    public float GetLeftTrigger()
    {
        return currentGamepadState.Triggers.Left;
    }

    public float GetRightTrigger()
    {
        return currentGamepadState.Triggers.Right;
    }

    public void SetVibration(float leftMotor, float rightMotor, float time)
    {
        GamePad.SetVibration(playerIndex, leftMotor, leftMotor);
        StartCoroutine(StopVibrationCoroutine(time));
    }

    public void SetVibration(float leftMotor, float rightMotor)
    {
        GamePad.SetVibration(playerIndex, leftMotor, rightMotor);
    }

    public void StopVibration()
    {
        GamePad.SetVibration(playerIndex, 0f, 0f);
    }

    private IEnumerator StopVibrationCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        StopVibration();
    }

    private bool CheckState(string buttonsName, GamePadState gamepadState, ButtonState buttonState)
    {
        switch (gamepadButtonsNames[buttonsName])
        {
            case GamePadButtonsName.A:
                return gamepadState.Buttons.A == buttonState;
            case GamePadButtonsName.X:
                return gamepadState.Buttons.X == buttonState;
            case GamePadButtonsName.Y:
                return gamepadState.Buttons.Y == buttonState;
            case GamePadButtonsName.B:
                return gamepadState.Buttons.B == buttonState;
            case GamePadButtonsName.Back:
                return gamepadState.Buttons.Back == buttonState;
            case GamePadButtonsName.Guide:
                return gamepadState.Buttons.Guide == buttonState;
            case GamePadButtonsName.Start:
                return gamepadState.Buttons.Start == buttonState;
            case GamePadButtonsName.LeftShoulder:
                return gamepadState.Buttons.LeftShoulder == buttonState;
            case GamePadButtonsName.RightShoulder:
                return gamepadState.Buttons.RightShoulder == buttonState;
            case GamePadButtonsName.LeftStick:
                return gamepadState.Buttons.LeftStick == buttonState;
            case GamePadButtonsName.RightStick:
                return gamepadState.Buttons.RightStick == buttonState;

            default:
                return false;
        }
    }

    private void OnDisable()
    {
        StopVibration();
    }
}

[Serializable]
public struct Axis
{
    public string VirtualHorizontal;
    public string VirtualVertical;

    public Vector2 Value;
}

[Serializable]
public class ButtonMappingName
{
    public string VirtualName;
    public string KeyName;
    public GamePadButtonsName GamePadButton;
}

public enum GamePadButtonsName
{
    A,
    X,
    Y,
    B,
    Back,
    Guide,
    Start,
    LeftShoulder,
    RightShoulder,
    LeftStick,
    RightStick
}

