using Godot;
using System;
using System.Diagnostics;

[Tool]
public partial class C_Rune : C_ActivableArea
{
    protected int _variant = 1;
    [Export]
    public int variant
    {
        get { return _variant; }
        set 
        { 
            if (_variant != Math.Clamp(value, 1, 26))
            { 
                _variant =  Math.Clamp(value, 1, 26);
                UpdateSprite();
            } 
        }
    }

    [Export]
    public Color pendingColor = new Color(0.3f, 0.3f, 0.3f);
    [Export]
    public Color activatedColor = new Color(1, 1, 1);
    [Export]
    public float transitionDuration = 1;

    protected float _timeLeft;

    public override void _Ready()
    {
        base._Ready();
        SetPhysicsProcess(false);
    }

    protected override void UpdateSprite()
    {
        if (_activable.sprite == null) return;
        _activable.sprite.Animation = "runes";
        _activable.sprite.Frame = variant - 1;
        _activable.sprite.SelfModulate = state == C_Activable.E_State.Activated ? activatedColor : pendingColor;
    }

    public override bool CanActivate(Node2D activator)
    {
        Debug.Print("Process " + (IsPhysicsProcessing() ? "true" : "false"));
        return IsPhysicsProcessing() == false;
    }

    public override void Activate()
    {
        base.Activate();
        
        _timeLeft = transitionDuration;
        SetPhysicsProcess(true);
    }

    public override void Reset()
    {
        base.Reset();

        _timeLeft = 0;
        SetPhysicsProcess(true);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (state == C_Activable.E_State.Activating)
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= (float)delta;
                _activable.sprite.SelfModulate = C_Utils.LerpColor(pendingColor, activatedColor, 1.0f - (_timeLeft / transitionDuration));
            }
            else
            {
                _timeLeft = 0;
                _activable.sprite.SelfModulate = activatedColor;

                Monitorable = false;
                state = C_Activable.E_State.Activated;
                SetPhysicsProcess(false);
            }
        }
        else if (state == C_Activable.E_State.Desactivating)
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= (float)delta;
                _activable.sprite.SelfModulate = C_Utils.LerpColor(activatedColor, pendingColor, 1.0f - (_timeLeft / transitionDuration));
            }
            else
            {
                _timeLeft = 0;
                _activable.sprite.SelfModulate = pendingColor;
                state = C_Activable.E_State.Pending;
                SetPhysicsProcess(false);
            }
        }
    }
}
