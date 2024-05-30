using Godot;

public partial class C_Stopwatch : Node
{
    #region Variables
    [Export]
    public bool physicsProcessing = false;
    [Export]
    public bool fixedDurationMode = true;
    [Export]
    public float duration = 1;
    [Export]
    public float randomDurationRange = 2;

    protected float _timeLeft;
    #endregion

    #region Properties
    public bool isOver
    {
        get { return _timeLeft <= 0; }
    }
    #endregion

    #region Functions
    internal void SetProcessState(bool newState)
    {
        if (physicsProcessing) SetPhysicsProcess(newState);
        else SetProcess(newState);
    }

    public bool Start()
    {
        if (IsProcessing() || IsPhysicsProcessing()) return false;

        float time = duration;
        if (fixedDurationMode == false)
        {
            GD.Randomize();
            time += GD.Randf() * randomDurationRange;
        }

        _timeLeft = time;
        SetProcessState(true);
        return true;
    }

    public void Stop()
    {
        if (IsProcessing() || IsPhysicsProcessing())
        {
            _timeLeft = 0;
            SetProcessState(false);
        }
    }

    public void Update(float delta)
    {
        if (isOver) Stop();
        else _timeLeft -= delta;
    }
    #endregion

    #region Godot Overrides
    public override void _Ready()
    {
        base._Ready();
        SetProcess(false);
        SetPhysicsProcess(false);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Update((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Update((float)delta);
    }
    #endregion
}
