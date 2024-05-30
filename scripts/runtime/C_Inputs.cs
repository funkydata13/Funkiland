using System.Diagnostics;
using Godot;

public partial class C_Inputs : Node
{
    public enum E_Listener { Player = 1, Camera = 2, UI = 4 }

    #region Static Variables
    public static E_Listener Listeners = E_Listener.Player | E_Listener.Camera | E_Listener.UI;
    private static E_Listener _lastListeners;

    public static Vector2 direction;
    public static Vector2 cameraDirection;
    public static Vector2 uiDirection;
    #endregion

    #region Static Properties
    public static bool IsActionPressed(string name)
    {
        return Input.IsActionPressed(name);
    }

    public static bool IsActionJustPressed(string name)
    {
        return Input.IsActionJustPressed(name);
    }

    public static bool IsActionReleased(string name)
    {
        return Input.IsActionJustReleased(name);
    }

    public static bool CanListenInputs(E_Listener listener)
    {
        return (Listeners & listener) == listener;
    }
    #endregion

    #region Static Functions
    public static void LockInputsTo(E_Listener listener)
    {
        _lastListeners = Listeners;
        Listeners = listener;
    }

    public static void UnlockInputs()
    {
        Listeners = _lastListeners;
    }
    #endregion

    #region Godot Overrides
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        Vector2 d = new Vector2(Input.GetAxis("left", "right"), Input.GetAxis("up", "down"));
        direction = CanListenInputs(E_Listener.Player) ? d : Vector2.Zero;
        uiDirection = CanListenInputs(E_Listener.UI) ? d : Vector2.Zero;
        cameraDirection = CanListenInputs(E_Listener.Camera) ? new Vector2(Input.GetAxis("camera_left", "camera_right"), Input.GetAxis("camera_up", "camera_down")) : Vector2.Zero;
    }
    #endregion
}
