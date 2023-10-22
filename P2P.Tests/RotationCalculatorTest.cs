using System.Numerics;

namespace P2P.Tests;

[TestFixture]
public class RotationCalculatorTest
{
	[Test]
	public void RotationMatrixObjectGravityTest()
	{
		var rotationMatrixCalculator = new RotationMatrixCalculator();
		var actual =
			rotationMatrixCalculator.GetRotationMatrixCertainRefToObjectReferenceFrame(Angle.CreateFromDegree(0),
				new Vector3(0, 0, 0));
	
		// Failed
		Assert.AreEqual(Matrix4x4.Identity, actual);
	}
	
	[Test]
	public void RotationMatrixCameraGravityTest()
	{
		var rotationMatrixCalculator = new RotationMatrixCalculator();
		var actual =
			rotationMatrixCalculator.GetRotationMatrixCertainRefToCameraReferenceFrame(new Vector3(0, 0, 0));
	
		// Failed
		Assert.AreEqual(Matrix4x4.Identity, actual);
	}

}