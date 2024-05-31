using Godot;

public static class C_Utils
{
    public static Color LerpColor(Color color1, Color color2, float w)
    {
        return new Color(
            Mathf.Lerp(color1.R, color2.R, w),
            Mathf.Lerp(color1.G, color2.G, w),
            Mathf.Lerp(color1.B, color2.B, w)
        );
    }

    public static Vector2 LerpVector(Vector2 v1, Vector2 v2, float w)
    {
        return new Vector2(Mathf.Lerp(v1.X, v2.X, w), Mathf.Lerp(v1.Y, v2.Y, w));
    }
}