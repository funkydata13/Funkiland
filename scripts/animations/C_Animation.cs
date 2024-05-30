using Godot;
using System;

public partial class C_Animation : Node
{
    #region Variables
    [Export]
    public AnimationPlayer player;
    [Export]
    public int playCount = 1;
    [Export]
    public float playSpeed = 1;
    [Export]
    public bool pingPong = false;

    protected string _animationName;
    protected bool _doingPong;
    protected int _counter;
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();

        if (player == null) GD.PushError("Pas d'AnimationPlayer lié au C_Animation");
        SetProcess(false);
        SetPhysicsProcess(false);
    }
    #endregion

    #region Functions
    public virtual bool Play(bool resetBefore)
    {
        if (resetBefore) Reset();
        if (_counter <= 0) return false;

        if (pingPong)
        {
            if (_doingPong == false)
            {
                _counter--;
                _doingPong = true;
                player.Play(_animationName, -1, -playSpeed, true);
                return true;
            }
            else
            {
                player.Play(_animationName, -1, playSpeed);
                return true;
            }
        }
        else
        {
            _counter--;
            player.Play(_animationName, -1, playSpeed);
            return true;
        }
    }

    public virtual void Reset()
    {
        if (player.IsPlaying()) player.Stop(false);
        _counter = playCount;
        _doingPong = false;
    }
    #endregion
}
