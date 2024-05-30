using Godot;
using System;
using System.Diagnostics;

public partial class C_Boar : C_Character
{
    protected C_AttributeBar _vitalityBar;

    public override E_State state 
    { 
        get { return base.state; }
        set
        {
            base.state = value;
            if (value == E_State.Dead) _vitalityBar.isDisabled = true;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        _vitalityBar = GetNode<C_AttributeBar>("Vitality Bar");
        _inventory.Generate("1|0-1-2|30-60-10");
        _inventory.Generate("100|0-1|30-70|70-30-0");
    }

    public override void Terminate()
    {
        inventory.Drop();
        base.Terminate();
    }
}
