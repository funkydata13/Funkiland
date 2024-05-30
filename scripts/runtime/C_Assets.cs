using System.Collections.Generic;
using Godot;

public static class C_Assets
{
    private static bool _isInitialized = false;
    private static Dictionary<C_Attribute.E_Type, Color> _attributeColors;
    private static Dictionary<C_Item.E_Type, PackedScene> _itemScenes;
    private static Dictionary<C_Item.E_Type, Texture2D> _itemTextures;

    public static Color GetAttributeColor(C_Attribute.E_Type attributeType)
    {
        if (_isInitialized == false) Initialize();
        return _attributeColors[attributeType];
    }

    public static PackedScene GetItemScene(C_Item.E_Type itemType)
    {
        if (_isInitialized == false) Initialize();
        return _itemScenes[itemType];
    }

    public static Texture2D GetItemTexture(C_Item.E_Type itemType)
    {
        if (_isInitialized == false) Initialize();
        return _itemTextures[itemType];
    }

    public static C_Item CreateItemInstance(C_Item.E_Type itemType)
    {
        if (_isInitialized == false) Initialize();

        if (itemType == C_Item.E_Type.Key) return _itemScenes[itemType].Instantiate<C_ItemKey>();
        else if (itemType == C_Item.E_Type.Coin) return _itemScenes[itemType].Instantiate<C_ItemCoin>();
        else
        {
            C_ItemPotion newPotion = _itemScenes[itemType].Instantiate<C_ItemPotion>();
            if (itemType == C_Item.E_Type.Stamina) newPotion.attributeType = C_Attribute.E_Type.Stamina;
            else if (itemType == C_Item.E_Type.Magic) newPotion.attributeType = C_Attribute.E_Type.Magic;
            else newPotion.attributeType = C_Attribute.E_Type.Vitality;

            return newPotion;
        }
    }

    private static void Initialize()
    {
        if (_attributeColors == null)
        {
            _attributeColors = new Dictionary<C_Attribute.E_Type, Color>
            {
                { C_Attribute.E_Type.Vitality, new Color(1.0f, 0.1f, 0.1f) },
                { C_Attribute.E_Type.Stamina, new Color(0.475f, 1.0f, 0.1f) },
                { C_Attribute.E_Type.Magic, new Color(0.1f, 0.79f, 1.0f) }
            };
        }

        if (_itemScenes == null)
        {
            _itemScenes = new Dictionary<C_Item.E_Type, PackedScene>
            {
                { C_Item.E_Type.Key, ResourceLoader.Load<PackedScene>("res://scenes/actors/collectibles/key.tscn") },
                { C_Item.E_Type.Coin, ResourceLoader.Load<PackedScene>("res://scenes/actors/collectibles/coin.tscn") },
                { C_Item.E_Type.Vitality, ResourceLoader.Load<PackedScene>("res://scenes/actors/collectibles/potion.tscn") },
                { C_Item.E_Type.Stamina, ResourceLoader.Load<PackedScene>("res://scenes/actors/collectibles/potion.tscn") },
                { C_Item.E_Type.Magic, ResourceLoader.Load<PackedScene>("res://scenes/actors/collectibles/potion.tscn") },
                { C_Item.E_Type.Vial, ResourceLoader.Load<PackedScene>("res://scenes/actors/collectibles/potion.tscn") }
            };
        }

        if (_itemTextures == null)
        {
            _itemTextures = new Dictionary<C_Item.E_Type, Texture2D>
            {
                { C_Item.E_Type.Key, ResourceLoader.Load<Texture2D>("res://assets/sprites/key.png") },
                { C_Item.E_Type.Coin, ResourceLoader.Load<Texture2D>("res://assets/sprites/coin.png") },
                { C_Item.E_Type.Vitality, ResourceLoader.Load<Texture2D>("res://assets/sprites/potion_vitality.png") },
                { C_Item.E_Type.Stamina, ResourceLoader.Load<Texture2D>("res://assets/sprites/potion_stamina.png") },
                { C_Item.E_Type.Magic, ResourceLoader.Load<Texture2D>("res://assets/sprites/potion_magic.png") },
                { C_Item.E_Type.Vial, ResourceLoader.Load<Texture2D>("res://assets/sprites/potion_empty.png") }
            };
        }

        _isInitialized = true;
    }
}