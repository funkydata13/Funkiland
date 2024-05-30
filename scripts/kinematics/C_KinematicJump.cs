using Godot;

public partial class C_KinematicJump : C_Kinematic
{
    #region Variables
    [Export]
    public C_KinematicAir airKinematic;
    [Export]
    public bool applyLocalGravity = true;
    [Export]
    public float height = 50;
    [Export]
    public float timeToAscent = 0.4f;
    [Export]
    public float timeToDescent = 0.25f;
    [Export]
    public bool isImpulsion = false;
    #endregion

    #region Constructor
    public C_KinematicJump()
    {
        type = E_Type.Jump;
    }
    #endregion

    #region Godot Overrides
    public override void _Ready()
    {
        base._Ready();

        if (airKinematic == null) GD.PushError("Le C_KinematicAir n'est pas lié dans le C_KinematicJump");
    }
    #endregion

    #region Update
    public override void update(double delta, float factor = 1)
    {
        float jumpVelocity = 0;

        if (applyLocalGravity)
        {
            airKinematic.currentFallGravity = (-2.0f * height) / (timeToDescent * timeToDescent) * -1.0f * factor;
            airKinematic.currentRaiseGravity = (-2.0f * height) / (timeToAscent * timeToAscent) * -1.0f * factor;
            jumpVelocity = (2.0f * height) / timeToAscent * -1.0f * factor;
        }
        else jumpVelocity = -height;

        if (isImpulsion) character.Velocity = new Vector2(character.Velocity.X, character.Velocity.Y + jumpVelocity);
        else character.Velocity = new Vector2(character.Velocity.X, jumpVelocity);
    }
    #endregion
}
