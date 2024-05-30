using Godot;
using System;
using System.Diagnostics;

public partial class C_AnimationAttributeFlash : C_AnimationAttribute
{
    [Export]
    public CanvasItem canvas;
    [Export]
    public Color color = Colors.White;

    public override void _Ready()
    {
        base._Ready();

        if (canvas == null) GD.PushError("Un C_AnimationFlash n'a pas de CanvasItem lié !");
        _animationName = "sprite_animations/effect_flash";
    }

    public override bool Play(bool resetBefore)
    {
        canvas.Material.Set("shader_parameter/flash_color", color);
        return base.Play(resetBefore);
    }
}