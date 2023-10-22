namespace P2P;

public class Angle
{
	private Angle(double degree)
	{
		Degree = degree;
	}

	public static Angle CreateFromDegree(double degree)
	{
		return new Angle(degree);
	}

	public double Degree { get; }

	public double Radian => Degree / 180 * Math.PI;
}