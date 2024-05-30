using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_StateAttacking : C_State
{
    public enum E_ComboState { Pending, Failure, Success }

    [ExportGroup("Attack Properties")]
    [Export]
    public Area2D area;
    [Export]
    public float repelForce = 10;
    [Export]
    public int attackIndex = 1;
    [Export]
    public bool isComboLink = false;
    [Export]
    public int comboFrameTrigger = 0;

    [ExportGroup("Attributes Properties")]
    [Export]
    public C_Attribute.E_Type attributeConsumed = C_Attribute.E_Type.Stamina;
    [Export]
    public float attributeConsumption = 1;
    [Export]
    public C_Attribute.E_Type attributeDamaged = C_Attribute.E_Type.Vitality;
    [Export]
    public float attributeDamage = 1;

    protected C_Character _character;
    protected C_Attribute _consumedAttribute;
    protected E_ComboState _comboState;
    protected List<I_AttributeOwner> _targets;

    protected C_KinematicAir airKinematic;
    protected C_KinematicGround groundKinematic;

    public override void _Ready()
    {
        base._Ready();

        _animationNameOverride = string.Format("Attacking {0}", attackIndex);
        _character = Owner as C_Character;

        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
        groundKinematic = machine.kinematics.GetChildKinematic<C_KinematicGround>(C_Kinematic.E_Type.Ground);

        if (area == null) GD.PushError("Un état C_StateAttacking n'a pas de Area2D liée !");
        area.BodyEntered += OnBodyEnter;
        area.BodyExited += OnBodyExit;

        _targets = new List<I_AttributeOwner>();
    }

    protected void OnBodyEnter(Node2D body)
    {
        if (body is I_AttributeOwner) _targets.Add(body as I_AttributeOwner);
    }

    protected void OnBodyExit(Node2D body)
    {
        if (body is I_AttributeOwner) _targets.Remove(body as I_AttributeOwner);
    }

    public override bool CanEnter()
    {
        if (_consumedAttribute == null) _consumedAttribute = _character.attributes[attributeConsumed];
        return _consumedAttribute.currentValue >= attributeConsumption;
    }

    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();
        _comboState = machine.kinematics.isGrounded ? E_ComboState.Pending : E_ComboState.Failure;
    }

    public override void Exit()
    {
        base.Exit();

        if (_targets.Count > 0)
        {
            foreach(I_AttributeOwner ownner in _targets)
                ownner.attributes[attributeDamaged].ModifyValue(C_Attribute.E_Action.Damage, attributeDamage, _character, repelForce);
        }

        _consumedAttribute.ModifyValue(C_Attribute.E_Action.Lost, attributeConsumption, _character, 0);
    }

    public override void CheckStatus(double delta)
    {
        if (_comboState == E_ComboState.Pending && machine.CanReadInputs && C_Inputs.IsActionJustPressed("attack"))
            _comboState = machine.sprite.Frame >= comboFrameTrigger ? E_ComboState.Success : E_ComboState.Failure;

        if (IsOver())
        {
            if (_comboState == E_ComboState.Success && machine.ChangeState(string.Format("Attacking {0}", attackIndex + 1))) return;
            else machine.ChangeState("Idle");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (machine.kinematics.isGrounded) groundKinematic.update(delta);
        else airKinematic.update(delta);

        base._PhysicsProcess(delta);
    }
}
