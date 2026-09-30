using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Central input facade. Prefers the Input System, falls back to legacy Input Manager
/// while Project Settings use Both.
/// </summary>
public static class GameInput
{
    private static bool _trampAxisHeld;

    public static Vector2 Move
    {
        get
        {
            Vector2 value = Vector2.zero;

            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) value.x -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) value.x += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) value.y -= 1f;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) value.y += 1f;
            }

            Gamepad pad = Gamepad.current;
            if (pad != null)
                value += pad.leftStick.ReadValue();

            if (value.sqrMagnitude < 0.0001f)
                value = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            return Vector2.ClampMagnitude(value, 1f);
        }
    }

    public static Vector2 Look
    {
        get
        {
            // Prefer legacy axes so existing sensitivity tuning in PlayerCam stays valid.
            Vector2 legacy = new Vector2(
                Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("Pad X"),
                Input.GetAxisRaw("Mouse Y") + Input.GetAxisRaw("Pad Y"));

            if (legacy.sqrMagnitude > 0.0001f)
                return legacy;

            Vector2 value = Vector2.zero;
            Mouse mouse = Mouse.current;
            if (mouse != null)
                value += mouse.delta.ReadValue() * 0.05f;

            Gamepad pad = Gamepad.current;
            if (pad != null)
                value += pad.rightStick.ReadValue();

            return value;
        }
    }

    public static bool PausePressedThisFrame
    {
        get
        {
            Gamepad pad = Gamepad.current;
            return WasPressed(Key.Escape) ||
                   (pad != null && pad.startButton.wasPressedThisFrame) ||
                   Input.GetButtonDown("Pause") ||
                   Input.GetButtonDown("Cancel");
        }
    }

    public static bool JumpPressedThisFrame
    {
        get
        {
            Gamepad pad = Gamepad.current;
            return WasPressed(Key.Space) ||
                   (pad != null && pad.buttonSouth.wasPressedThisFrame) ||
                   Input.GetButtonDown("A/Space");
        }
    }

    public static bool RestartHeld
    {
        get
        {
            Gamepad pad = Gamepad.current;
            return IsPressed(Key.R) ||
                   (pad != null && pad.selectButton.isPressed) ||
                   Input.GetButton("Restart");
        }
    }

    public static bool RestartPressedThisFrame
    {
        get
        {
            Gamepad pad = Gamepad.current;
            return WasPressed(Key.R) ||
                   (pad != null && pad.selectButton.wasPressedThisFrame) ||
                   Input.GetButtonDown("Restart");
        }
    }

    public static bool RestartReleasedThisFrame =>
        WasReleased(Key.R) ||
        Input.GetButtonUp("Restart");

    public static bool LoadSavePressedThisFrame =>
        WasPressed(Key.T) ||
        Input.GetButtonDown("LoadSave");

    public static bool ParasolHeld
    {
        get
        {
            Gamepad pad = Gamepad.current;
            return IsPressed(Key.Space) ||
                   (pad != null && pad.buttonSouth.isPressed) ||
                   Input.GetButton("Parasol") ||
                   Mathf.Abs(Input.GetAxis("Parasol")) > 0.1f;
        }
    }

    public static bool ThrowTrampPressedThisFrame
    {
        get
        {
            Gamepad pad = Gamepad.current;
            Mouse mouse = Mouse.current;

            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                return true;
            if (pad != null && pad.rightTrigger.wasPressedThisFrame)
                return true;
            if (Input.GetKeyDown(KeyCode.Mouse0))
                return true;

            bool axisHeld = Mathf.Abs(Input.GetAxisRaw("Tramp")) > 0.5f;
            bool pressed = axisHeld && !_trampAxisHeld;
            _trampAxisHeld = axisHeld;
            return pressed;
        }
    }

    public static bool ThrowBananaPressedThisFrame
    {
        get
        {
            Gamepad pad = Gamepad.current;
            Mouse mouse = Mouse.current;
            return (mouse != null && mouse.middleButton.wasPressedThisFrame) ||
                   (pad != null && pad.buttonWest.wasPressedThisFrame) ||
                   Input.GetButtonDown("Banana");
        }
    }

    public static bool ThrowGluePressedThisFrame
    {
        get
        {
            Gamepad pad = Gamepad.current;
            Mouse mouse = Mouse.current;
            return (mouse != null && mouse.rightButton.wasPressedThisFrame) ||
                   (pad != null && pad.buttonEast.wasPressedThisFrame) ||
                   Input.GetButtonDown("Glue");
        }
    }

    public static bool DevFlyTogglePressedThisFrame =>
        WasPressed(Key.PageUp) ||
        Input.GetKeyDown(KeyCode.PageUp);

    public static bool ClimbUpHeld =>
        IsPressed(Key.LeftShift) ||
        Input.GetKey(KeyCode.LeftShift);

    public static bool ClimbDownHeld =>
        IsPressed(Key.LeftCtrl) ||
        Input.GetKey(KeyCode.LeftControl);

    private static bool WasPressed(Key key)
    {
        Keyboard kb = Keyboard.current;
        return kb != null && kb[key].wasPressedThisFrame;
    }

    private static bool WasReleased(Key key)
    {
        Keyboard kb = Keyboard.current;
        return kb != null && kb[key].wasReleasedThisFrame;
    }

    private static bool IsPressed(Key key)
    {
        Keyboard kb = Keyboard.current;
        return kb != null && kb[key].isPressed;
    }
}
