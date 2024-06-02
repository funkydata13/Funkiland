using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateSlidingEnd : C_State
{
    #region Variables
    [Export]
    public C_StateSlidingStart slidingStartState;
    protected C_KinematicAir airKinematic;
    protected C_KinematicGround groundKinematic;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
        groundKinematic = machine.kinematics.GetChildKinematic<C_KinematicGround>(C_Kinematic.E_Type.Ground);

        if (slidingStartState == null) GD.PushError("Le C_StateSlidingEnd n'a pas de C_StateSlidingStart attaché !");
    }
    #endregion

    #region Functions Overrides
    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();
    }

    public override void Exit()
    {
        base.Exit();

        machine.ledgeDetector.TargetPosition = slidingStartState.stateSlide.ledgeTargetPositionBackup;
        machine.obstacleDetector.TargetPosition = slidingStartState.stateSlide.obstacleTargetPositionBackup;
    }

    public override void CheckStatus(double delta)
    {
        if (IsOver()) machine.ChangeState(machine.kinematics.isGrounded ? "Idle" : "Flying");
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        if (slidingStartState.stateSlide.stateBreakSlide)
        {
            if (machine.kinematics.isGrounded) groundKinematic.Update(delta, slidingStartState.stateSlide.stateBreakSlideFriction);
            else airKinematic.Update(delta);
        }

        base._PhysicsProcess(delta);
    }
    #endregion
}