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

    protected bool _canReadInputs;
    protected C_State _currentState;
    protected C_State _previousState;
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

        _currentState = FindChild(defaultState) as C_State;
        if (_currentState.CanEnter() == false) GD.PushError("L'état initial de la C_MachineState n'accepte pas l'entrée !");
        _currentState.Enter();

        SetProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _canReadInputs = readInputs && C_Inputs.CanListenInputs(C_Inputs.E_Listener.Player);
    }
    #endregion
}