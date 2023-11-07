using System.Numerics;

namespace P2P.Tests;

[TestFixture]
public class AlphaSelectorTest
{
	[Test]
	public void CalculateProjectedCoordinateTest()
	{
		var externalParameter = Matrix4x4.CreateRotationZ(MathF.PI/4);
		var marker = new Vector4(1,2,3,0);

        var actual = AlphaSelector.CalculateProjectedCoordinate(externalParameter, marker);

		var expected = new Vector2(1/MathF.Sqrt(2), 1f / 3f /MathF.Sqrt(2));
		
		//Assert.AreEqual(expected, actual, 0.001f);
	}
	

}