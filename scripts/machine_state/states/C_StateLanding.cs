using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateLanding : C_State
{
    #region Variables
    [Export]
    public C_Attribute damagedAttribute;
    [Export]
    public float frictionFactor = 2;
    protected C_KinematicAir airKinematic;
    protected C_KinematicGround groundKinematic;
    protected float _damage = 0;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
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

        if (damagedAttribute != null) _damage = damagedAttribute.maximumValue * (airKinematic.fallHeight - airKinematic.maximumFallHeight) / airKinematic.maximumFallHeight;
        else _damage = 0;
    }

    public override void CheckStatus(double delta)
    {
        if (IsOver()) machine.ChangeState("Idle");
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        if (_trigger && _damage > 0)
        {
            _trigger = false;
            damagedAttribute.ModifyValue(C_Attribute.E_Action.Damage, _damage, null, 0);
        }

        groundKinematic.update(delta);
        base._PhysicsProcess(delta);
    }
    #endregion
}