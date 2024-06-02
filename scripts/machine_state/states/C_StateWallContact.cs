using Godot;
using System;
using System.Diagnostics;

public partial class C_StateWallContact : C_State
{
    [Export]
    public float spriteOffset = -4;
    [Export]
    public float maximumFallSpeed = 60;
    public S_StateWall stateWall;
    protected C_KinematicAir airKinematic;

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        airKinematic = machine.kinematics.GetChildKinematic<C_KinematicAir>(C_Kinematic.E_Type.Air);
    }
    #endregion

    #region Functions Overrides
    public override bool CanEnter()
    {
        return airKinematic != null;
    }

    public override void Enter()
    {
        base.Enter();
        SetSpriteAnimation();

        stateWall = new S_StateWall() { stateMaximumFallSpeed = maximumFallSpeed , maximumFallSpeedBackup = airKinematic.maximumSpeed,
            stateSpriteOffset = spriteOffset, spriteOffsetBackup = machine.sprite.Offset.X };
        
        airKinematic.maximumSpeed = maximumFallSpeed;
        machine.sprite.Offset = new Vector2(spriteOffset, machine.sprite.Offset.Y);
    }

    public void UndoState()
    {
        airKinematic.maximumSpeed = stateWall.maximumFallSpeedBackup;
        machine.sprite.Offset = new Vector2(stateWall.spriteOffsetBackup, machine.sprite.Offset.Y);
    }

    public override void CheckStatus(double delta)
    {
        if (IsOver())
        {
            if (machine.kinematics.isGrounded)
            {
                UndoState();
                machine.ChangeState("Idle");
                return;
            }
            else
            {
                if (machine.isFacingObstacle && machine.obstacleClass == "TileMap")
                {
                    machine.ChangeState("Wall Sliding");
                    return;
                }
                else 
                { 
                    UndoState(); 
                    machine.ChangeState("Flying"); 
                    return;
                }
            }
        }

        if (machine.CanReadInputs)
        {
            if (C_Inputs.IsActionJustPressed("jump"))
            {
                UndoState();
                machine.ChangeState("Wall Jumping");
            }
        }
    }
    #endregion

    #region Physics Process
    public override void _PhysicsProcess(double delta)
    {
        airKinematic.Update(delta);
        base._PhysicsProcess(delta);
    }
    #endregion
}