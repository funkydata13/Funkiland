using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_ActorDetector : Area2D
{
    protected C_Character _character;
    protected List<I_Item> items;
    protected C_StackList<I_Item> collectibles;

    public override void _Ready()
    {
        base._Ready();

        _character = Owner as C_Character;

        items = new List<I_Item>();
        collectibles = new C_StackList<I_Item>();

        BodyEntered += OnBodyEnter;
        BodyExited += OnBodyExit;

        SetProcess(false);
    }

    protected void OnBodyEnter(Node2D body)
    {
        if (body is I_Item) items.Add(body as I_Item);
    }

    protected void OnBodyExit(Node2D body)
    {
        if (body is I_Item) { items.Remove(body as I_Item); collectibles.Remove(body as I_Item); }
    }

    protected bool ProcessCollectiblesList()
    {
        if (items.Count > 0)
        {
            int i = 0;

            while (i < items.Count)
            {
                if (items[i].state == C_Item.E_State.Pending)
                {
                    collectibles.Add(items[i]);
                    items.RemoveAt(i);
                }
                else i++;
            }
        }
        
        if (collectibles.Count == 0) return false;

        I_Item nextCollectible = collectibles.Pop();
        if (nextCollectible == null) return false;
        if (nextCollectible.state == C_Item.E_State.Pending)
        {
            nextCollectible.Pick(_character.GlobalPosition, _character.inventory);
            collectibles.Push();
            return true;
        }
        else return false;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (C_Inputs.CanListenInputs(C_Inputs.E_Listener.Player) && C_Inputs.IsActionJustPressed("action"))
        {
            ProcessCollectiblesList();
        }
    }
}
