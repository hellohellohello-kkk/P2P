using System.Numerics;

namespace P2P;

public class RotationMatrixCalculator
{
	public Matrix4x4 GetRotationMatrixCertainRefToObjectReferenceFrame(Angle angle, Vector3 gravity)
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
	
	public Matrix4x4 GetRotationMatrixCertainRefToCameraReferenceFrame(Vector3 gravity)
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
		return RotationMatrixCertainRefToCameraReferenceFrame * Matrix4x4.Transpose(RotationMatrixCertainRefToObjectReferenceFrame);
	}

	private static float CalculateDenominator(Vector3 gravity)
	{
		return FloatMath.Sqrt(gravity.X*gravity.X+ gravity.Z*gravity.Z);
	}
}