using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateDeath : C_State
{
    #region Variables
    [Export]
    public C_Attribute attribute;
    [Export]
    public float frictionFactor = 5;

    protected C_Character _character;
    protected Node2D _deathFrom;

    protected C_KinematicAir airKinematic;
    protected C_KinematicGround groundKinematic;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();

        if (Owner is C_Character) _character = Owner as C_Character;
        if (attribute == null) GD.PushError("Pas de C_Attribute lié au C_StateDeath !");
        else attribute.Depleted += OnAttributeDepleted;
        
        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
        groundKinematic = machine.kinematics.GetChildKinematic<C_KinematicGround>(C_Kinematic.E_Type.Ground);
    }
    #endregion

    protected void OnAttributeDepleted(Node2D depletedBy)
    {
        _deathFrom = depletedBy;
        machine.ChangeState(Name, true);
    }

    #region Functions Overrides
    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();

        if (_character != null) _character.state = C_Character.E_State.Dead;
    }

    public override void CheckStatus(double delta)
    {
        if (IsOver() && _trigger)
        {
            _trigger = false;
            _character.Terminate();
        }
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        if (machine.kinematics.isGrounded) groundKinematic.Update(delta, frictionFactor);
        else airKinematic.Update(delta, frictionFactor);

        base._PhysicsProcess(delta);
    }
    #endregion
}