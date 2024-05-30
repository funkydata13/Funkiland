using Godot;
using System;

public partial class C_PlayerSpawner : Area2D
{
    protected const string PLAYER_SCENE_PATH = "res://scenes/actors/characters/player.tscn";
    protected static PackedScene PlayerScene;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
        if (PlayerScene == null) PlayerScene = ResourceLoader.Load<PackedScene>(PLAYER_SCENE_PATH);
    }

    protected void OnBodyEntered(Node2D body)
    {
        if (body is C_Player && IsInstanceValid(C_Game.Level)) C_Game.Level.currentPlayerSpawner = this;
    }

    public C_Player CreatePlayerIntance()
    {
        C_Player newPlayer = PlayerScene.Instantiate<C_Player>();
        newPlayer.GlobalPosition = GlobalPosition;
        GetParent().AddChild(newPlayer);
        return newPlayer;
    }
}
