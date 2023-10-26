using System.ComponentModel;
using System.Numerics;

namespace P2P;

public class TranslationVectorCalculator
{
    public static Vector3 GetTranslationVector(Vector2 markerANormalizedImagePosition, Vector2 markerBNormalizedImagePosition, Vector4 markerACoordinateAtObjectReferenceFrame, Vector4 markerBCoordinateAtObjectReferenceFrame, Matrix4x4 RotationMatrix)
    {
        var Az = AzCalculator(markerANormalizedImagePosition, markerBNormalizedImagePosition, markerACoordinateAtObjectReferenceFrame, markerBCoordinateAtObjectReferenceFrame, RotationMatrix);
        var tx = Az * markerANormalizedImagePosition.X - Vector4.Dot(GetColumn1(RotationMatrix), markerACoordinateAtObjectReferenceFrame);
            
        var ty = Az * markerANormalizedImagePosition.Y - Vector4.Dot(GetColumn2(RotationMatrix), markerACoordinateAtObjectReferenceFrame);
        var tz = Az - Vector4.Dot(GetColumn3(RotationMatrix), markerACoordinateAtObjectReferenceFrame);

        return new Vector3(tx, ty, tz);
    }

    public static Vector4 GetColumn1(Matrix4x4 matrix)
    {
        return new Vector4(matrix.M11, matrix.M21, matrix.M31, matrix.M41);
    }
    public static Vector4 GetColumn2(Matrix4x4 matrix)
    {
        return new Vector4(matrix.M12, matrix.M22, matrix.M32, matrix.M42);
    }
    public static Vector4 GetColumn3(Matrix4x4 matrix)
    {
        return new Vector4(matrix.M13, matrix.M23, matrix.M33, matrix.M43);
    }
    public static float AzCalculator(Vector2 markerANormalizedImagePosition, Vector2 markerBNormalizedImagePosition, Vector4 markerACoordinateAtObjectReferenceFrame, Vector4 markerBCoordinateAtObjectReferenceFrame, Matrix4x4 RotationMatrix)
    {
        var numerator = Vector4.Dot( GetColumn1(RotationMatrix) - markerBNormalizedImagePosition.X*GetColumn3(RotationMatrix) , markerBCoordinateAtObjectReferenceFrame-markerACoordinateAtObjectReferenceFrame );
        var denominator = markerBNormalizedImagePosition.X - markerANormalizedImagePosition.X;

        return numerator / denominator;
    }
}