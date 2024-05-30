using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateFlyingJump : C_State
{
    #region Variables
    protected C_KinematicAir airKinematic;
    protected C_KinematicJump jumpKinematic;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
        jumpKinematic = airKinematic.GetChildKinematic<C_KinematicJump>(C_Kinematic.E_Type.Jump);
    }
    #endregion
    
    #region Functions Overrides
    public override bool CanEnter()
    {
        return jumpKinematic != null && machine.kinematics.isFalling == false && machine.previousState.Name == "Jumping";
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
            jumpKinematic.update(delta);
        }
        
        base._PhysicsProcess(delta);
    }
    #endregion
}