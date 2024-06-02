using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

[Tool]
public partial class C_Jar : C_DestructibleBody
{
    private static List<List<string>> LootTable = new List<List<string>>()
    {
        new List<string>() { "1|1-2-3-4|40-30-20-10" },
        new List<string>() { "1|1-2-3-4|40-30-20-10" },
        new List<string>() { "1|1-2-3-4|70-15-10-5" },
        new List<string>() { "1|1-2-3-4|70-15-10-5" }
    };

    protected int _variant = 1;
    [Export]
    public int variant
    {
        get { return _variant; }
        set 
        { 
            if (_variant != Math.Clamp(value, 1, 4))
            { 
                _variant =  Math.Clamp(value, 1, 4);
                UpdateSprite();
            } 
        }
    }

    public override void _Ready()
    {
        base._Ready();

        if (_inventory != null)
        {
            foreach (string s in LootTable[variant - 1])
                _inventory.Generate(s);
        }

        UpdateSprite();
    }

    protected virtual void UpdateSprite()
    {
        if (_sprite == null) return;

        _sprite.Animation = variant.ToString();
        _sprite.Frame = isDestroyed ? 1 : 0;
    }

    protected override void OnVitalityDepleted(Node2D depletedBy)
    {
        DisablePhysics();
        UpdateSprite();

        _inventory.Drop();
    }
}
