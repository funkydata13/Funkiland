using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class C_GameHud : Control
{
    protected C_Player _player;
    protected Dictionary<C_Attribute.E_Type, Label> _attributeLabels;
    protected Dictionary<C_Attribute.E_Type, TextureProgressBar> _attributeBars;

    protected Dictionary<C_Item.E_Type, Label> _itemLabels;
    protected Dictionary<C_Item.E_Type, TextureRect> _itemTextures;

    public override void _Ready()
    {
        base._Ready();

        _attributeLabels = new Dictionary<C_Attribute.E_Type, Label>
        {
            { C_Attribute.E_Type.Vitality, GetNode<Label>("Margin/Attributes/Vitality/Text") },
            { C_Attribute.E_Type.Stamina, GetNode<Label>("Margin/Attributes/Stamina/Text") },
            { C_Attribute.E_Type.Magic, GetNode<Label>("Margin/Attributes/Magic/Text") }
        };

        _attributeBars = new Dictionary<C_Attribute.E_Type, TextureProgressBar>
        {
            { C_Attribute.E_Type.Vitality, GetNode<TextureProgressBar>("Margin/Attributes/Vitality/Bar") },
            { C_Attribute.E_Type.Stamina, GetNode<TextureProgressBar>("Margin/Attributes/Stamina/Bar") },
            { C_Attribute.E_Type.Magic, GetNode<TextureProgressBar>("Margin/Attributes/Magic/Bar") }
        };

        _itemLabels = new Dictionary<C_Item.E_Type, Label>
        {
            { C_Item.E_Type.Key, GetNode<Label>("Margin/Items/KeyText") },
            { C_Item.E_Type.Coin, GetNode<Label>("Margin/Items/CoinText") },
            { C_Item.E_Type.Vitality, GetNode<Label>("Margin/Potions/Vitality/Text") },
            { C_Item.E_Type.Stamina, GetNode<Label>("Margin/Potions/Stamina/Text") },
            { C_Item.E_Type.Magic, GetNode<Label>("Margin/Potions/Magic/Text") }
        };

        _itemTextures = new Dictionary<C_Item.E_Type, TextureRect>
        {
            { C_Item.E_Type.Vitality, GetNode<TextureRect>("Margin/Potions/Vitality/Texture") },
            { C_Item.E_Type.Stamina, GetNode<TextureRect>("Margin/Potions/Stamina/Texture") },
            { C_Item.E_Type.Magic, GetNode<TextureRect>("Margin/Potions/Magic/Texture") }
        };

        GetNode<Button>("Margin/Commands/Kill").MouseEntered += OnMouseEnterControl;
        GetNode<Button>("Margin/Commands/Kill").MouseExited += OnMouseExitControl;
        GetNode<Button>("Margin/Commands/Kill").Pressed += OnPlayerKillClick;

        GetNode<Button>("Margin/Commands/Menu").MouseEntered += OnMouseEnterControl;
        GetNode<Button>("Margin/Commands/Menu").MouseExited += OnMouseExitControl;
        GetNode<Button>("Margin/Commands/Menu").Pressed += OnMainMenuClick;

        SetPhysicsProcess(false);
    }

    protected void UpdateAttributeUI(C_Attribute.E_Type type)
    {
        if (_attributeBars[type].MaxValue != _player.attributes[type].maximumValue)
        {
            _attributeBars[type].MaxValue = _player.attributes[type].maximumValue;
            _attributeBars[type].Step = 1.0f / _player.attributes[type].maximumValue;
        }
        
        _attributeBars[type].Value = _player.attributes[type].currentValue;
        _attributeLabels[type].Text = string.Format("{0}/{1}", (int)_player.attributes[type].currentValue, (int)_player.attributes[type].maximumValue);
    }

    protected void LinkToPlayer()
    {
        if (IsInstanceValid(C_Game.Player))
        {
            _player = C_Game.Player;

            UpdateAttributeUI(C_Attribute.E_Type.Vitality);
            _player.attributes[C_Attribute.E_Type.Vitality].Changed += OnVitalityChanged;
            UpdateAttributeUI(C_Attribute.E_Type.Stamina);
            _player.attributes[C_Attribute.E_Type.Stamina].Changed += OnStaminaChanged;
            UpdateAttributeUI(C_Attribute.E_Type.Magic);
            _player.attributes[C_Attribute.E_Type.Magic].Changed += OnMagicChanged;

            _itemLabels[C_Item.E_Type.Key].Text = string.Format("x{0}", _player.inventory[C_Item.E_Type.Key]);
            _itemLabels[C_Item.E_Type.Coin].Text = string.Format("x{0}", _player.inventory[C_Item.E_Type.Coin]);
            _itemLabels[C_Item.E_Type.Vitality].Text = string.Format("x{0}", _player.inventory[C_Item.E_Type.Vitality]);
            _itemLabels[C_Item.E_Type.Stamina].Text = string.Format("x{0}", _player.inventory[C_Item.E_Type.Stamina]);
            _itemLabels[C_Item.E_Type.Magic].Text = string.Format("x{0}", _player.inventory[C_Item.E_Type.Magic]);

            _itemTextures[C_Item.E_Type.Vitality].Texture = C_Assets.GetItemTexture(_player.inventory[C_Item.E_Type.Vitality] > 0 ? C_Item.E_Type.Vitality : C_Item.E_Type.Vial);
            _itemTextures[C_Item.E_Type.Stamina].Texture = C_Assets.GetItemTexture(_player.inventory[C_Item.E_Type.Stamina] > 0 ? C_Item.E_Type.Stamina : C_Item.E_Type.Vial);
            _itemTextures[C_Item.E_Type.Magic].Texture = C_Assets.GetItemTexture(_player.inventory[C_Item.E_Type.Magic] > 0 ? C_Item.E_Type.Magic : C_Item.E_Type.Vial);

            _player.inventory.Changed += OnInventoryChanged;
        }
    }

    protected void OnVitalityChanged(int action, float changedAmount, float changedValue, Node2D changeBy, float changeDelta)
    {
        UpdateAttributeUI(C_Attribute.E_Type.Vitality);
    }

    protected void OnStaminaChanged(int action, float changedAmount, float changedValue, Node2D changeBy, float changeDelta)
    {
        UpdateAttributeUI(C_Attribute.E_Type.Stamina);
    }

    protected void OnMagicChanged(int action, float changedAmount, float changedValue, Node2D changeBy, float changeDelta)
    {
        UpdateAttributeUI(C_Attribute.E_Type.Magic);
    }

    protected void OnInventoryChanged(int type, int quantity)
    {
        C_Item.E_Type realType = (C_Item.E_Type)type;
        _itemLabels[realType].Text = string.Format("x{0}", quantity);

        if (_itemTextures.ContainsKey(realType)) 
            _itemTextures[realType].Texture = C_Assets.GetItemTexture(quantity > 0 ? realType : C_Item.E_Type.Vial);
    }

    protected void OnMouseEnterControl()
    {
        C_Inputs.LockInputsTo(C_Inputs.E_Listener.UI);
    }

     protected void OnMouseExitControl()
    {
        C_Inputs.UnlockInputs();
    }

    protected void OnPlayerKillClick()
    {
        if (IsInstanceValid(_player)) _player.attributes[C_Attribute.E_Type.Vitality].ModifyValue(C_Attribute.E_Action.Lost, 9999, null, 0);
    }

    protected void OnMainMenuClick()
    {

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (IsInstanceValid(_player) == false) LinkToPlayer();
    }
}
