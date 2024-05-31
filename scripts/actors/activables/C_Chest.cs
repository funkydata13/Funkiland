using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

[Tool]
public partial class C_Chest : C_ActivableArea
{
    private static List<List<string>> LootTable = new List<List<string>>()
    {
        new List<string>() { "1|1-2-3-4|40-40-15-5", "100|1-2-3|85-10-5|85-10-5" },
        new List<string>() { "1|1-2-3-4|30-30-25-15", "100|1-2-3|70-20-10|70-20-10" },
        new List<string>() { "1|2-3-4-5|20-30-30-20", "100|1-2-3|50-30-20|50-30-20" },
        new List<string>() { "1|3-4-5-6|10-30-30-30", "100|1-2-3|30-40-30|30-30-40" },
    };

    protected int _variant = 1;
    [Export]
    public int variant
    {
        get { return _variant; }
        set 
        { 
            if (_variant != Math.Clamp(value, 1, 2))
            { 
                _variant =  Math.Clamp(value, 1, 2);
                UpdateSprite();
            } 
        }
    }

    protected int _keysNeeded = 1;
    [Export]
    public int keysNeeded
    {
        get { return _keysNeeded; }
        set
        {
            if (_keysNeeded != Math.Clamp(value, 0, 3))
            { 
                _keysNeeded =  Math.Clamp(value, 0, 3);
                UpdateSprite();
            } 
        }
    }

    public C_Inventory inventory;
    public C_Stopwatch stopwatch;
    protected C_Inventory _targetInventory;

    public override void _Ready()
    {
        base._Ready();

        if (Engine.IsEditorHint() == false)
        {
            stopwatch = GetNode<C_Stopwatch>("Stopwatch");
            inventory = GetNode<C_Inventory>("Inventory");
            foreach (string s in LootTable[_keysNeeded]) inventory.Generate(s);
        }
    }

    protected override void UpdateSprite()
    {
        if (_activable.sprite == null) return;

        if ((int)state < 3) _activable.sprite.Play(string.Format("{0}_{1}", variant - 1, (int)state));
        else
        {
            string anim = string.Format("{0}_{1}", variant - 1, state == C_Activable.E_State.Activated ? "1" : "0");
            _activable.sprite.Animation = anim;
            _activable.sprite.Frame = state == C_Activable.E_State.Activated ? _activable.sprite.SpriteFrames.GetFrameCount(anim) : 0;
        }
    }

    public override bool CanActivate(Node2D activator)
    {
        if (_keysNeeded == 0)
        {
            _targetInventory = null;
            return true;
        }

        if (activator is C_Character)
        {
            C_Character character = activator as C_Character;
            if (character.inventory != null)
            {
                 if (character.inventory[C_Item.E_Type.Key] >= _keysNeeded)
                 {
                    _targetInventory = character.inventory;
                    return true;
                 }
            }
        }

        return false;
    }

    public override void Activate()
    {
        base.Activate();
        if (_targetInventory != null) _targetInventory.Remove(C_Item.E_Type.Key, _keysNeeded);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_activable.sprite != null && _activable.sprite.IsPlaying() == false)
        {
            if (state == C_Activable.E_State.Activating)
            {
                state = C_Activable.E_State.Activated;
                inventory.Drop();
                stopwatch.Start();
            }
            else if (state == C_Activable.E_State.Activated)
            {
                if (stopwatch.isOver) state = C_Activable.E_State.Desactivating;
            }
            else if (state == C_Activable.E_State.Desactivating)
            {
                state = C_Activable.E_State.Desactivated;

                Monitorable = false;
                SetPhysicsProcess(false);
            }
        }
    }
}
