using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_State : Node
{
    #region Variables
    public C_MachineState machine;
    public C_StateStopwatch stopwatch;

    protected bool _trigger;
    protected bool _isAnimationLoops;
    protected bool _isAnimationPlaysBackward;
    protected int _animationFramesCount;
    protected string _animationNameOverride = string.Empty;
    #endregion

    #region Functions
    public void SetSpriteAnimation(bool playBackwards = false)
    {
        SetSpriteAnimation(_animationNameOverride == string.Empty ? Name : _animationNameOverride, playBackwards);
    }

    public void SetSpriteAnimation(string animationName, bool playBackwards = false)
    {
        if (machine.sprite.Animation == animationName) return;

        if (playBackwards) machine.sprite.PlayBackwards(animationName);
        else machine.sprite.Play(animationName);

        _isAnimationPlaysBackward = playBackwards;
        _isAnimationLoops = machine.sprite.SpriteFrames.GetAnimationLoop(animationName);
        _animationFramesCount = machine.sprite.SpriteFrames.GetFrameCount(animationName);
    }

    public bool IsPlayingLastFrame()
    {
        if (machine.sprite.IsPlaying() == false) return true;
        if (_isAnimationPlaysBackward) return machine.sprite.Frame == 0;
        else return machine.sprite.Frame == _animationFramesCount - 1;
    }

    public bool IsOver()
    {
        return stopwatch != null ? (stopwatch.isOver && IsPlayingLastFrame()) : IsPlayingLastFrame();
    }
    #endregion

    #region Virtual Functions
    public virtual bool CanEnter()
    {
        return true;
    }

    public virtual void Enter()
    {
        _trigger = true;
        
        if (stopwatch != null) if (stopwatch.Start() == false) Debug.Print("Le chronomètre de l'état ", Name, " ne s'est pas lancé lors de l'entrée dans l'état");
        SetPhysicsProcess(true);
    }

    public virtual void Exit()
    {
        _trigger = false;

        if (stopwatch != null) stopwatch.Stop();
        SetPhysicsProcess(false);
    }

    public virtual bool CheckStopwatchStatus()
    {
        if (stopwatch != null && stopwatch.isOver && stopwatch.nextState != string.Empty) return machine.ChangeState(stopwatch.nextState);
        else return false;
    }

    public virtual void CheckStatus(double delta) { }
    #endregion

    #region Godot Overrides
    public override void _Ready()
    {
        base._Ready();

        machine = GetParent<C_MachineState>();
        foreach (C_StateStopwatch node in GetChildren()) { stopwatch = node; break; }

        SetProcess(false);
        SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        CheckStatus(delta);
    }
    #endregion
}