using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_ActorDetector : Area2D
{
    protected C_Character _character;
    protected List<I_Item> _items;
    protected C_StackList<I_Item> _collectibles;
    protected C_StackList<I_Activable> _activables;

    public override void _Ready()
    {
        base._Ready();

        _character = Owner as C_Character;

        _items = new List<I_Item>();
        _collectibles = new C_StackList<I_Item>();
        _activables = new C_StackList<I_Activable>();

        BodyEntered += OnBodyEnter;
        BodyExited += OnBodyExit;
        AreaEntered += OnAreaEnter;
        AreaExited += OnAreaExit;

        SetProcess(false);
    }

    protected void OnBodyEnter(Node2D body)
    {
        if (body is I_Item) _items.Add(body as I_Item);
    }

    protected void OnBodyExit(Node2D body)
    {
        if (body is I_Item) { _items.Remove(body as I_Item); _collectibles.Remove(body as I_Item); }
    }

    protected void OnAreaEnter(Area2D area)
    {
        if (area is I_Activable) _activables.Add(area as I_Activable);
    }

    protected void OnAreaExit(Area2D area)
    {
        if (area is I_Activable) _activables.Remove(area as I_Activable);
    }

    protected void ProcessActivablesList()
    {
        if (_activables.Count == 0) return;

        I_Activable activable = _activables.Pop();
        if (activable == null) return;
        if (activable.isActivated || activable.isActivating || activable.CanActivate(_character) == false) _activables.Push();
        else activable.Activate();
    }

    protected bool ProcessCollectiblesList()
    {
        if (_items.Count > 0)
        {
            int i = 0;

            while (i < _items.Count)
            {
                if (_items[i].state == C_Item.E_State.Pending)
                {
                    _collectibles.Add(_items[i]);
                    _items.RemoveAt(i);
                }
                else i++;
            }
        }
        
        if (_collectibles.Count == 0) return false;

        I_Item nextCollectible = _collectibles.Pop();
        if (nextCollectible == null) return false;
        if (nextCollectible.state == C_Item.E_State.Pending)
        {
            nextCollectible.Pick(_character.GlobalPosition, _character.inventory);
            _collectibles.Push();
            return true;
        }
        else return false;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (C_Inputs.CanListenInputs(C_Inputs.E_Listener.Player) && C_Inputs.IsActionJustPressed("action"))
        {
            if (ProcessCollectiblesList() == false) ProcessActivablesList();
        }
    }
}
