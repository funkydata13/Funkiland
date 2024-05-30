using Godot;
using System.Collections.Generic;


public partial class C_Attributes : Node
{
    #region Variables
    public Dictionary<C_Attribute.E_Type, C_Attribute> attributes;
    public Dictionary<C_Attribute.E_Type, S_AttributeTransfer> transfers;
    #endregion

    #region Properties
    public C_Attribute this[C_Attribute.E_Type type]
    {
        get { return attributes[type]; }
    }
    #endregion

    #region Ready
    public override void _Ready()
    {
        base._Ready();
        attributes = new Dictionary<C_Attribute.E_Type, C_Attribute>();
        transfers = new Dictionary<C_Attribute.E_Type, S_AttributeTransfer>();

        foreach (Node node in GetChildren())
        {
            if (node is C_Attribute)
            {
                C_Attribute attribute = node as C_Attribute;
                attributes.Add(attribute.type, attribute);
                transfers.Add(attribute.type, new S_AttributeTransfer { type = attribute.type, factor = 1 });
            }
        }

        SetProcess(false);
        SetPhysicsProcess(false);
    }
    #endregion

    #region Functions
    public bool HasAttribute(C_Attribute.E_Type type)
    {
        return attributes.ContainsKey(type);
    }

    public void SetAttributeTransfer(C_Attribute.E_Type transferFrom, C_Attribute.E_Type transferTo, float transferTactor = 1)
    {
        if (transfers.ContainsKey(transferFrom) == false) return;
        transfers[transferFrom] = new S_AttributeTransfer { type = transferTo, factor = transferTactor };
    }

    public void ResetAttributeTransfer(C_Attribute.E_Type type)
    {
        SetAttributeTransfer(type, type, 1);
    }

    public void Damage(C_Attribute.E_Type type, C_Attribute.E_Action action, float amount, Node2D dealer, float force)
    {
        float damagePool = amount;

        if (transfers[type].type != type)
        {
            damagePool *= transfers[type].factor;
            float mirroredValue = attributes[transfers[type].type].currentValue;

            attributes[transfers[type].type].ModifyValue(C_Attribute.E_Action.Transfer, damagePool, dealer, force);

            if (damagePool <= mirroredValue) damagePool = 0;
            else damagePool -= mirroredValue;
        }

        if (damagePool > 0) attributes[type].ModifyValue(action, damagePool, dealer, force);
    }
    #endregion
}