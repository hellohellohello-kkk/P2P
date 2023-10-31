using System;
using System.Numerics;
using KellermanSoftware.CompareNetObjects;

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

    [TestCase(0, 0, 0, 5000)]
    [TestCase(MathF.PI / 6, 0, 1000, 4000)]
    [TestCase(MathF.PI / 4, 23, 523, 10000)]
    [TestCase(MathF.PI / 3, 100, 0, 8000)]
    [TestCase(MathF.PI / 2, 55, 230, 5000)]
    public void P2pTest(float radian, float tx, float ty, float tz)
    {
        //定数設定
        var f = 50; //mm
        var dpx = 0.00345f; //mm/pix
        var imageCenter = new Vector2(1024, 1024);

        //正解となる外部Pを設定(回転)
        var expectedMatrix = Matrix4x4.CreateRotationZ(radian);

        //オブジェクト
        var objectAInObjectReferenceFrame = new Vector4(100, 30, 30, 1);
        var objectBInObjectReferenceFrame = new Vector4(-50, -50, 0, 1);

        //重力方向
        var gravityVectorInObjectReferenceFrame = new Vector4(0, 0, 1, 1);
        var gravityVectorInCameraReferenceFrame = Vector4.Transform(gravityVectorInObjectReferenceFrame, expectedMatrix);
    
        Console.WriteLine(gravityVectorInCameraReferenceFrame.ToString());
        
        //正解となる外部Pを設定（並進）
        expectedMatrix.M41 = tx;
        expectedMatrix.M42 = ty;
        expectedMatrix.M43 = tz;

        var objectA = Vector4.Transform(objectAInObjectReferenceFrame, expectedMatrix);
        var objectB = Vector4.Transform(objectBInObjectReferenceFrame, expectedMatrix);

        var imagePositionA = ImageCalculator.CalculateImageCoordinates(f, dpx, imageCenter, objectA);
        var imagePositionB = ImageCalculator.CalculateImageCoordinates(f, dpx, imageCenter, objectB);

        var projectedImagePositionA = ImageCalculator.CalculateProjectionalCoordinates(f, dpx, imageCenter, imagePositionA);
        var projectedImagePositionB = ImageCalculator.CalculateProjectionalCoordinates(f, dpx, imageCenter, imagePositionB);

        Console.WriteLine(projectedImagePositionA.X + "," + projectedImagePositionA.Y);
        Console.WriteLine(projectedImagePositionB.X + "," + projectedImagePositionB.Y);

        var calculator = new AlphaCalculator(objectAInObjectReferenceFrame, objectBInObjectReferenceFrame);

        var alpha = calculator.CalculateAlphaByHandCalculation(projectedImagePositionA, projectedImagePositionB, gravityVectorInObjectReferenceFrame, gravityVectorInCameraReferenceFrame);

        var alpha1Degree = Angle.CreateFromDegree(alpha[0] * 180.0 / Math.PI);
        var alpha2Degree = Angle.CreateFromDegree(alpha[1] * 180.0 / Math.PI);

        Console.WriteLine("alpha1 : " + alpha1Degree.Degree);
        Console.WriteLine("alpha2 : " + alpha2Degree.Degree);

        var actualMatrix1 = calculateMatrix(objectAInObjectReferenceFrame, objectBInObjectReferenceFrame, gravityVectorInObjectReferenceFrame, gravityVectorInCameraReferenceFrame, projectedImagePositionA, projectedImagePositionB, alpha1Degree);
        var actualMatrix2 = calculateMatrix(objectAInObjectReferenceFrame, objectBInObjectReferenceFrame, gravityVectorInObjectReferenceFrame, gravityVectorInCameraReferenceFrame, projectedImagePositionA, projectedImagePositionB, alpha2Degree);

        Console.WriteLine(actualMatrix1);
        Console.WriteLine(actualMatrix2);
        Console.WriteLine(expectedMatrix);
        var comparisonResult1 = new CompareLogic() { Config = new ComparisonConfig() { DoublePrecision = 0.01 } }.Compare(expectedMatrix.GetDoubleList(), actualMatrix1.GetDoubleList());
        var comparisonResult2 = new CompareLogic() { Config = new ComparisonConfig() { DoublePrecision = 0.01 } }.Compare(expectedMatrix.GetDoubleList(), actualMatrix2.GetDoubleList());
        Assert.IsTrue(comparisonResult1.AreEqual || comparisonResult2.AreEqual);

    }

    private static Matrix4x4 calculateMatrix(Vector4 objectAInObjectReferenceFrame, Vector4 objectBInObjectReferenceFrame, Vector4 gravityVectorInObjectReferenceFrame, Vector4 gravityVectorInCameraReferenceFrame, Vector2 projectedImagePositionA, Vector2 projectedImagePositionB, Angle alpha1Degree)
    {
        var rotationMatrixCalculator = new RotationMatrixCalculator();
        var matrix1 = rotationMatrixCalculator.GetRotationMatrixCertainRefToObjectReferenceFrame(alpha1Degree, gravityVectorInObjectReferenceFrame);
        var matrix2 = rotationMatrixCalculator.GetRotationMatrixCertainRefToCameraReferenceFrame(gravityVectorInCameraReferenceFrame);
        var actualMatrix = rotationMatrixCalculator.GetRotationObjectReferenceFrameToCameraReferenceFrame(matrix2, matrix1);
        var translation = TranslationVectorCalculator.GetTranslationVector(projectedImagePositionA, projectedImagePositionB, objectAInObjectReferenceFrame, objectBInObjectReferenceFrame, actualMatrix);
        actualMatrix.M41 = translation.X;
        actualMatrix.M42 = translation.Y;
        actualMatrix.M43 = translation.Z;
        return actualMatrix;
    }
}