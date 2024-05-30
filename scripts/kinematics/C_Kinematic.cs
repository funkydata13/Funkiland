using Godot;
using System.Collections.Generic;

public partial class C_Kinematic : Node
{
    public enum E_Type { Root, Air, Ground, Move, Walk, Run, Sprint, Jump }

    #region Variables
    [Export]
    public E_Type type;
    public C_Character character;
    public Dictionary<E_Type, C_Kinematic> childNodes;
    #endregion

    #region Properties
    public bool isGrounded
    {
        get { return character.IsOnFloor(); }
    }

    public bool isFalling
    {
        get { return isGrounded == false && character.Velocity.Y >= 0; }
    }

    public bool isDirectionJustChanged
    {
        get { return character.Velocity.X * character.forwardDirection < 0; }
    }

    public Vector2 velocity
    {
        get { return character.Velocity; }
    }
    #endregion

    #region Godot Overrides
    public override void _Ready()
    {
        base._Ready();
        character = Owner as C_Character;

        childNodes = new Dictionary<E_Type, C_Kinematic>();
        foreach (Node node in GetChildren())
        {
            if (node is C_Kinematic) childNodes.Add((node as C_Kinematic).type, node as C_Kinematic);
        }

        SetProcess(false);
        SetPhysicsProcess(false);
    }
    #endregion

    #region Functions
    public bool HasKinematic(E_Type type)
    {
        return childNodes.ContainsKey(type);
    }

    public T GetChildKinematic<T>(E_Type type) where T : C_Kinematic
    {
        return HasKinematic(type) ? childNodes[type] as T : null;
    }

    public void Repel(Node2D from, float lateralSpeed, float verticalSpeed, bool impulsion = false)
    {
        Vector2 force = Vector2.Zero;

        if (from != null) force = new Vector2((character.GlobalPosition.X >= from.GlobalPosition.X ? 1 : -1) * lateralSpeed, -verticalSpeed);
        else force = new Vector2(-character.forwardDirection * lateralSpeed, -verticalSpeed);

        if (impulsion) character.Velocity += force;
        else character.Velocity = force;
    }

    public void Push(float speed, bool pushForward, bool autoFlipActor = false, bool impulsion = false)
    {
        float velocityX = character.forwardDirection * speed * (pushForward ? 1 : -1);
        if (autoFlipActor && pushForward == false) character.Flip();

        if (impulsion) character.Velocity = new Vector2(character.Velocity.X + velocityX, character.Velocity.Y);
        else character.Velocity = new Vector2(velocityX, character.Velocity.Y);
    }

    public virtual void update(double delta, float factor = 1) { }
    #endregion
}