using System.Numerics;

namespace P2P;

public static class Matrix4X4Extension
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