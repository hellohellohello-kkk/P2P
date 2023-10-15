using System.Numerics;

namespace P2P.Tests;

[TestFixture]
public class RotationCalculatorTest
{
	[Test]
	public void RotationMatrixInnerProductTest()
	{
		var actual =
			new RotationMatrixCalculator().GetRotationMatrixObjectToCertainRef(Angle.CreateFromDegree(0),
				new Vector3(0, 0, 0));
	
		// Failed
		Assert.AreEqual(Matrix4x4.Identity, actual);
	}

}