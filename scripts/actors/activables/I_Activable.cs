using Godot;
using System;

public interface I_Activable
{
    bool isActivating { get; }
    bool isActivated { get; }
    int childsCount { get; }
    bool CanActivate(Node2D activator);
    void Activate();
    void Reset();
    I_Activable GetActivationChild(int index);
    void SetActivationChild(I_Activable child);
}