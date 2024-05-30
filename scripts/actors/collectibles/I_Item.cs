using Godot;

public interface I_Item
{
    C_Item.E_Type type { get; }
    C_Item.E_State state { get; }
    void Drop(Vector2 position);
    void Pick(Vector2 position, C_Inventory targetInventory = null);
}