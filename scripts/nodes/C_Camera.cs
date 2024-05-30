using Godot;

public partial class C_Camera : Camera2D
{
    #region Variables
    [Export]
    public bool isTrackingPlayer = true;
    [Export]
    public bool offsetWithPlayer = true;
    [Export]
    public Vector2 viewOffset = new Vector2(50, -30);

    protected Node2D _trackedNode;
    protected Vector2 _currentOffset;
    protected Vector2 _previousOffset;
    #endregion

    #region Ready
    public override void _Ready()
    {
        SetProcess(false);
        SetPhysicsProcess(true);
    }
    #endregion

    #region Functions
    public void Track(Node2D target)
    {
        if (IsInstanceValid(target) == false) return;
        _trackedNode = target;
        _currentOffset = _previousOffset = viewOffset;
    }

    protected void Update()
    {
        bool updateFlag = false;

        if (C_Inputs.cameraDirection.X != 0)
        {
            _currentOffset.X = C_Inputs.cameraDirection.X > 0 ? viewOffset.X : -viewOffset.X;
            updateFlag = true;
        }

        if (updateFlag == false && offsetWithPlayer && IsInstanceValid(C_Game.Player) && C_Inputs.direction.X != 0)
        {
            _currentOffset.X = C_Inputs.direction.X > 0 ? viewOffset.X : -viewOffset.X;
            _previousOffset = _currentOffset;
            updateFlag = true;
        }

        if (updateFlag == false) _currentOffset.X = _previousOffset.X;
        if (C_Inputs.cameraDirection.Y != 0) _currentOffset.Y = C_Inputs.cameraDirection.Y > 0 ? -viewOffset.Y * 2.0f : viewOffset.Y * 2.0f;
        else _currentOffset.Y = viewOffset.Y;
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (IsInstanceValid(_trackedNode) == false && isTrackingPlayer && IsInstanceValid(C_Game.Player)) Track(C_Game.Player);
        if (IsInstanceValid(_trackedNode))
        {
            Update();
            GlobalPosition = _trackedNode.GlobalPosition + _currentOffset;
        }
    }
    #endregion
}