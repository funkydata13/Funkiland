using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Godot;

public partial class C_ActivableBody : RigidBody2D, I_Activable
{
    [Export]
    public Node2D[] activationChilds = new Node2D[0];
    protected C_Activable _activable;

    public C_Activable.E_State state
    {
        get { return _activable.state; }
        set { if (_activable.state != value) { _activable.state = value; UpdateSprite(); } }
    }

    public bool isActivating 
    { 
        get { return _activable.isActivating; } 
    }
    public bool isActivated
    { 
        get { return _activable.isActivated; } 
    }
    public int childsCount
    { 
        get { return _activable.childsCount; } 
    }

    public C_ActivableBody()
    {
        _activable = new C_Activable();
    }

    public override void _Ready()
    {
        base._Ready();

        if (_activable == null) _activable = new C_Activable();
        _activable.sprite = GetNode<AnimatedSprite2D>("Sprite");
        _activable.TransferArray(activationChilds);

        UpdateSprite();
    }

    protected virtual void UpdateSprite() { }

    public I_Activable GetActivationChild(int index)
    {
        return _activable.GetChild(index);
    }

    public void SetActivationChild(I_Activable activable)
    {
        _activable.SetChild(activable);
    }

    public virtual bool CanActivate(Node2D activator)
    {
        return true;
    }

    public virtual void Activate()
    {
        if (state == C_Activable.E_State.Pending)
            state  = C_Activable.E_State.Activating;
        else if (state == C_Activable.E_State.Activated)
            state  = C_Activable.E_State.Desactivating;
            
        _activable.Activate();
    }

    public virtual void Reset()
    {
        _activable.Reset();
    }
}