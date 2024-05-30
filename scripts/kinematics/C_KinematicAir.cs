using Godot;

public partial class C_KinematicAir : C_Kinematic
{
    #region Variables
    [Export]
    public float baseGravity = 980;
    [Export]
    public float maximumSpeed = 500;
    [Export]
    public float maximumFallHeight = 120;
    public float peekHeight = float.MaxValue;

    public float currentFallGravity;
    public float currentRaiseGravity;
    #endregion

    #region Properties
    public float gravity
    {
        get { return isFalling ? currentFallGravity : currentRaiseGravity; }
    }

    public bool isHighFall
    {
        get { return Mathf.Abs(character.GlobalPosition.Y - peekHeight) > maximumFallHeight; }
    }
    #endregion

    #region Constructor
    public C_KinematicAir()
    {
        type = E_Type.Air;
    }
    #endregion

    #region Godot Overrides
    public override void _Ready()
    {
        base._Ready();
        ResetGravity();
    }
    #endregion

    #region Function & Update
    public void ResetGravity()
    {
        currentFallGravity = currentRaiseGravity = baseGravity;
    }

    public override void update(double delta, float factor = 1)
    {
        if (peekHeight > character.Position.Y) peekHeight = character.GlobalPosition.Y;
        character.Velocity = new Vector2(character.Velocity.X, Mathf.Clamp(character.Velocity.Y + (gravity * factor * (float)delta), -maximumSpeed, maximumSpeed));
    }
    #endregion
}