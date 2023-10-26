using System.Numerics;

namespace P2P;

public static class ImageCalculator
{
    public static Vector2 CalculateImageCoordinates(float f, float dpX, Vector2 imageCenter, Vector4 Poisition)
    {
        var xA = (f / dpX) * (Poisition.X / Poisition.Z) + imageCenter.X;
        var yA = (f / dpX) * (Poisition.Y / Poisition.Z) + imageCenter.Y;

        return new Vector2(xA, yA);
    }

    public static Vector2 CalculateProjectionalCoordinates(float f, float dpX, Vector2 imageCenter, Vector2 imagePosition)
    {
        var xTilde = (imagePosition.X - imageCenter.X) * dpX / f;
        var yTilde = (imagePosition.Y - imageCenter.Y) * dpX / f;

        return new Vector2(xTilde, yTilde);
    }
}