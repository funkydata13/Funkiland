using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateSliding : C_State
{
    #region Variables
    [Export]
    public C_StateSlidingStart slidingStartState;
    protected C_KinematicAir airKinematic;
    protected C_KinematicGround groundKinematic;
    protected C_KinematicMove runKinematic;
    protected C_KinematicSlide slideKinematic;
    protected float _frictionFactorDelta;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
        groundKinematic = machine.kinematics.GetChildKinematic<C_KinematicGround>(C_Kinematic.E_Type.Ground);
        runKinematic = groundKinematic.GetChildKinematic<C_KinematicMove>(C_Kinematic.E_Type.Run);
        slideKinematic = groundKinematic.GetChildKinematic<C_KinematicSlide>(C_Kinematic.E_Type.Slide);

        if (slidingStartState == null) GD.PushError("Le C_StateSliding n'a pas de C_StateSlidingStart attaché !");
    }
    #endregion

    #region Functions Overrides
    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();

        _frictionFactorDelta = slidingStartState.stateSlide.stateBreakSlide ? slidingStartState.stateSlide.stateBreakSlideFriction : 
            slideKinematic.movementFriction / groundKinematic.movementFriction;
    }

    public override void CheckStatus(double delta)
    {
        if (machine.kinematics.isDirectionJustChanged || machine.kinematics.isGrounded == false || slideKinematic.isBreak)
        {
            machine.ChangeState("Sliding End");
            return;
        }

        if (machine.CanReadInputs)
        {
            if (C_Inputs.direction.Y > 0 || (C_Inputs.direction.X != 0 && Mathf.Abs(machine.kinematics.velocity.X) <= runKinematic.speed))
            {
                machine.ChangeState("Sliding End");
            }
        }
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        if (machine.isFacingLedge || machine.isFacingObstacle)
        {
            slidingStartState.stateSlide.stateBreakSlide = true;
            _frictionFactorDelta = slidingStartState.stateSlide.stateBreakSlideFriction;
        }

        if (machine.kinematics.isGrounded) groundKinematic.Update(delta, _frictionFactorDelta);
        else airKinematic.Update(delta);

        base._PhysicsProcess(delta);
    }
    #endregion
}