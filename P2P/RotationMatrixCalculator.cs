using System.Numerics;

namespace P2P;

public class RotationMatrixCalculator
{
	public Matrix4x4 GetRotationMatrixCertainRefToObjectReferenceFrame(Angle angle, Vector4 gravity)
	{
		var gObj = new Vector3(gravity.X, gravity.Y, gravity.Z );
		
		var denominator = CalculateDenominator(gravity);
		
		var m1 = new Vector3(gravity.Z / denominator, 0, -gravity.X / denominator);
		var m2 = new Vector3(-gravity.X * gravity.Y / denominator, denominator, -gravity.Y * gravity.Z / denominator);
		var mObj = m1 * FloatMath.Sin(angle.Radian) + m2 * FloatMath.Cos(angle.Radian);

		var nObj = new Vector3(
			(-gravity.Z * FloatMath.Cos(angle.Radian) + gravity.X * gravity.Y * FloatMath.Sin(angle.Radian)) /
			denominator,
			denominator * FloatMath.Sin(angle.Radian),
			(gravity.X * FloatMath.Cos(angle.Radian) - gravity.Y * gravity.Z * FloatMath.Sin(angle.Radian)) /
			denominator);

		return Matrix4X4Extension.CreateFromThreeVector(gObj, mObj, nObj);
	}
	
	public Matrix4x4 GetRotationMatrixCertainRefToCameraReferenceFrame(Vector4 gravity)
	{
		var gCam = new Vector3(gravity.X, gravity.Y, gravity.Z);

		var denominator = CalculateDenominator(gravity);

		var mCam = new Vector3(gravity.Z / denominator, 0, -gravity.X / denominator);

		var nCam = new Vector3(
			-gravity.X * gravity.Y / denominator,
			denominator,
			-gravity.Y * gravity.Z / denominator);

		return Matrix4X4Extension.CreateFromThreeVector(gCam, mCam, nCam);
	}

	public Matrix4x4 GetRotationObjectReferenceFrameToCameraReferenceFrame(Matrix4x4 RotationMatrixCertainRefToCameraReferenceFrame, Matrix4x4 RotationMatrixCertainRefToObjectReferenceFrame)
	{
		var matrix = Matrix4x4.Transpose(RotationMatrixCertainRefToObjectReferenceFrame) * RotationMatrixCertainRefToCameraReferenceFrame;

        return matrix;
	}

	private float CalculateDenominator(Vector4 gravity)
	{
		return FloatMath.Sqrt(gravity.X*gravity.X+ gravity.Z*gravity.Z);
	}

    public static Matrix4x4 calculateExternalParameter(Vector4 objectAInObjectReferenceFrame, Vector4 objectBInObjectReferenceFrame, Vector4 gravityVectorInObjectReferenceFrame, Vector4 gravityVectorInCameraReferenceFrame, Vector2 projectedImagePositionA, Vector2 projectedImagePositionB, Angle alpha1Degree)
    {
        var rotationMatrixCalculator = new RotationMatrixCalculator();
        var rotationMatrixRefToObject = rotationMatrixCalculator.GetRotationMatrixCertainRefToObjectReferenceFrame(alpha1Degree, gravityVectorInObjectReferenceFrame);
        var rotationMatrizRefToCamera = rotationMatrixCalculator.GetRotationMatrixCertainRefToCameraReferenceFrame(gravityVectorInCameraReferenceFrame);
        var translation = TranslationVectorCalculator.GetTranslationVector(projectedImagePositionA, projectedImagePositionB, objectAInObjectReferenceFrame, objectBInObjectReferenceFrame, externalParameter);

        var externalParameter = rotationMatrixCalculator.GetRotationObjectReferenceFrameToCameraReferenceFrame(rotationMatrizRefToCamera, rotationMatrixRefToObject);
        externalParameter.M41 = translation.X;
        externalParameter.M42 = translation.Y;
        externalParameter.M43 = translation.Z;

        return externalParameter;
    }

}