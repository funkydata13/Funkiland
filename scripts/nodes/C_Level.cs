using Godot;
using System;

public partial class C_Level : Node2D
{
    private const string LEVELS_PATH = "res://levels/$.tscn";

    public enum E_PlayerState { Null, Defeated, Dead, Live, Operationnal }
    protected E_PlayerState _playerState;

    [Export]
    public string next_Level = "main_menu";
    [Export]
    public bool isPlayable = true;
    [Export]
    public C_PlayerSpawner currentPlayerSpawner;
    [Export]
    public float playerRespawnDelay = 2;

    protected float _respawnTimeLeft;

    public override void _Ready()
    {
        base._Ready();

        C_Game.Level = this;
        if (isPlayable && currentPlayerSpawner == null) GD.PushError("Un C_Level n'a pas de C_PlayerSpawner attaché !");

        SetProcess(isPlayable);
        SetPhysicsProcess(false);
    }

    protected void OnPlayerDefeated()
    {
        _respawnTimeLeft = playerRespawnDelay;
        _playerState = E_PlayerState.Defeated;
    }

    public void ChangeLevel(string levelFilename)
    {
        string fName = LEVELS_PATH.Replace("$", levelFilename);

        if (ResourceLoader.Exists(fName) == false) GD.PushError(string.Format("Le niveau {0} localisé en théorie dans {1} n'existe pas !", levelFilename, fName));
        else GetTree().ChangeSceneToFile(fName);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (IsInstanceValid(C_Game.Player) == false && IsInstanceValid(currentPlayerSpawner) && (_playerState == E_PlayerState.Null || _playerState == E_PlayerState.Dead))
        {
            if (currentPlayerSpawner.CreatePlayerIntance() != null) _playerState = E_PlayerState.Live;
        }
        else if (_playerState == E_PlayerState.Live)
        {
            C_Game.Player.Defeated += OnPlayerDefeated;
            _playerState = E_PlayerState.Operationnal;
        }
        else if (_playerState == E_PlayerState.Defeated)
        {
            if (_respawnTimeLeft > 0) _respawnTimeLeft -= (float)delta;
            else
            {
                _playerState = E_PlayerState.Dead;
                C_Game.Player.QueueFree();
            }
        }
    }
}
