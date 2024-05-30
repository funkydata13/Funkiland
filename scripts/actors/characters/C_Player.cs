using Godot;

public partial class C_Player : C_Character
{
    #region Ready
    public override void _Ready()
    {
        base._Ready();
        C_Game.Player = this;
    }
    #endregion

    #region Functions
    public override void Terminate()
    {
        EmitSignal(SignalName.Defeated);
    }
    #endregion

    #region Process
    public override void _PhysicsProcess(double delta)
    {
        if (C_Inputs.CanListenInputs(C_Inputs.E_Listener.Player) && C_Inputs.direction.X * forwardDirection < 0) Flip();
        base._PhysicsProcess(delta);
    }
    #endregion
}
