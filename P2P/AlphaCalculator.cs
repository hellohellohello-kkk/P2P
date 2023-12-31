using System.ComponentModel;
using System.Numerics;

namespace P2P;

public class AlphaCalculator
{
	private readonly Marker _largeA;
	private readonly Marker _largeB;

	public AlphaCalculator(Vector3 markerACoordinateAtObjectReferenceFrame, Vector3 markerBCoordinateAtObjectReferenceFrame)
	{ 
		//TODO 謨ｴ蜷域ｧ縺悟叙繧後ｋ繧医≧縺ｫ縺吶ｋ
		_largeA = new Marker(markerACoordinateAtObjectReferenceFrame);
		_largeB = new Marker(markerBCoordinateAtObjectReferenceFrame);
	}
	
    public AlphaCalculator(Vector4 markerACoordinateAtObjectReferenceFrame, Vector4 markerBCoordinateAtObjectReferenceFrame)
    {
	    _largeA = new Marker(new Vector3(markerACoordinateAtObjectReferenceFrame.X,
		    markerACoordinateAtObjectReferenceFrame.Y, markerACoordinateAtObjectReferenceFrame.Z));
	    _largeB = new Marker(new Vector3(markerBCoordinateAtObjectReferenceFrame.X,
		    markerBCoordinateAtObjectReferenceFrame.Y, markerBCoordinateAtObjectReferenceFrame.Z));
    }

    public double[] CalculateAlpha(Vector2 xTildaA, Vector2 xTildaB, Vector4 gravityObject, Vector4 gravityCamera)
    {
		var aInImagePlane = new ImagePlanePoint(xTildaA);
		var bInImagePlane = new ImagePlanePoint(xTildaB);

		var gObj = new ObjectVector4(gravityObject);
		var gCam = new CameraVector4(gravityCamera);

		var a = CalculateParameterA(aInImagePlane, bInImagePlane, gObj, gCam);
		var b = CalculateParameterB(aInImagePlane, bInImagePlane, gObj, gCam);
		var c = CalculateParameterC(aInImagePlane, bInImagePlane, gObj, gCam);

		var beta = Math.Atan2(a, b);
		var m = Math.Sqrt(a * a + b * b);

        var alpha = new double[] { Math.Acos(-c / m) + beta, -Math.Acos(-c / m) + beta };

        return alpha;
	}

    public double[] CalculateAlphaByHandCalculation(Vector2 xTildaA, Vector2 xTildaB, Vector4 gravityObject, Vector4 gravityCamera)
    {
		var aInImagePlane = new ImagePlanePoint(xTildaA);
		var bInImagePlane = new ImagePlanePoint(xTildaB);

		var gObj = new ObjectVector4(gravityObject);
		var gCam = new CameraVector4(gravityCamera);

		var a = CalculateParameterAByHandCaluculation(aInImagePlane, bInImagePlane, gObj, gCam);
		var b = CalculateParameterBByHandCaluculation(aInImagePlane, bInImagePlane, gObj, gCam);
		var c = CalculateParameterCByHandCaluculation(aInImagePlane, bInImagePlane, gObj, gCam);

		var beta = Math.Atan2(a , b);
		var m = Math.Sqrt(a * a + b * b);

        var alpha = new double[] { Math.Acos(-c / m) + beta, -Math.Acos(-c / m) + beta };
        
		return alpha;
	}

	private double CalculateParameterA(ImagePlanePoint a, ImagePlanePoint b,  ObjectVector4 gObj, CameraVector4 gCam)
	{
		var xA = a.X;
		var xB = b.X;
		var xDiff = xA - xB;
		
		var yA = a.Y;
		var yB = b.Y;
		var yDiff = yA - yB;

		var returnValue = 1 / xDiff * ((gObj.U*gObj.U + gObj.V*gObj.V) * gCam.Y * (-_largeA.V * gCam.X + _largeB.V * gCam.X + _largeA.V * xA * gCam.Z - 
			                               _largeB.V * xB * gCam.Z) + _largeA.U * (gObj.W * (gCam.X * xA + gCam.Z) + gObj.U * gObj.V * gCam.Y * (gCam.X - xA * gCam.Z)) -
		                          _largeB.U * (gObj.W * (gCam.X * xB + gCam.Z) + gObj.U* gObj.V * gCam.Y * (gCam.X - xB * gCam.Z))) - 
		                     1 / yDiff * (gObj.W * (gCam.X * (_largeA.U * yA - _largeB.U * yB) - _largeB.V * gObj.W * (gCam.X * gCam.X + 
			                                            gCam.Y * yB * gCam.Z + gCam.Z * gCam.Z) + _largeA.V * gObj.W * (gCam.X * gCam.X + gCam.Z * (gCam.Y * yA + gCam.Z))) + gObj.U * gObj.V * (-_largeA.U * (gCam.X * gCam.X + 
				                                  gCam.Z * (gCam.Y * yA + gCam.Z)) + _largeB.V * (gCam.X * gCam.X + gCam.Z * (gCam.Y * yB + gCam.Z))) + gObj.U*gObj.U * (_largeA.V * (gCam.X * gCam.X + 
				                     gCam.Z * (gCam.Y * yA + gCam.Z)) - _largeB.V * (gCam.X * gCam.X + gCam.Z * (gCam.Y * yB + gCam.Z))));

		return returnValue;
	}

	private double CalculateParameterB(ImagePlanePoint a, ImagePlanePoint b,  ObjectVector4 gObj, CameraVector4 gCam)
    {
	    var xA = a.X;
        var xB = b.X;
        var xDiff = xA - xB;

        var yA = a.Y;
        var yB = b.Y;
        var yDiff = yA - yB;

        var returnValue = 1 / xDiff * (gObj.U * gObj.V * (-_largeA.U * (gCam.X * xA + gCam.Z) + _largeB.U * (gCam.X * _largeB.V + gCam.Z)) +
                             gObj.U * gObj.U * (_largeA.V * (gCam.X * xA + gCam.Z) - _largeB.V * (gCam.X * xB + gCam.Z) + gObj.W * gCam.Y * (_largeA.U * gCam.X - _largeB.U * gCam.X - 
	                             _largeA.U * xA * gCam.Z + _largeB.U * xB * gCam.Z)) + gObj.W * (_largeA.V * gObj.W * (gCam.X * xA+ gCam.Z) - 
	                             _largeB.V * gObj.W * (gCam.X * xB + gCam.Z) + (gObj.V * gObj.V + gObj.W * gObj.W) * gCam.Y * (-_largeB.U * gCam.X + _largeB.U * xB * gCam.Z + 
		                             _largeA.U * (gCam.X - xA * gCam.Z)))) - 1 / yDiff * (gObj.U * gObj.V * gCam.X * (-_largeA.U * yA + _largeB.U * yB) +
                              gObj.U * gObj.U * (gCam.X * (_largeA.V * yA - _largeB.V * yB) + _largeB.U * gObj.W * (gCam.X * gCam.X + gCam.Y * yB * gCam.Z + gCam.Z * gCam.Z) - 
	                              _largeA.U * (gObj.V * gObj.V + gObj.W * gObj.W) * (gCam.X * gCam.X + gCam.Z * (gCam.Y * yA + gCam.Z)) + _largeB.U * (gObj.V * gObj.V + gObj.W * gObj.W) * (gCam.X * gCam.X + 
		                              gCam.Z * (gCam.Y * yB + gCam.Z))));

        return returnValue;
    }

	private double CalculateParameterC(ImagePlanePoint a, ImagePlanePoint b,  ObjectVector4 gObj, CameraVector4 gCam)
    {
	    var xA = a.X;
	    var xB = b.X;
	    var xDiff = xA - xB;
	    
	    var yA = a.Y;
	    var yB = b.Y;
	    var yDiff = yA - yB;

	    var firstCoefficient = Math.Sqrt((gObj.U * gObj.U + gObj.W * gObj.W ) * ( gCam.X * gCam.X + gCam.Z * gCam.Z)) / xDiff;
	    var secondCoefficient = Math.Sqrt((gObj.U * gObj.U + gObj.W * gObj.W) * (gCam.X * gCam.X + gCam.Z * gCam.Z)) / yDiff;
	    var c = firstCoefficient * (gObj.U * (-_largeB.U * gCam.X + _largeB.U * xB * gCam.Z + _largeA.U * (gCam.X - 
		            xA * gCam.Z)) + gObj.V * (-_largeB.V * gCam.X + _largeB.V * xB * gCam.Z + _largeA.V * (gCam.X - xA * gCam.Z))) -
	            secondCoefficient * (gObj.U * (-_largeB.U * gCam.Y + _largeB.U * yB * gCam.Z + 
	                _largeA.U * (gCam.Y - yA * gCam.Z)) + gObj.V * (-_largeB.V * gCam.Y + _largeB.V * yB * gCam.Z + _largeA.V * (gCam.Y - yA * gCam.Z)));

        return c;
    }

    private double CalculateParameterAByHandCaluculation(ImagePlanePoint a, ImagePlanePoint b, ObjectVector4 gObj, CameraVector4 gCam)
    {
        var xA = a.X;
        var xB = b.X;
        var xDiff = xA - xB;

        var yA = a.Y;
        var yB = b.Y;
        var yDiff = yA - yB;

		var coefficientObj = Math.Sqrt(gObj.U * gObj.U + gObj.W * gObj.W);
		var coeffieientCam = Math.Sqrt(gCam.X * gCam.X + gCam.Z * gCam.Z);

        var firstCoefficient = 1 / xDiff;
        var secondCoefficient = 1 / yDiff;

        var difBAu = _largeB.U - _largeA.U;
        var difBAv = _largeB.V - _largeA.V;
        var difBAw = _largeB.W - _largeA.W;
        var difxAxBu = xA * _largeA.U - xB * _largeB.U;
        var difxAxBv = xA * _largeA.V - xB * _largeB.V;
        var difxAxBw = xA * _largeA.W - xB * _largeB.W;
        var difyAyBu = yA * _largeA.U - yB * _largeB.U;
        var difyAyBv = yA * _largeA.V - yB * _largeB.V;
        var difyAyBw = yA * _largeA.W - yB * _largeB.W;

        var returnValue = firstCoefficient *
                (
                    (gCam.Z * gObj.W + gCam.X * gCam.Y * gObj.U * gObj.V) * difBAu +
                    -gCam.X * gCam.Y * coefficientObj * coefficientObj * difBAv +
                    (-gCam.Z * gObj.U + gCam.X * gCam.Y * gObj.V * gObj.W) * difBAw +
                    (-gCam.X * gObj.W + gCam.Y * gCam.Z * gObj.U * gObj.V) * difxAxBu +
                    -gCam.Y * gCam.Z * coefficientObj * coefficientObj * difxAxBv +
                    (gCam.X * gObj.U + gCam.Y * gCam.Z * gObj.V * gObj.W) * difxAxBw
                )
                -
                secondCoefficient *
                (
                    -coeffieientCam * coeffieientCam * gObj.U * gObj.V * difBAu +
                    coeffieientCam * coeffieientCam * coefficientObj * coefficientObj * difBAv + 
					-coeffieientCam * coeffieientCam * gObj.V * gObj.W * difBAw+
                    (-gCam.X * gObj.W + gCam.Y * gCam.Z * gObj.U * gObj.V) * difyAyBu +
                    -gCam.Y * gCam.Z * coefficientObj * coefficientObj * difyAyBv +
                    (gCam.X * gObj.U + gCam.Y * gCam.Z * gObj.V * gObj.W) * difyAyBw
                );

        return returnValue;
    }

    private double CalculateParameterBByHandCaluculation(ImagePlanePoint a, ImagePlanePoint b, ObjectVector4 gObj, CameraVector4 gCam)
    {
        var xA = a.X;
        var xB = b.X;
        var xDiff = xA - xB;

        var yA = a.Y;
        var yB = b.Y;
        var yDiff = yA - yB;

        var coefficientObj = Math.Sqrt(gObj.U * gObj.U + gObj.W * gObj.W);
        var coeffieientCam = Math.Sqrt(gCam.X * gCam.X + gCam.Z * gCam.Z);

        var firstCoefficient = 1 / xDiff;
        var secondCoefficient = 1 / yDiff;

        var difBAu = _largeB.U - _largeA.U;
        var difBAv = _largeB.V - _largeA.V;
        var difBAw = _largeB.W - _largeA.W;
        var difxAxBu = xA * _largeA.U - xB * _largeB.U;
        var difxAxBv = xA * _largeA.V - xB * _largeB.V;
        var difxAxBw = xA * _largeA.W - xB * _largeB.W;
        var difyAyBu = yA * _largeA.U - yB * _largeB.U;
        var difyAyBv = yA * _largeA.V - yB * _largeB.V;
        var difyAyBw = yA * _largeA.W - yB * _largeB.W;

        var returnValue = firstCoefficient *
                (
                    (gCam.X * gCam.Y * gObj.W - gCam.Z * gObj.U * gObj.V) * difBAu +
                    gCam.Z * coefficientObj * coefficientObj * difBAv +
                    (-gCam.Z * gObj.V * gObj.W - gCam.X * gCam.Y * gObj.U) * difBAw +
                    (gCam.X * gObj.U * gObj.V + gCam.Y * gCam.Z * gObj.W) * difxAxBu +
                    -gCam.X * coefficientObj * coefficientObj * difxAxBv +
                    (gCam.X * gObj.U * gObj.W - gCam.Y * gCam.Z * gObj.W) * difxAxBw
                )
                -
                secondCoefficient *
                (
                    -coeffieientCam * coeffieientCam * gObj.W * difBAu +
                    0 * difBAv +
                    coeffieientCam * coeffieientCam * gObj.U * difBAw +
                    (gCam.X * gObj.U * gObj.V + gCam.Y * gCam.Z * gObj.W) * difyAyBu +
                    -gCam.X * coefficientObj * coefficientObj * difyAyBv +
                    (gCam.X * gObj.U * gObj.W - gCam.Y * gCam.Z * gObj.W) * difyAyBw
                );

        return returnValue;
    }

    private double CalculateParameterCByHandCaluculation(ImagePlanePoint a, ImagePlanePoint b, ObjectVector4 gObj, CameraVector4 gCam)
    {
        var xA = a.X;
        var xB = b.X;
        var xDiff = xA - xB;

        var yA = a.Y;
        var yB = b.Y;
        var yDiff = yA - yB;

        var coefficientObj = Math.Sqrt(gObj.U * gObj.U + gObj.W * gObj.W);
        var coeffieientCam = Math.Sqrt(gCam.X * gCam.X + gCam.Z * gCam.Z);
        var firstCoefficient = coefficientObj * coeffieientCam / xDiff;
        var secondCoefficient = coefficientObj * coeffieientCam / yDiff;

		var difBAu = _largeB.U - _largeA.U;
        var difBAv = _largeB.V - _largeA.V;
		var difBAw = _largeB.W - _largeA.W;
		var difxAxBu = xA * _largeA.U - xB * _largeB.U;
        var difxAxBv = xA * _largeA.V - xB * _largeB.V;
        var difxAxBw = xA * _largeA.W - xB * _largeB.W;
        var difyAyBu = yA * _largeA.U - yB * _largeB.U;
        var difyAyBv = yA * _largeA.V - yB * _largeB.V;
        var difyAyBw = yA * _largeA.W - yB * _largeB.W;


		var c = firstCoefficient *
				(
					gCam.X * (gObj.U * difBAu + gObj.V * difBAv + gObj.W * difBAw) +
					gCam.Z * (gObj.U * difxAxBu + gObj.V * difxAxBv + gObj.W * difxAxBw)
				)
				-
				secondCoefficient *
				(
                    gCam.Y * (gObj.U * difBAu + gObj.V * difBAv + gObj.W * difBAw) +
                    gCam.Z * (gObj.U * difyAyBu + gObj.V * difyAyBv + gObj.W * difyAyBw)
                );
		
        return c;
    }

    public static float Vector4Dot(Vector4 vec1, Vector4 vec2)
    {
        return (vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z + vec2.Z);
    }

    public class ObjectVector4
    {
	    private readonly Vector4 _value;

	    public ObjectVector4(Vector4 value)
	    {
		    _value = value;
	    }

	    public float U => _value.X;
	    public float V => _value.Y;
	    public float W => _value.Z;
    }
    
    public class CameraVector4
    {
	    private readonly Vector4 _value;

	    public CameraVector4(Vector4 value)
	    {
		    _value = value;
	    }

	    public float X => _value.X;
	    public float Y => _value.Y;
	    public float Z => _value.Z;
    }

    public class Marker
    {
	    private readonly Vector3 _value;

	    public Marker(Vector3 value)
	    {
		    _value = value;
	    }

        public Marker(Vector4 value)
        {
            _value = new Vector3(value.X, value.Y, value.Z);
        }

        public float U => _value.X;
	    public float V => _value.Y;
	    public float W => _value.Z;
    }

    public class ImagePlanePoint
    {
	    private readonly Vector2 _value;

	    public ImagePlanePoint(Vector2 value)
	    {
		    _value = value;
	    }

	    public float X => _value.X;
	    public float Y => _value.Y;
    }
}
    