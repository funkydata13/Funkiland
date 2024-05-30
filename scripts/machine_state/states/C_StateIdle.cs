using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateIdle : C_State
{
    #region Variables
    protected C_KinematicGround groundKinematic;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        groundKinematic = machine.kinematics.GetChildKinematic<C_KinematicGround>(C_Kinematic.E_Type.Ground);
    }
    #endregion

    #region Functions Overrides
    public override bool CanEnter()
    {
        return groundKinematic != null;
    }

    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();
    }

    public override void CheckStatus(double delta)
    {
        if (CheckStopwatchStatus()) return;
        if (machine.kinematics.isGrounded == false) { machine.ChangeState("Flying"); return; }

        if (machine.CanReadInputs)
        {
            if (C_Inputs.IsActionJustPressed("jump") && machine.ChangeState("Jumping")) return;
            if (C_Inputs.IsActionJustPressed("attack") && machine.ChangeState("Attacking 1")) return;
            if (C_Inputs.direction.X != 0)
            {
                if (C_Inputs.IsActionPressed("walk") || Mathf.Abs(C_Inputs.direction.X) <= 0.5f) machine.ChangeState("Walking");
                else machine.ChangeState("Running");
            }
        }
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        groundKinematic.update(delta);
        base._PhysicsProcess(delta);
    }
    #endregion
}