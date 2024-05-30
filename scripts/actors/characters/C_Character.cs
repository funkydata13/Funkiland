using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Schema;

public partial class C_Character : CharacterBody2D, I_AttributeOwner
{
    [Signal]
    public delegate void DefeatedEventHandler();

    public enum E_State { Free, Rooted, Stunned, Freezed, Dead }

    #region Variables
    public E_State _state = E_State.Free;
    protected C_Inventory _inventory;
    protected C_Attributes _attributes;
    private bool _isFacingRight = true;
    #endregion

    #region Properties
    public virtual E_State state
    {
        get { return _state; }
        set { _state = value; }
    }

    public bool isFacingRight
    {
        get { return _isFacingRight; }
        set 
        {
            if (_isFacingRight != value && (int)_state <= 1) { _isFacingRight = value; Scale *= new Vector2(-1, 1); }
        }
    }

    public float forwardDirection
    {
        get { return _isFacingRight ? 1 : -1; }
    }

    public C_Inventory inventory
    {
        get { return _inventory; }
    }

    public C_Attributes attributes
    {
        get { return _attributes; }
    }
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        
        _inventory = GetNode<C_Inventory>("Inventory");
        _attributes = GetNode<C_Attributes>("Attributes");
    }
    #endregion

    #region Functions
    public void Flip()
    {
        isFacingRight = !isFacingRight;
    }

    public void TurnTo(Vector2 position, bool invertDirection = false)
    {
        if (invertDirection == false)
            if ((GlobalPosition.X > position.X && _isFacingRight) || (GlobalPosition.X < position.X && _isFacingRight == false)) Flip();
        else
            if ((GlobalPosition.X > position.X && _isFacingRight == false) || (GlobalPosition.X < position.X && _isFacingRight)) Flip();
    }

    public virtual void Terminate()
    {
        EmitSignal(SignalName.Defeated);
        QueueFree();
    }
    #endregion

    #region Godot Overrides
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        MoveAndSlide();
    }
    #endregion
}