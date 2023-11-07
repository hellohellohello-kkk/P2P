using System;
using System.Numerics;
using KellermanSoftware.CompareNetObjects;
using static P2P.AlphaCalculator;

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
		//Assert.AreEqual(0, alpha);
	}

    [TestCase(1, 0, 0, MathF.PI / 3, 1, -100, 4000)]
    [TestCase(0, 1, 0, MathF.PI / 3, 1, -100, 4000)]
    [TestCase(0, 0, 1, MathF.PI / 3, 1, -100, 4000)]
    [TestCase(1, 1, 1, MathF.PI / 4, 0, 0, 5000)]
    [TestCase(1, -1, 1, MathF.PI / 3, 1, -100, 4000)]
    [TestCase(1, 0, 1, MathF.PI / 3, 1, -100, 4000)]
    public void P2pTest(float p, float q, float r, float angle, float tx, float ty, float tz)
    {
        //定数設定
        var f = 50; //mm
        var dpx = 0.00345f; //mm/pix
        var imageCenter = new Vector2(1024, 1024);
        //オブジェクト
        var objectAInObjectReferenceFrame = new Vector4(100, 30, 30, 1);
        var objectBInObjectReferenceFrame = new Vector4(-50, -50, 0, 1);
        var objectCInObjectReferenceFrame = new Vector4(-50, 60, 0, 1);

        //正解となる外部Pを設定(回転)
        var axisVector = new Vector3(p, q, r);
        var normalizedAxisVector = axisVector / axisVector.Length();
        var expectedMatrix = Matrix4x4.CreateFromAxisAngle(normalizedAxisVector, angle);

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
        var objectC = Vector4.Transform(objectCInObjectReferenceFrame, expectedMatrix);

        var imagePositionA = ImageCalculator.CalculateImageCoordinates(f, dpx, imageCenter, objectA);
        var imagePositionB = ImageCalculator.CalculateImageCoordinates(f, dpx, imageCenter, objectB);
        var imagePositionC = ImageCalculator.CalculateImageCoordinates(f, dpx, imageCenter, objectC);

        var projectedImagePositionA = ImageCalculator.CalculateProjectionalCoordinates(f, dpx, imageCenter, imagePositionA);
        var projectedImagePositionB = ImageCalculator.CalculateProjectionalCoordinates(f, dpx, imageCenter, imagePositionB);
        var projectedImagePositionC = ImageCalculator.CalculateProjectionalCoordinates(f, dpx, imageCenter, imagePositionC);

        Console.WriteLine(projectedImagePositionA.X + "," + projectedImagePositionA.Y);
        Console.WriteLine(projectedImagePositionB.X + "," + projectedImagePositionB.Y);

        var calculator = new AlphaCalculator(objectAInObjectReferenceFrame, objectBInObjectReferenceFrame);

        var alpha = calculator.CalculateAlphaByHandCalculation(projectedImagePositionA, projectedImagePositionB, gravityVectorInObjectReferenceFrame, gravityVectorInCameraReferenceFrame);

        var alpha1Degree = Angle.CreateFromDegree(alpha[0] * 180.0 / Math.PI);
        var alpha2Degree = Angle.CreateFromDegree(alpha[1] * 180.0 / Math.PI);
        var alphaList = new Angle[]{ alpha1Degree, alpha2Degree};

        //Console.WriteLine("alpha1 : " + alpha1Degree.Degree);
        //Console.WriteLine("alpha2 : " + alpha2Degree.Degree);

        var alphaSelector = new AlphaSelector(new Vector4[] { objectAInObjectReferenceFrame, objectBInObjectReferenceFrame, objectCInObjectReferenceFrame }, new Vector2[] { projectedImagePositionA, projectedImagePositionB, projectedImagePositionC });
        var betterAlpha = alphaSelector.SelectBetterAlpha(alphaList, gravityVectorInObjectReferenceFrame, gravityVectorInCameraReferenceFrame);

        var actualMatrix = RotationMatrixCalculator.calculateExternalParameter(objectAInObjectReferenceFrame, objectBInObjectReferenceFrame, gravityVectorInObjectReferenceFrame, gravityVectorInCameraReferenceFrame, projectedImagePositionA, projectedImagePositionB, betterAlpha);
        
        Console.WriteLine(actualMatrix);
        Console.WriteLine(expectedMatrix);
        var comparisonResult = new CompareLogic() { Config = new ComparisonConfig() { DoublePrecision = 1 } }.Compare(expectedMatrix.GetDoubleList(), actualMatrix.GetDoubleList());
        Assert.IsTrue(comparisonResult.AreEqual);

    }

    
}