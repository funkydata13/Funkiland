using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateHitted : C_State
{
    #region Variables
    [Export]
    public C_Attribute attribute;
    [Export]
    public C_Attribute.E_Action action;
    [Export]
    public float frictionFactor = 0.25f;

    protected C_Character _character;
    protected C_Character.E_State _previousCharacterState;
    protected Node2D _hitFrom;
    protected float _hitForce;

    protected C_KinematicAir airKinematic;
    protected C_KinematicGround groundKinematic;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();

        if (Owner is C_Character) _character = Owner as C_Character;
        if (attribute == null) GD.PushError("Pas de C_Attribute lié au C_StateHitted !");
        else attribute.Changed += OnAttributeChanged;
        
        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
        groundKinematic = machine.kinematics.GetChildKinematic<C_KinematicGround>(C_Kinematic.E_Type.Ground);
    }
    #endregion

    protected void OnAttributeChanged(int action, float changedAmount, float changedValue, Node2D changeBy, float changeDelta)
    {
        if ((int)this.action != action || changedValue <= 0) return;

        _hitFrom = changeBy;
        _hitForce = changeDelta;

        machine.ChangeState(Name, true);
    }

    #region Functions Overrides
    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();

        if (_character != null)
        {
            _previousCharacterState = _character.state;
            _character.state = C_Character.E_State.Stunned;
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (_character != null) _character.state = _previousCharacterState;
    }

    public override void CheckStatus(double delta)
    {
        if (CheckStopwatchStatus()) return;
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        if (_trigger)
        {
            machine.kinematics.Repel(_hitFrom, _hitForce, _hitForce);

            _trigger = false;
            _hitFrom = null;
            _hitForce = 0;
        }
        else
        {
            if (machine.kinematics.isGrounded) groundKinematic.Update(delta);
            else airKinematic.Update(delta);
        }

        base._PhysicsProcess(delta);
    }
    #endregion
}