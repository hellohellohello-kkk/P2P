using System.Numerics;

namespace P2P;

public class Angle
{
	private Angle(double degree)
	{
		Degree = degree;
	}

	public static Angle CreateFromDegree(double degree)
	{
		return new Angle(degree);
	}

	public double Degree { get; }

	public double Radian => Degree / 180 * Math.PI;
}

public class RotationMatrixCalculator
{
	public Matrix4x4 GetRotationMatrixObjectToCertainRef(Angle angle, Vector3 gravity)
	{
		var gObj = new Vector3(gravity.X, gravity.Y, gravity.Z );

		var denominator = FloatMath.Sqrt(Math.Pow(gravity.X, 2) + Math.Pow(gravity.Z, 2));
		var m1 = new Vector3(gravity.Z / denominator, 0, -gravity.X / denominator);
		var m2 = new Vector3(-gravity.X * gravity.Y / denominator, denominator, -gravity.Y * gravity.Z / denominator);
		var mObj = m1 * FloatMath.Sin(angle.Radian) + m2 * FloatMath.Cos(angle.Radian);

		var nObj = new Vector3(
			(-gravity.Z * FloatMath.Cos(angle.Radian) + gravity.X * gravity.Y * FloatMath.Sin(angle.Radian)) /
			denominator,
			denominator * FloatMath.Sin(angle.Radian),
			(gravity.X * FloatMath.Cos(angle.Radian) - gravity.Y * gravity.Z * FloatMath.Sin(angle.Radian)) /
			denominator);

		return Matrix4x4Extension.CreateFromThreeVector(gObj, mObj, nObj);
	}
}

public class FloatMath
{
	public static float Sin(double value)
	{
		return (float)Math.Sin(value);
	}
	
	public static float Cos(double value)
	{
		return (float)Math.Cos(value);
	}

	public static float Sqrt(double value)
	{
		return (float)Math.Sqrt(value);
	}
}

public static class Matrix4x4Extension
{
	public static Matrix4x4 CreateFromThreeVector(Vector3 firstRow, Vector3 secondRow, Vector3 thirdRow)
	{
		return new Matrix4x4(
			firstRow.X, firstRow.Y, firstRow.Z, 0, 
			secondRow.X, secondRow.Y, secondRow.Z, 0,
			thirdRow.X, thirdRow.Y, thirdRow.Z, 0, 
			0, 0, 0, 1
		);
	}
}


