using Godot;

public partial class C_KinematicMove : C_Kinematic
{
    #region Exported Variables
    [Export]
    public float speed = 110;
    [Export]
    public float maximumSpeed = 500;
    [Export]
    public float movementFriction = 20;
    #endregion

    #region Constructor
    public C_KinematicMove()
    {
        type = E_Type.Move;
    }
    #endregion

    #region Update
    public override void Update(double delta, float factor = 1)
    {
        character.Velocity = new Vector2(Mathf.Clamp(Mathf.MoveToward(character.Velocity.X, speed * character.forwardDirection * factor, movementFriction * factor), -maximumSpeed, maximumSpeed), character.Velocity.Y);
    }
    #endregion
}