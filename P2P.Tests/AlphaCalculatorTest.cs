using System.Numerics;

namespace P2P.Tests;

[TestFixture]
public class AlphaCalculatorTest
{
	[Test]
	public void ParameterATest()
	{
		var alphaCalculator = new AlphaCalculator(Vector3.One, Vector3.Zero);
		var alpha = alphaCalculator.CalculateAlpha(new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f));
	
		//Failed
		Assert.AreEqual(0, alpha);
	}

    [Explicit]
    [Test]
    public void P2pTest()
    {
        var objectAInObjectReferenceFrame = new Vector4(100, 100, 100, 1);
        var objectBInObjectReferenceFrame = new Vector4(0, 0, 0, 1);

        var gravityVectorInCameraReferenceFrame = new Vector3(0, 0, 1);
        var gravityVectorInObjectReferenceFrame = new Vector3(0, 0, 1);

        //��]
        var expectedMatrix = Matrix4x4.Identity;
        //���i
        expectedMatrix.M41 = 10;
        expectedMatrix.M42 = 20;
        expectedMatrix.M43 = 30;

        var objectAVector4 = Vector4.Transform(objectAInObjectReferenceFrame, expectedMatrix);
        var expectedObjectAInCameraReferenceFrame = new Vector3(objectAVector4.X, objectAVector4.Y, objectAVector4.Z);

        var objectBVector4 = Vector4.Transform(objectBInObjectReferenceFrame, expectedMatrix);
        var expectedObjectBInCameraReferenceFrame = new Vector3(objectBVector4.X, objectBVector4.Y, objectBVector4.Z);

        var f = 50; //mm
        var dpx = 0.00345f; //mm/pix
        var imageCenter = new Vector2(1024, 1024);
        var imagePositionA = Class1.CalculateImageCoordinatesFromCameraCoordinates(f, dpx, imageCenter, expectedObjectAInCameraReferenceFrame);
        var imagePositionB = Class1.CalculateImageCoordinatesFromCameraCoordinates(f, dpx, imageCenter, expectedObjectBInCameraReferenceFrame);

        var projectedImagePositionA = Class1.CalculateProjectionalCoordinatesFromImageCoordinates(f, dpx, imageCenter, imagePositionA);
        var projectedImagePositionB = Class1.CalculateProjectionalCoordinatesFromImageCoordinates(f, dpx, imageCenter, imagePositionB);


        var calculator = new AlphaCalculator(objectAInObjectReferenceFrame, objectBInObjectReferenceFrame);

        var alpha = calculator.CalculateAlpha(projectedImagePositionA, projectedImagePositionB, gravityVectorInObjectReferenceFrame, gravityVectorInCameraReferenceFrame);

        var alphaDegree = Angle.CreateFromDegree(alpha * 180.0 / Math.PI);

        Console.WriteLine(alphaDegree);

        var rotationMatrixCalculator = new RotationMatrixCalculator();
        var actualMatrix = rotationMatrixCalculator.GetRotationMatrixCertainRefToObjectReferenceFrame(alphaDegree, gravityVectorInCameraReferenceFrame);

        Console.WriteLine(actualMatrix);

    }
}