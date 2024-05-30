using System.Collections.Generic;
using System.Diagnostics;
using Godot;

public partial class C_Inventory : Node
{
    [Signal]
    public delegate void ChangedEventHandler(int type, int quantity);
    [Signal]
    public delegate void DropedEventHandler();

    [Export]
    public int potionsCapacity = 3;
    [Export]
    public int coinsCapacity = 999;
    [Export]
    public int keysCapacity = 9;

    public Dictionary<C_Item.E_Type, int> items;
    public Dictionary<C_Item.E_Type, int> itemsCapacity;

    public int this[C_Item.E_Type type]
    {
        get { return items[type]; }
    }

    public override void _Ready()
    {
        base._Ready();

        items = new Dictionary<C_Item.E_Type, int>
        {
            { C_Item.E_Type.Coin, 0 },
            { C_Item.E_Type.Key, 0 },
            { C_Item.E_Type.Vitality, 0 },
            { C_Item.E_Type.Stamina, 0 },
            { C_Item.E_Type.Magic, 0 }
        };

        itemsCapacity = new Dictionary<C_Item.E_Type, int>
        {
            { C_Item.E_Type.Coin, coinsCapacity },
            { C_Item.E_Type.Key, keysCapacity },
            { C_Item.E_Type.Vitality, potionsCapacity },
            { C_Item.E_Type.Stamina, potionsCapacity },
            { C_Item.E_Type.Magic, potionsCapacity }
        };
    }

    private int GetInventoryCapacity(C_Item.E_Type type)
    {
        return itemsCapacity[type];
    }

    private int GetRandomIndex(string probaList)
    {
        GD.Randomize();

        int p = 0;
        int value = -1;
        int rndValue = GD.RandRange(0, 100);
        string[] pList = probaList.Split('-');

        for (int i = 0; i < pList.Length; i++)
        {
            p += int.Parse(pList[i]);
            if (rndValue <= p)
            {
                value = i;
                break;
            }
        }

        if (value == -1) return pList.Length - 1;
        else return value;
    }

     public void Generate(string generationData)
    {
        // 1|0-1-2|30-60-10
        // 0|0-1|90-10
        // 100|1-2|80-20|60-30-10

        string[] pipes = generationData.Split('|');
        C_Item.E_Type type = (C_Item.E_Type)int.Parse(pipes[0]);

        string[] quantities = pipes[1].Split('-');
        int quantity = int.Parse(quantities[GetRandomIndex(pipes[2])]);
        if (quantity == 0) return;

        if ((int)type <= 10) Append(type, quantity);
        else
        {
            for (int i = 0; i < quantity; i++)
            {
                int potionType = GetRandomIndex(pipes[3]) + 2;
                Append((C_Item.E_Type)potionType, 1);
            }
        }
    }

     public void Append(C_Item.E_Type type, int quantity)
    {
        int initialQuantity = 0;

        if (items.ContainsKey(type) == false) items.Add(type, quantity);
        else { initialQuantity = items[type]; items[type] += quantity; };

        items[type] = Mathf.Clamp(items[type], 0, GetInventoryCapacity(type));
        
        if (items[type] != initialQuantity) EmitSignal(SignalName.Changed, (int)type, items[type]);
    }

    public void Remove(C_Item.E_Type type, int quantity)
    {
        if (items.ContainsKey(type)) 
        {
            int initialQuantity = items[type];
            items[type] = Mathf.Clamp(items[type] - quantity, 0, GetInventoryCapacity(type)); 
            if (items[type] != initialQuantity) EmitSignal(SignalName.Changed, (int)type, items[type]);
        }
    }

    public void Drop()
    {
        foreach (C_Item.E_Type type in items.Keys)
        {
            if (items[type] > 0)
            {
                for (int i = 0; i < items[type]; i++)
                {
                    C_Item item = C_Assets.CreateItemInstance(type);
                    item.Visible = false;
                    Owner.GetParent<Node2D>().AddChild(item);
                    item.Drop((Owner as Node2D).GlobalPosition);
                }

                items[type] = 0;
            }
        }

        EmitSignal(SignalName.Droped, 0, 0, true);
    }
}