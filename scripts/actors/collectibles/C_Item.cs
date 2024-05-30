using Godot;
using System;
using System.Diagnostics;

public partial class C_Item : RigidBody2D, I_Item
{
    protected const float VELOCITY_FACTOR = 200.0f;
    protected const float ANIMATION_SPEED = 2.0f;

    public enum E_State { Idle, Dropping, Pending, Picking };
    public enum E_Type { Key, Coin, Vitality, Stamina, Magic, Vial }

    [Export]
    public E_State startState = E_State.Idle;
    protected E_Type _type;
    protected E_State _state;

    public C_Inventory inventory;
    public Sprite2D sprite;
    public AnimationPlayer animationPlayer;

    public virtual E_Type type
    {
        get { return _type; }
    }

    public virtual E_State state
    {
        get { return _state; }
    }

    public override void _Ready()
    {
        base._Ready();

        _state = startState;

        sprite = GetNode<Sprite2D>("Sprite");
        animationPlayer = GetNode<AnimationPlayer>("Sprite/Anim Player");
    }

    public void Drop(Vector2 position)
    {
        if (_state != E_State.Idle) return;

        GD.Randomize();
        GlobalPosition = position;
        LinearVelocity = new Vector2((float)GD.RandRange(-0.25, 0.25), -1) * VELOCITY_FACTOR;
        animationPlayer.Play("sprite_animations/effect_scale_pop", -1, -ANIMATION_SPEED, true);

        _state = E_State.Dropping;

        Visible = true;
    }

    public void Pick(Vector2 position, C_Inventory targetInventory = null)
    {
        if (state != E_State.Pending) return;

        inventory = targetInventory;
        LinearVelocity = new Vector2(0, -1) * VELOCITY_FACTOR;
        animationPlayer.Play("sprite_animations/effect_scale_pop", -1, 2);

        _state = E_State.Picking;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (animationPlayer != null && animationPlayer.IsPlaying() == false)
        {
            if (_state == E_State.Dropping) 
            {
                _state = E_State.Pending;
            }
            else if (_state == E_State.Picking)
            {
                if (inventory != null) 
                {
                    inventory.Append(type, 1);
                }
                QueueFree();
            }
        }
    }
}
