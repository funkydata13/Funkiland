using Godot;

[Tool]
public partial class C_ItemPotion : C_Item
{
    public C_Attribute.E_Type _attributeType;
    [Export]
    public C_Attribute.E_Type attributeType
    {
        get { return _attributeType; }
        set
        {
            _attributeType = value;

            if (attributeType == C_Attribute.E_Type.Vitality) _type = E_Type.Vitality; 
            else if (attributeType == C_Attribute.E_Type.Stamina) _type = E_Type.Stamina; 
            else _type = E_Type.Magic;

            UpdateSprite();
        }
    }

    public C_ItemPotion()
    {
        _type = E_Type.Vitality;
    }

    public override void _Ready()
    {
        base._Ready();
        UpdateSprite();
    }

    protected void UpdateSprite()
    {
        if (sprite != null) sprite.Texture = C_Assets.GetItemTexture(_type);
    }
}