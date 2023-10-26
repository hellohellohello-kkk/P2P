using System.Numerics;

namespace P2P.Tests;

[TestFixture]
public class AlphaCalculatorTest
{
	[Test]
	public void ParameterATest()
	{
		var alphaCalculator = new AlphaCalculator(Vector3.One, Vector3.Zero);
		var alpha = alphaCalculator.CalculateAlpha(new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 0.0f), new Vector4(0.0f, 0.0f, 0.0f, 0.0f));
	
		//Failed
		Assert.AreEqual(0, alpha);
	}

    [Explicit]
    [Test]
    public void P2pTest()
    {
        //定数設定
        var f = 50; //mm
        var dpx = 0.00345f; //mm/pix
        var imageCenter = new Vector2(1024, 1024);

        //正解となる外部Pを設定(回転)
        var expectedMatrix = Matrix4x4.CreateRotationX(MathF.PI/4);
        
        //オブジェクト
        var objectAInObjectReferenceFrame = new Vector4(100, 100, 0, 1);
        var objectBInObjectReferenceFrame = new Vector4(-100, -100, 0, 1);

        //重力方向
        var gravityVectorInObjectReferenceFrame = new Vector4(0, 0, 1, 1);
        var gravityVectorInCameraReferenceFrame = Vector4.Transform(gravityVectorInObjectReferenceFrame, Matrix4x4.Transpose(expectedMatrix));

        //正解となる外部Pを設定（並進）
        expectedMatrix.M41 = 0;
        expectedMatrix.M42 = 0;
        expectedMatrix.M43 = 5000;

        var objectA = Vector4.Transform(objectAInObjectReferenceFrame, expectedMatrix);
        var objectB = Vector4.Transform(objectBInObjectReferenceFrame, expectedMatrix);

        
        var imagePositionA = ImageCalculator.CalculateImageCoordinates(f, dpx, imageCenter, objectA);
        var imagePositionB = ImageCalculator.CalculateImageCoordinates(f, dpx, imageCenter, objectB);

        var projectedImagePositionA = ImageCalculator.CalculateProjectionalCoordinates(f, dpx, imageCenter, imagePositionA);
        var projectedImagePositionB = ImageCalculator.CalculateProjectionalCoordinates(f, dpx, imageCenter, imagePositionB);


        var calculator = new AlphaCalculator(objectAInObjectReferenceFrame, objectBInObjectReferenceFrame);

        var alpha = calculator.CalculateAlpha(projectedImagePositionA, projectedImagePositionB, gravityVectorInObjectReferenceFrame, gravityVectorInCameraReferenceFrame);

        var alphaDegree = Angle.CreateFromDegree(alpha * 180.0 / Math.PI);

        Console.WriteLine(alphaDegree.Degree);

        var rotationMatrixCalculator = new RotationMatrixCalculator();
        var matrix1 = rotationMatrixCalculator.GetRotationMatrixCertainRefToObjectReferenceFrame(alphaDegree, gravityVectorInObjectReferenceFrame);
        var matrix2 = rotationMatrixCalculator.GetRotationMatrixCertainRefToCameraReferenceFrame(gravityVectorInCameraReferenceFrame);
        var actualMatrix = rotationMatrixCalculator.GetRotationObjectReferenceFrameToCameraReferenceFrame(matrix2, matrix1);

        Console.WriteLine(actualMatrix);

        var translation = TranslationVectorCalculator.GetTranslationVector(projectedImagePositionA, projectedImagePositionB, objectAInObjectReferenceFrame, objectBInObjectReferenceFrame, actualMatrix);
        Console.WriteLine(translation.X +","+translation.Y+","+translation.Z );
    }
}