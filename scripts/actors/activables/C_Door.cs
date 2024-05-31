using Godot;
using System;
using System.Diagnostics;

[Tool]
public partial class C_Door : C_ActivableBody
{
    [Export]
    public Vector2 activatedPositionOffset;
    [Export]
    public float transitionDuration = 3;

    protected bool _flip;
    [Export]
    public bool flip
    {
        get { return _flip;}
        set { if (_flip != value) { _flip = value; UpdateSprite(); } }
    }

    protected float _timeLeft;
    protected Vector2 _positionBackup;
    protected Vector2 _targetPosition;

    public override void _Ready()
    {
        base._Ready();
        _targetPosition = GlobalPosition + activatedPositionOffset;
    }

    protected override void UpdateSprite()
    {
        base.UpdateSprite();
        if (_activable.sprite != null) _activable.sprite.FlipH = _flip;
    }

    public override bool CanActivate(Node2D activator)
    {
        return state == C_Activable.E_State.Pending || state == C_Activable.E_State.Activated;
    }

    public override void Activate()
    {
        base.Activate();
        
        if (state == C_Activable.E_State.Activating) _positionBackup = GlobalPosition;
        _timeLeft = transitionDuration;    
        SetPhysicsProcess(true);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (state == C_Activable.E_State.Activating)
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= (float)delta;
                GlobalPosition = C_Utils.LerpVector(_positionBackup, _targetPosition, 1.0f - (_timeLeft / transitionDuration));
            }
            else
            {
                _timeLeft = 0;

                GlobalPosition = _targetPosition;
                state = C_Activable.E_State.Activated;
                SetPhysicsProcess(false);
            }
        }
        else if (state == C_Activable.E_State.Desactivating)
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= (float)delta;
                GlobalPosition = C_Utils.LerpVector(_targetPosition, _positionBackup, 1.0f - (_timeLeft / transitionDuration));
            }
            else
            {
                _timeLeft = 0;

                GlobalPosition = _positionBackup;
                state = C_Activable.E_State.Pending;
                SetPhysicsProcess(false);
            }
        }
    }
}
