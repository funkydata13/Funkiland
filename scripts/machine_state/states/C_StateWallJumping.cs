using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateWallJumping : C_State
{
    #region Variables
    [Export]
    public float jumpFactor = 1;
    [Export]
    public float lateralImpulse = 150;
    protected C_KinematicJump jumpKinematic;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        jumpKinematic = machine.kinematics.GetChildKinematic<C_KinematicGround>(C_Kinematic.E_Type.Ground).GetChildKinematic<C_KinematicJump>(C_Kinematic.E_Type.Jump);
    }
    #endregion

    #region Functions Overrides
    public override bool CanEnter()
    {
        return jumpKinematic != null;
    }

    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();
    }

    public override void CheckStatus(double delta)
    {
        if (IsOver()) machine.ChangeState("Flying");
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        if (_trigger)
        {
            _trigger = false;
            machine.kinematics.Push(lateralImpulse, false, true);
            jumpKinematic.Update(delta);
        }
        
        base._PhysicsProcess(delta);
    }
    #endregion
}