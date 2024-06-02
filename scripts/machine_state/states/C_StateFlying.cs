using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateFlying : C_State
{
    #region Variables
    [Export]
    public string animationNameOverride = string.Empty;
    protected C_KinematicAir airKinematic;
    protected C_KinematicMove moveKinematic;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
        moveKinematic = airKinematic.GetChildKinematic<C_KinematicMove>(C_Kinematic.E_Type.Move);
    }
    #endregion

    #region Functions
    protected void SelectSpriteAnimation()
    {
        if (animationNameOverride == string.Empty) SetSpriteAnimation(machine.kinematics.isFalling ? "Flying Down" : "Flying Up");
        else SetSpriteAnimation(animationNameOverride);
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
        SelectSpriteAnimation();
        airKinematic.peekHeight = float.MaxValue;
    }

    public override void CheckStatus(double delta)
    {
        if (machine.kinematics.isGrounded) 
        {
            airKinematic.ResetGravity();
            machine.ChangeState(airKinematic.isHighFall ? "Landing" : "Idle"); 
            airKinematic.peekHeight = 0;
            return; 
        }

        if (machine.isFacingObstacle && machine.kinematics.isFalling && machine.obstacleClass == "TileMap")
        {
            if (machine.ChangeState("Wall Contact"))
            {
                airKinematic.ResetGravity();
                return;
            }
        }

        if (machine.CanReadInputs)
        {
            if (C_Inputs.IsActionJustPressed("jump") && machine.ChangeState("Flying Jump")) return;
            if (C_Inputs.IsActionJustPressed("attack") && machine.ChangeState("Attacking 1")) return;
        }
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        if (animationNameOverride == string.Empty) SelectSpriteAnimation();

        airKinematic.Update(delta);
        if (moveKinematic != null && machine.CanReadInputs && C_Inputs.direction.X != 0) moveKinematic.Update(delta);

        base._PhysicsProcess(delta);
    }
    #endregion
}