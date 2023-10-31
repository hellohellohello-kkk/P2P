using System.Numerics;

namespace P2P.Tests;

internal static class TestingMatrix4X4Extension
{
	internal static IReadOnlyCollection<double> GetDoubleList(this Matrix4x4 original)
	{
		return new List<double>
		{
			original.M11, original.M12, original.M13, original.M14,
			original.M21, original.M22, original.M23, original.M24,
			original.M31, original.M32, original.M33, original.M34,
			original.M41, original.M42, original.M43, original.M44
		};
	}
}