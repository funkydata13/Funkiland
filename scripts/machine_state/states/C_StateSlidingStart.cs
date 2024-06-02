using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateSlidingStart : C_State
{
    [Export]
    public float ledgeMinimalHeight = 30;
    [Export]
    public float obstacleDetectionDistance = 30;
    [Export]
    public float frictionFactorOnBreak = 4;
    public S_StateSlide slideBackup;
    protected C_KinematicSlide slideKinematic;

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        slideKinematic = machine.kinematics.GetChildKinematic<C_KinematicGround>(C_Kinematic.E_Type.Ground).GetChildKinematic<C_KinematicSlide>(C_Kinematic.E_Type.Slide);
    }
    #endregion

    #region Functions Overrides
    public override bool CanEnter()
    {
        return slideKinematic != null && machine.isFacingLedge == false && machine.isFacingObstacle == false;
    }

    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();

        slideBackup = new S_StateSlide() { 
            breakSlide = false, breakSlideFriction = frictionFactorOnBreak, 
            ledgeTargetPosition = machine.ledgeDetector.TargetPosition, obstacleTargetPosition = machine.obstacleDetector.TargetPosition 
        };

        machine.ledgeDetector.TargetPosition = new Vector2(slideBackup.ledgeTargetPosition.X, ledgeMinimalHeight);
        machine.obstacleDetector.TargetPosition = new Vector2(slideBackup.obstacleTargetPosition.X, obstacleDetectionDistance);
    }

    public override void CheckStatus(double delta)
    {
        if (IsOver()) machine.ChangeState("Sliding");
        else if (machine.isFacingLedge || machine.isFacingObstacle)
        {
            slideBackup.breakSlide = true;
            machine.ChangeState("Sliding");
        }
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        if (_trigger)
        {
            _trigger = false;
            slideKinematic.Update(delta);
        }
        
        base._PhysicsProcess(delta);
    }
    #endregion
}