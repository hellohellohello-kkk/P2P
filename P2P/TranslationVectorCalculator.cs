using System.ComponentModel;
using System.Numerics;

namespace P2P;

public class TranslationVectorCalculator
{
    public Vector3 GetTranslationVector(Vector2 markerANormalizedImagePosition, Vector2 markerBNormalizedImagePosition, Vector3 markerACoordinateAtObjectReferenceFrame, Vector3 markerBCoordinateAtObjectReferenceFrame, Matrix4x4 RotationMatrix)
    {
        var Az = AzCalculator(markerANormalizedImagePosition, markerBNormalizedImagePosition, markerACoordinateAtObjectReferenceFrame, markerBCoordinateAtObjectReferenceFrame, RotationMatrix);
        var tx = Az * markerANormalizedImagePosition.X - Vector3.Dot(GetColumn1(RotationMatrix), markerACoordinateAtObjectReferenceFrame);
            
        var ty = Az * markerANormalizedImagePosition.Y - Vector3.Dot(GetColumn2(RotationMatrix), markerACoordinateAtObjectReferenceFrame);
        var tz = Az - Vector3.Dot(GetColumn3(RotationMatrix), markerACoordinateAtObjectReferenceFrame);

        return new Vector3(tx, ty, tz);
    }

    public static Vector3 GetColumn1(Matrix4x4 matrix)
    {
        return new Vector3(matrix.M11, matrix.M21, matrix.M31);
    }
    public static Vector3 GetColumn2(Matrix4x4 matrix)
    {
        return new Vector3(matrix.M12, matrix.M22, matrix.M32);
    }
    public static Vector3 GetColumn3(Matrix4x4 matrix)
    {
        return new Vector3(matrix.M13, matrix.M23, matrix.M33);
    }
    public  float AzCalculator(Vector2 markerANormalizedImagePosition, Vector2 markerBNormalizedImagePosition, Vector3 markerACoordinateAtObjectReferenceFrame, Vector3 markerBCoordinateAtObjectReferenceFrame, Matrix4x4 RotationMatrix)
    {
        var numerator = Vector3.Dot( GetColumn1(RotationMatrix) - markerBNormalizedImagePosition.X*GetColumn3(RotationMatrix) , markerACoordinateAtObjectReferenceFrame-markerBCoordinateAtObjectReferenceFrame );
        var denominator = markerBNormalizedImagePosition.X - markerANormalizedImagePosition.Y;

        return numerator / denominator;
    }
}