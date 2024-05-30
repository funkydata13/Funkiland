using Godot;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

public partial class C_AttributeBar : Control
{
    [Export]
    public C_Attribute attribute;
    [Export]
    public float showDuration = 2;
    protected bool _isDisabled = false;
    protected TextureProgressBar _bar;
    protected float _timeLeft = 0;

    public bool isDisabled
    {
        get { return _isDisabled; }
        set 
        {
            if (_isDisabled == value) return;
            
            _isDisabled = value;
            if (_isDisabled)
            {
                Visible = false;
                SetProcess(false);
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();

        if (attribute == null) GD.PushError("La C_AttributeBar n'a pas d'attibut attaché !");
        attribute.Changed += OnAttributeChanged;

        _bar = GetNode<TextureProgressBar>("Bar");
        _bar.MaxValue = attribute.maximumValue;
        _bar.Step = 1.0f / attribute.maximumValue;
        _bar.Value = attribute.currentValue;
        _bar.TintProgress = C_Assets.GetAttributeColor(attribute.type);

        _timeLeft = showDuration;

        if (showDuration == 0) SetProcess(false);
        SetPhysicsProcess(false);
    }

    protected void OnAttributeChanged(int action, float changedAmount, float changedValue, Node2D changeBy, float changeDelta)
    {
        if (_bar.MaxValue != attribute.maximumValue)
        {
            _bar.MaxValue = attribute.maximumValue;
            _bar.Step = 1.0f / attribute.maximumValue;
        }

        _bar.Value = attribute.currentValue;
        _timeLeft = showDuration;

        if (Visible == false && isDisabled == false) Visible = true;
        if (showDuration > 0 && IsProcessing() == false) SetProcess(true);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_timeLeft > 0) 
        {
            _timeLeft -= (float)delta;
        }
        else
        {
            Visible = false;
            SetProcess(false);
        }
    }
}
