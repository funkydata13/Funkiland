using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class C_Attribute : Node
{
    #region Signals
    [Signal]
    public delegate void ChangedEventHandler(int action, float changedAmount, float changedValue, Node2D changeBy, float changeDelta);
    [Signal]
    public delegate void DepletedEventHandler(Node2D depletedBy);
    #endregion

    #region Enums
    public enum E_Action { Lost = 1, Damage = 2, DOT = 4, Receive = 8, Heal = 16, Regen = 32, Transfer = 64 }
    public enum E_Type { Vitality = 0, Stamina, Magic }
    #endregion
    
    #region Variables
    [Export]
    public E_Type type = E_Type.Vitality;
    [Export]
    public float currentValue = 20;
    public float previousValue;
    [Export]
    public float maximumValue = 20;
    [Export]
    public float regenerationRate = 0;
    [Export]
    public bool regenerationWhenDepleted = true;
    [Export]
    public float regenerationSignalsInterval = 0.25f;
    protected float _signalsTimeLeft;
    protected Dictionary<E_Action, List<C_AnimationAttribute>> _animations;
    #endregion

    #region Properties
    public bool isDepleted
    {
        get { return currentValue <= 0; }
    }
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();

        _animations = new Dictionary<E_Action, List<C_AnimationAttribute>>();
        foreach (Node node in GetChildren())
        {
            if (node is C_AnimationAttribute)
            {
                C_AnimationAttribute attributeNode = node as C_AnimationAttribute;
                if (_animations.ContainsKey(attributeNode.triggerOn) == false) _animations.Add(attributeNode.triggerOn, new List<C_AnimationAttribute>());
                _animations[attributeNode.triggerOn].Add(attributeNode);
            }
        }
    }
    #endregion

    #region Functions
    public void SetCurrentValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0, maximumValue);
    }

    public void SetMaximumValue(float value)
    {
        if (maximumValue != value)
        {
            maximumValue = value;
            currentValue = Mathf.Clamp(currentValue, 0, maximumValue);
        }
    }

    protected void PlayAnimations(E_Action action)
    {
        if (_animations.ContainsKey(action) == false) return;
        foreach (C_AnimationAttribute node in _animations[action])
            node.Play(true);
    }

    public void ModifyValue(E_Action action, float amout, Node2D actor, float delta)
    {
        if (amout == 0) return;

        previousValue = currentValue;
        if ((int)action <= 4) SetCurrentValue(currentValue - amout);
        else SetCurrentValue(currentValue + amout);

        if (currentValue != previousValue)
        {
            EmitSignal(SignalName.Changed, (int)action, currentValue - previousValue, currentValue, actor, delta);
            if (currentValue == 0)
                EmitSignal(SignalName.Depleted, actor);
        }

        PlayAnimations(action);
        previousValue = currentValue;
    }

    public void Regenerate(float delta)
    {
        if (regenerationRate == 0 || currentValue == maximumValue || (currentValue == 0 && regenerationWhenDepleted == false)) return;

        previousValue = currentValue;
        SetCurrentValue(currentValue + (regenerationRate * delta));

        if (currentValue != previousValue)
        {
            if (_signalsTimeLeft <= 0)
            {
                _signalsTimeLeft = regenerationSignalsInterval;
                EmitSignal(SignalName.Changed, (int)E_Action.Regen, currentValue - previousValue, currentValue, Owner as Node2D, 0);
            }
            else _signalsTimeLeft -= delta;
        }
        else
        {
            if (_signalsTimeLeft != regenerationSignalsInterval)
            {
                _signalsTimeLeft = regenerationSignalsInterval;
                EmitSignal(SignalName.Changed, (int)E_Action.Regen, 0, currentValue, Owner as Node2D, 0);
            }
        }

        previousValue = currentValue;
    }
    #endregion

    #region Process
    public override void _Process(double delta)
    {
        base._Process(delta);
        Regenerate((float)delta);
    }
    #endregion
}
