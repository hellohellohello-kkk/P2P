using System.ComponentModel;
using System.Numerics;

namespace P2P;

public class AlphaCalculator
{
	private readonly Vector3 _markerACoordinateAtObjectReferenceFrame;
	private readonly Vector3 _markerBCoordinateAtObjectReferenceFrame;

	public AlphaCalculator(Vector3 markerACoordinateAtObjectReferenceFrame, Vector3 markerBCoordinateAtObjectReferenceFrame)
	{
		_markerACoordinateAtObjectReferenceFrame = markerACoordinateAtObjectReferenceFrame;
		_markerBCoordinateAtObjectReferenceFrame = markerBCoordinateAtObjectReferenceFrame;
	}
    public AlphaCalculator(Vector4 markerACoordinateAtObjectReferenceFrame, Vector4 markerBCoordinateAtObjectReferenceFrame)
    {
        _markerACoordinateAtObjectReferenceFrame = new Vector3(markerACoordinateAtObjectReferenceFrame.X,
            markerACoordinateAtObjectReferenceFrame.Y, markerACoordinateAtObjectReferenceFrame.Z);
        _markerBCoordinateAtObjectReferenceFrame = new Vector3(markerBCoordinateAtObjectReferenceFrame.X,
            markerBCoordinateAtObjectReferenceFrame.Y, markerBCoordinateAtObjectReferenceFrame.Z);
    }

    public double CalculateAlpha(Vector2 markerAPosition, Vector2 markerBPosition, Vector4 gravityObject, Vector4 gravityCamera)
	{
		var a = CalculateParameterA(markerAPosition, markerBPosition, gravityObject, gravityCamera);
		var b = CalculateParameterB(markerAPosition, markerBPosition, gravityObject, gravityCamera);
		var c = CalculateParameterC(markerAPosition, markerBPosition, gravityObject, gravityCamera);

		var beta = Math.Atan2(a, b);
		var m = Math.Sqrt(a * a + b * b);
		
		var alpha = Math.Acos(-c / m) + beta;

		return alpha;
	}

	private double CalculateParameterA(Vector2 markerAPosition, Vector2 markerBPosition, Vector4 gravityObject, Vector4 gravityCamera)
	{
		var xDiff = markerAPosition.X - markerBPosition.X;
		var yDiff = markerAPosition.Y - markerBPosition.Y;

		var a = 1 / xDiff * ((gravityObject.X*gravityObject.X + gravityObject.Y*gravityObject.Y) * gravityCamera.Y * (-_markerACoordinateAtObjectReferenceFrame.Y * gravityCamera.X + _markerBCoordinateAtObjectReferenceFrame.Y * gravityCamera.X + _markerACoordinateAtObjectReferenceFrame.Y * markerAPosition.X * gravityCamera.Z - 
		                                                                 _markerBCoordinateAtObjectReferenceFrame.Y * markerBPosition.X * gravityCamera.Z) +
		                     _markerACoordinateAtObjectReferenceFrame.X * (gravityObject.Z * (gravityCamera.X * markerAPosition.X + gravityCamera.Z) +
		                                                                  gravityObject.X * gravityObject.Y * gravityCamera.Y * (gravityCamera.X - markerAPosition.X * gravityCamera.Z)) -
		                     _markerBCoordinateAtObjectReferenceFrame.X * (gravityObject.Z * (gravityCamera.X * markerBPosition.X + gravityCamera.Z) +
		                                                                  gravityObject.X * gravityObject.Y * gravityCamera.Y * (gravityCamera.X - markerBPosition.X * gravityCamera.Z))) - 
		                     1 / yDiff *
		                     (gravityObject.Z * (gravityCamera.X * (_markerACoordinateAtObjectReferenceFrame.X * markerAPosition.Y - _markerBCoordinateAtObjectReferenceFrame.X * markerBPosition.Y) -
		                                         _markerBCoordinateAtObjectReferenceFrame.Y * gravityObject.Z * (gravityCamera.X * gravityCamera.X + gravityCamera.Y * markerBPosition.Y * gravityCamera.Z + gravityCamera.Z * gravityCamera.Z) +
		                                         _markerACoordinateAtObjectReferenceFrame.Y * gravityObject.Z * (gravityCamera.X * gravityCamera.X + gravityCamera.Z * (gravityCamera.Y * markerAPosition.Y + gravityCamera.Z))) +
		                      gravityObject.X * gravityObject.Y * (-_markerACoordinateAtObjectReferenceFrame.X * (gravityCamera.X * gravityCamera.X + gravityCamera.Z * (gravityCamera.Y * markerAPosition.Y + gravityCamera.Z)) +
		                                                           _markerBCoordinateAtObjectReferenceFrame.Y * (gravityCamera.X * gravityCamera.X + gravityCamera.Z * (gravityCamera.Y * markerBPosition.Y + gravityCamera.Z))) +
		                      gravityObject.X*gravityObject.X * (_markerACoordinateAtObjectReferenceFrame.Y * (gravityCamera.X * gravityCamera.X + gravityCamera.Z * (gravityCamera.Y * markerAPosition.Y + gravityCamera.Z)) -
		                                                         _markerBCoordinateAtObjectReferenceFrame.Y * (gravityCamera.X * gravityCamera.X + gravityCamera.Z * (gravityCamera.Y * markerBPosition.Y + gravityCamera.Z))));

		return a;
	}

	private double CalculateParameterB(Vector2 markerAPosition, Vector2 markerBPosition, Vector4 gravityObject, Vector4 gravityCamera)
    {
	    var xDiff = markerAPosition.X - markerBPosition.X;
        var yDiff = markerAPosition.Y - markerBPosition.Y;

        var b = 1 / xDiff * (gravityObject.X * gravityObject.Y * (-_markerACoordinateAtObjectReferenceFrame.X * (gravityCamera.X * markerAPosition.X + gravityCamera.Z) + 
                                                                  _markerBCoordinateAtObjectReferenceFrame.X * (gravityCamera.X * markerBPosition.Y + gravityCamera.Z)) +
                             gravityObject.X * gravityObject.X * (_markerACoordinateAtObjectReferenceFrame.Y * (gravityCamera.X * markerAPosition.X + gravityCamera.Z) - 
                                                                          _markerBCoordinateAtObjectReferenceFrame.Y * (gravityCamera.X * markerBPosition.X + gravityCamera.Z) +
                                                                          gravityObject.Z * gravityCamera.Y * (_markerACoordinateAtObjectReferenceFrame.X * gravityCamera.X - 
	                                                                          _markerBCoordinateAtObjectReferenceFrame.X * gravityCamera.X - 
	                                                                          _markerACoordinateAtObjectReferenceFrame.X * markerAPosition.X * gravityCamera.Z + 
	                                                                          _markerBCoordinateAtObjectReferenceFrame.X * markerBPosition.X * gravityCamera.Z)) +
                             gravityObject.Z * (_markerACoordinateAtObjectReferenceFrame.Y * gravityObject.Z * (gravityCamera.X * markerAPosition.X + gravityCamera.Z) - 
                                                _markerBCoordinateAtObjectReferenceFrame.Y * gravityObject.Z * (gravityCamera.X * markerBPosition.X + gravityCamera.Z) +
                                                (gravityObject.Y * gravityObject.Y + gravityObject.Z * gravityObject.Z) * gravityCamera.Y *
                                                (-_markerBCoordinateAtObjectReferenceFrame.X * gravityCamera.X + _markerBCoordinateAtObjectReferenceFrame.X * markerBPosition.X * gravityCamera.Z + 
                                                 _markerACoordinateAtObjectReferenceFrame.X * (gravityCamera.X - markerAPosition.X * gravityCamera.Z)))) -
                             1 / yDiff *
                             (gravityObject.X * gravityObject.Y * gravityCamera.X * (-_markerACoordinateAtObjectReferenceFrame.X * markerAPosition.Y + 
                                                                                     _markerBCoordinateAtObjectReferenceFrame.X * markerBPosition.Y) +
                              gravityObject.X * gravityObject.X * (gravityCamera.X * (_markerACoordinateAtObjectReferenceFrame.Y * markerAPosition.Y - 
	                                                                           _markerBCoordinateAtObjectReferenceFrame.Y * markerBPosition.Y) +
                                                                           _markerBCoordinateAtObjectReferenceFrame.X * gravityObject.Z * 
                                                                           (gravityCamera.X * gravityCamera.X + gravityCamera.Y * markerBPosition.Y * gravityCamera.Z + 
                                                                            gravityCamera.Z * gravityCamera.Z) -
                                                                           _markerACoordinateAtObjectReferenceFrame.X * 
                                                                           (gravityObject.Y * gravityObject.Y + gravityObject.Z * gravityObject.Z) *
                                                                           (gravityCamera.X * gravityCamera.X + gravityCamera.Y * markerAPosition.Y * gravityCamera.Z + 
                                                                            gravityCamera.Z * gravityCamera.Z) + 
                                                                           _markerBCoordinateAtObjectReferenceFrame.X *
                                                                           (gravityObject.Y * gravityObject.Y + gravityObject.Z * gravityObject.Z) *
                                                                           (gravityCamera.X * gravityCamera.X + gravityCamera.Y * markerBPosition.Y * gravityCamera.Z + 
                                                                            gravityCamera.Z * gravityCamera.Z)));

        return b;
    }

	private double CalculateParameterC(Vector2 markerAPosition, Vector2 markerBPosition, Vector4 gravityObject, Vector4 gravityCamera)
    {
	    var xDiff = markerAPosition.X - markerBPosition.X;
        var yDiff = markerAPosition.Y - markerBPosition.Y;

        var c = Math.Sqrt(Vector4Dot(gravityObject, gravityObject) * Vector4Dot(gravityCamera, gravityCamera)) / xDiff *
                (gravityObject.X * (-_markerBCoordinateAtObjectReferenceFrame.X * gravityCamera.X +
                                    _markerBCoordinateAtObjectReferenceFrame.X * markerBPosition.X * gravityCamera.Z +
                                    _markerACoordinateAtObjectReferenceFrame.X * (gravityCamera.X - markerAPosition.X * gravityCamera.Z)) +
                 gravityObject.Y * (-_markerBCoordinateAtObjectReferenceFrame.Y * gravityCamera.X +
                                    _markerBCoordinateAtObjectReferenceFrame.Y * markerBPosition.X * gravityCamera.Z +
                                    _markerACoordinateAtObjectReferenceFrame.Y * (gravityCamera.X - markerAPosition.X * gravityCamera.Z))) -
                Math.Sqrt(Vector4Dot(gravityObject, gravityObject) * Vector4Dot(gravityCamera, gravityCamera)) / yDiff *
                (gravityObject.X * (-_markerBCoordinateAtObjectReferenceFrame.X * gravityCamera.Y +
                                    _markerBCoordinateAtObjectReferenceFrame.X * markerBPosition.Y * gravityCamera.Z +
                                    _markerACoordinateAtObjectReferenceFrame.X * (gravityCamera.Y - markerAPosition.Y * gravityCamera.Z)) +
                 gravityObject.Y * (-_markerBCoordinateAtObjectReferenceFrame.Y * gravityCamera.Y +
                                    _markerBCoordinateAtObjectReferenceFrame.Y * markerBPosition.Y * gravityCamera.Z +
                                    _markerACoordinateAtObjectReferenceFrame.Y * (gravityCamera.Y - markerAPosition.Y * gravityCamera.Z)));

        return c;
    }

    public static float Vector4Dot(Vector4 vec1, Vector4 vec2)
    {
        return (vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z + vec2.Z);
    }
}