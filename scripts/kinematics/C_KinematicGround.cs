using Godot;

public partial class C_KinematicGround : C_Kinematic
{
    #region Exported Variables
    [Export]
    public float maximumSpeed = 500;
    [Export]
    public float movementFriction = 20;
    #endregion

    #region Constructor
    public C_KinematicGround()
    {
        type = E_Type.Ground;
    }
    #endregion

    #region Update
    public override void update(double delta, float factor = 1)
    {
        character.Velocity = new Vector2(Mathf.Clamp(Mathf.MoveToward(character.Velocity.X, 0, movementFriction * factor), -maximumSpeed, maximumSpeed), character.Velocity.Y);
    }
    #endregion
}