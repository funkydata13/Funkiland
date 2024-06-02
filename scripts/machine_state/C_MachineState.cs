using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_MachineState : Node
{
    #region Variables
    [ExportGroup("Node Links")]
    [Export]
    public C_SpriteAnimated sprite;
    [Export]
    public C_Kinematic kinematics;
    [Export]
    public RayCast2D obstacleDetector;
    [Export]
    public RayCast2D ledgeDetector;

    [ExportGroup("Node Properties")]
    [Export]
    public bool readInputs = false;
    [Export]
    public string defaultState = "Idle";

    protected C_Character _character;
    protected bool _canReadInputs;
    protected C_State _currentState;
    protected C_State _previousState;
    protected bool _isFacingObstacle;
    protected bool _isFacingPlayer;
    protected bool _isFacingLedge;
    protected string _obstableClass;
    #endregion

    #region Properties
    public bool CanReadInputs
    {
        get { return _canReadInputs; }
    }

    public C_State previousState
    {
        get { return _previousState; }
    }

    public C_State currentState
    {
        get { return _currentState; }
    }

    public bool isFacingLedge
    {
        get { return _isFacingLedge; }
    }

    public bool isFacingObstacle
    {
        get { return _isFacingObstacle; }
    }

    public bool isFacingPlayer
    {
        get { return _isFacingPlayer; }
    }

    public bool isFacingSomething
    {
        get { return _isFacingObstacle || _isFacingPlayer; }
    }

    public string obstacleClass
    {
        get { return _obstableClass; }
    }
    #endregion

    #region Functions
    public bool ChangeState(string stateName, bool isOverride = false)
    {
        if (HasNode(stateName) == false) return false;
        if (_currentState.Name == stateName) 
        { 
            if (isOverride == false) return false;
            else
            {
                if (_currentState.CanEnter())
                {
                    _currentState.Exit();
                    _currentState.Enter();
                    return true;
                }
                else return false;
            }
        }

        C_State newState = FindChild(stateName) as C_State;
        if (newState.CanEnter() == false) return false;
        
        _currentState.Exit();
        _previousState = _currentState;
        _currentState = newState;
        _currentState.Enter();

        return true;
    }
    #endregion

    #region Godot Overrides
    public override void _Ready()
    {
        if (sprite == null || kinematics == null) GD.PushError("C_MachineState n'est la liée correctement !");

        _character = Owner as C_Character;
        
        _currentState = FindChild(defaultState) as C_State;
        if (_currentState.CanEnter() == false) GD.PushError("L'état initial de la C_MachineState n'accepte pas l'entrée !");
        _currentState.Enter();

        SetProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        _canReadInputs = readInputs && C_Inputs.CanListenInputs(C_Inputs.E_Listener.Player);

        if (obstacleDetector != null && obstacleDetector.IsColliding())
        {
            _obstableClass = obstacleDetector.GetCollider().GetClass();
            _isFacingPlayer = obstacleDetector.GetCollider() is C_Player;
            _isFacingObstacle = _isFacingPlayer == false;
        }
        else
        {
            _obstableClass = string.Empty;
            _isFacingPlayer = _isFacingObstacle = false;
        }
        
        _isFacingLedge = ledgeDetector != null && ledgeDetector.IsColliding() == false;
    }
    #endregion
}