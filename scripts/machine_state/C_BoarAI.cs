using Godot;

public partial class C_BoarAI : C_MachineState
{
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (isFacingLedge || isFacingObstacle) _character.Flip();
    }
}