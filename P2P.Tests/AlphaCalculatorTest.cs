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

    [Explicit]
    [Test]
    public void P2pTest()
    {
        //�萔�ݒ�
        var f = 50; //mm
        var dpx = 0.00345f; //mm/pix
        var imageCenter = new Vector2(1024, 1024);

        //�����ƂȂ�O��P��ݒ�(��])
        var expectedMatrix = Matrix4x4.CreateRotationX(MathF.PI/4);

        //�I�u�W�F�N�g
        var objectAInObjectReferenceFrame = new Vector4(100, 100, 0, 1);
        var objectBInObjectReferenceFrame = new Vector4(-100, -100, 0, 1);

        //�d�͕���
        var gravityVectorInObjectReferenceFrame = new Vector4(0, 0, 1, 1);
        var gravityVectorInCameraReferenceFrame = Vector4.Transform(gravityVectorInObjectReferenceFrame, Matrix4x4.Transpose(expectedMatrix));

        //�����ƂȂ�O��P��ݒ�i���i�j
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
        var comparisonResult1 = new CompareLogic() { Config = new ComparisonConfig() { DoublePrecision = 0.001 } }.Compare(expectedMatrix.GetDoubleList(), actualMatrix1.GetDoubleList());
        var comparisonResult2 = new CompareLogic() { Config = new ComparisonConfig() { DoublePrecision = 0.001 } }.Compare(expectedMatrix.GetDoubleList(), actualMatrix2.GetDoubleList());
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