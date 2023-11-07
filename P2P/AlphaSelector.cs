using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using static P2P.AlphaCalculator;

namespace P2P;

public class AlphaSelector
{
    private readonly Marker _largeA;
    private readonly Marker _largeB;
    private readonly Marker _largeC;
    private readonly Vector2 _projectedA;
    private readonly Vector2 _projectedB;
    private readonly Vector2 _projectedC;

    public AlphaSelector(Vector4[] markersInObjectFrame, Vector2[] markerProjectedPosition)
    {
        _largeA = new Marker(markersInObjectFrame[0]);
        _largeB = new Marker(markersInObjectFrame[1]);
        _largeC = new Marker(markersInObjectFrame[2]);

        _projectedA = markerProjectedPosition[0];
        _projectedB = markerProjectedPosition[1];
        _projectedC = markerProjectedPosition[2];
    }

    public Angle SelectBetterAlpha(Angle[] alphaList, Vector4 gravityVectorInObjectFrame, Vector4 gravityVectorInCameraframe)
    {
        double error1 = CalculateProjectionError(alphaList[0], gravityVectorInObjectFrame, gravityVectorInCameraframe);
        double error2 = CalculateProjectionError(alphaList[1], gravityVectorInObjectFrame, gravityVectorInCameraframe);

        if (error1 < error2)
        {
            return alphaList[0];
        }
        return alphaList[1];
    }


    private double CalculateProjectionError(Angle alpha, Vector4 gravityVectorInObjectFrame, Vector4 gravityVectorInCameraframe)
    {
        var markerA = ToVector4(_largeA);
        var markerB = ToVector4(_largeB);
        var markerC = ToVector4(_largeC);
        var externalParameter = RotationMatrixCalculator.calculateExternalParameter(markerA, markerB, gravityVectorInObjectFrame, gravityVectorInCameraframe, _projectedA, _projectedB, alpha);

        var expectedProjectedC = CalculateProjectedCoordinate(externalParameter, markerC);

        var error = CalculateReProjectedError(_projectedC, expectedProjectedC);

        return error;
    }

    private double CalculateReProjectedError(Vector2 e1, Vector2 e2)
    {
        var errorX = e1.X - e2.X;
        var errorY = e1.Y - e2.Y;
        return Math.Sqrt(errorX*errorX + errorY*errorY);
    }

    public static Vector2 CalculateProjectedCoordinate(Matrix4x4 externalParameter, Vector4 marker)
    {
        var markerInCameraFrame = Vector4.Transform(marker, externalParameter);

        var projecedPositon = new Vector2(markerInCameraFrame.X / markerInCameraFrame.Z, markerInCameraFrame.Y / markerInCameraFrame.Z);

        return projecedPositon;
    }


    public static Vector4 ToVector4(Marker v)
    {
        return new Vector4(v.U, v.V, v.W, 1f);
    }
}
    