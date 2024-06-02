using Godot;

public partial class C_KinematicSlide : C_Kinematic
{
    #region Exported Variables
    [Export]
    public float speed = 200;
    [Export]
    public float movementFriction = 5;
    [Export]
    public float breakBySpeedFactor = 0.2f;
    #endregion

    #region Properties
    public bool isBreak
    {
        get { return Mathf.Abs(character.Velocity.X) <= speed * breakBySpeedFactor; }
    }
    #endregion

    #region Constructor
    public C_KinematicSlide()
    {
        type = E_Type.Slide;
    }
    #endregion

    #region Update
    public override void Update(double delta, float factor = 1)
    {
        character.Velocity = new Vector2(speed * character.forwardDirection * factor, character.Velocity.Y);
    }
    #endregion
}