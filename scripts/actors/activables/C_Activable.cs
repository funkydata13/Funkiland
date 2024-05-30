using System.Collections.Generic;
using Godot;

public partial class C_Activable
{
    public enum E_State { Pending = 0, Activating, Desactivating, Activated, Desactivated }

    protected E_State _state = E_State.Pending;
    public List<I_Activable> activableChilds;
    public AnimatedSprite2D sprite;
    
    public virtual E_State state
    {
        get { return _state; }
        set { _state = value; }
    }

    public int childsCount
    { 
        get { return activableChilds.Count; } 
    }

    public bool isActivating 
    { 
        get { return _state == E_State.Activating || _state == E_State.Desactivating; } 
    }

    public bool isActivated 
    { 
        get { return _state == E_State.Activated || _state == E_State.Desactivated; } 
    }

    public C_Activable()
    {
        activableChilds = new List<I_Activable>();
    }

    public void TransferArray(Node2D[] childs)
    {
        if (childs.Length > 0)
        {
            for (int i = 0; i < childs.Length; i++)
            {
                if (childs[i] is I_Activable) activableChilds.Add(childs[i] as I_Activable);
            }
        }
    }

     public I_Activable GetChild(int index)
    {
        if (activableChilds.Count < index) return activableChilds[index];
        else return null;
    }
    public void SetChild(I_Activable child)
    {
        activableChilds.Add(child);
    }

    public void Activate() 
    { 
        if (activableChilds.Count > 0)
        {
            for (int i = 0; i < activableChilds.Count; i++)
                if (activableChilds[i].isActivating == false || activableChilds[i].isActivated == false) 
                    activableChilds[i].Activate();
        }
    }

    public void Reset()
    {
        _state = E_State.Pending;
    }
}