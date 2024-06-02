using Godot;
using System;
using System.Diagnostics;

public partial class C_StateWallSliding : C_State
{
    [Export]
    public C_StateWallContact wallContactState;
    protected C_KinematicAir airKinematic;

    #region Ready
    public override void _Ready()
    {
        base._Ready();

        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
        if (wallContactState == null) GD.PushError("Le C_StateWallSliding n'a pas de C_StateWallContact lié !");
    }
    #endregion

    #region Functions Overrides
    public override bool CanEnter()
    {
        return airKinematic != null;
    }

    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();
    }

    public override void Exit()
    {
        base.Exit();
        wallContactState.UndoState();
    }

    public override void CheckStatus(double delta)
    {
        if (machine.kinematics.isGrounded)
        {
            machine.ChangeState("Idle");
            return;
        }
        else
        {
            if (machine.isFacingObstacle == false || machine.obstacleClass != "TileMap")
            {
                machine.ChangeState("Flying");
                return;
            }
        }

        if (machine.CanReadInputs)
        {
            if (C_Inputs.IsActionJustPressed("jump")) machine.ChangeState("Wall Jumping");
        }
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        airKinematic.Update(delta);
        base._PhysicsProcess(delta);
    }
    #endregion
}