using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

public partial class C_DestructibleBody : RigidBody2D, I_AttributeOwner
{
    protected C_Attributes _attributes;
    protected C_Attribute _vitalityAttribute;

    public C_Attributes attributes
    {
        get { return _attributes; }
    }

    protected C_SpriteAnimated _sprite;
    protected C_Inventory _inventory;

    public bool isDestroyed
    {
        get { return _vitalityAttribute.isDepleted; }
    }

    public override void _Ready()
    {
        base._Ready();

        _sprite = GetNode<C_SpriteAnimated>("Sprite");
        _inventory = GetNode<C_Inventory>("Inventory");
        _attributes = GetNode<C_Attributes>("Attributes");
        if (_attributes == null) GD.PushError("Il n'y a pas de C_Attributes pour l'objet destructible !");

        _vitalityAttribute = _attributes[C_Attribute.E_Type.Vitality];
        _vitalityAttribute.Changed += OnVitalityChanged;
        _vitalityAttribute.Depleted += OnVitalityDepleted;
    }

    protected virtual void OnVitalityChanged(int action, float changedAmount, float changedValue, Node2D changeBy, float changeDelta)
    { }

    protected virtual void OnVitalityDepleted(Node2D depletedBy)
    { }

    protected void DisablePhysics()
    {
        SetCollisionLayerValue(4, false);
        SetCollisionMaskValue(2, false);
        SetCollisionMaskValue(3, false);
        Freeze = true;
    }
}