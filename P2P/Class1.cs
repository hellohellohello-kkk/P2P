using System.Numerics;

namespace P2P
{
    public class Class1
    {
        private readonly RotationMatrixCalculator _rotationMatrixCalculator;


        public Class1()
        {
            _rotationMatrixCalculator = new RotationMatrixCalculator();
        }

        public static (double, double) CalculateProjectionalCoordinatesFromCameraCoordinates(double[,] rotaiton, double tx, double ty, double tz, double[] A)
        {
            double denominator = rotaiton[2,0] * A[0] + rotaiton[2, 1] * A[1] + rotaiton[2,2] * A[2] + tz;
            double tildeXA = (rotaiton[0,0] * A[0] + rotaiton[0,1] * A[1] + rotaiton[0,2] * A[2] + tx) / denominator;
            double tildeYA = (rotaiton[1,0] * A[0] + rotaiton[1,1] * A[1] + rotaiton[1,2] * A[2] + ty) / denominator;

            return (tildeXA, tildeYA);
        }

        public static double[] CalculateTranslationalVector(double Az, double tildeXA, double tildeYA, double[,] R, double[] A)
        {
            double tx = Az * tildeXA - (R[0,0] * A[0] + R[0,1] * A[1] + R[0,2] * A[2]);
            double ty = Az * tildeYA - (R[1,0] * A[0] + R[1,1] * A[1] + R[1,2] * A[2]);
            double tz = Az - (R[2,0] * A[0] + R[2,1] * A[1] + R[2,2] * A[2]);

            var translationalVector = new double[] { tx, ty, tz };
            return translationalVector;
        }

        public static double CalculateAz(double[] R1, double[] R3, double[,] R, double xATilde,double xBTilde, double[] A, double[] B)
        {
            double numerator = (R[0,0] - xBTilde * R[2,0]) * (A[0] - B[0]) + (R[0,1] - xBTilde * R[2,1]) * (A[1] - B[1]) + (R[0,2] - xBTilde * R[2,2]) * (A[2] - B[2]);
            double denominator = xBTilde - xATilde;

            double Az = numerator / denominator;

            return Az;
        }

        public static double CalculateRotationAngle(double c, double M, double beta)
        {
            double alpha = Math.Acos(-c / M) + beta;

            return alpha;
        }




        public void calculatABC()
        {
            double x_A, x_B, y_A, y_B, g_u, g_v, g_w, g_x, g_y, g_z, A_u, A_v, B_u, B_v;
            double g_u2, g_v2, g_w2, g_x2, g_z2, x_diff, y_diff;
            double a, b, c;

            x_A = 0.0; // 初期値を設定してください
            x_B = 0.0; // 初期値を設定してください
            y_A = 0.0; // 初期値を設定してください
            y_B = 0.0; // 初期値を設定してください
            g_u = 0.0; // 初期値を設定してください
            g_v = 0.0; // 初期値を設定してください
            g_w = 0.0; // 初期値を設定してください
            g_x = 0.0; // 初期値を設定してください
            g_y = 0.0; // 初期値を設定してください
            g_z = 0.0; // 初期値を設定してください
            A_u = 0.0; // 初期値を設定してください
            A_v = 0.0; // 初期値を設定してください
            B_u = 0.0; // 初期値を設定してください
            B_v = 0.0; // 初期値を設定してください

            g_u2 = Math.Pow(g_u, 2);
            g_v2 = Math.Pow(g_v, 2);
            g_w2 = Math.Pow(g_w, 2);
            g_x2 = Math.Pow(g_x, 2);
            g_z2 = Math.Pow(g_z, 2);
            x_diff = x_A - x_B;
            y_diff = y_A - y_B;

            a = 1 / x_diff * ((g_u2 + g_v2) * g_y * (-A_v * g_x + B_v * g_x + A_v * x_A * g_z - B_v * x_B * g_z) +
                              A_u * (g_w * (g_x * x_A + g_z) + g_u * g_v * g_y * (g_x - x_A * g_z)) -
                              B_u * (g_w * (g_x * x_B + g_z) + g_u * g_v * g_y * (g_x - x_B * g_z))) - 1 / y_diff *
                (g_w * (g_x * (A_u * y_A - B_u * y_B) - B_v * g_w * (g_x2 + g_y * y_B * g_z + g_z2) +
                        A_v * g_w * (g_x2 + g_z * (g_y * y_A + g_z))) +
                 g_u * g_v * (-A_u * (g_x2 + g_z * (g_y * y_A + g_z)) + B_v * (g_x2 + g_z * (g_y * y_B + g_z))) +
                 g_u2 * (A_v * (g_x2 + g_z * (g_y * y_A + g_z)) - B_v * (g_x2 + g_z * (g_y * y_B + g_z))));
            b = 1 / x_diff * (g_u * g_v * (-A_u * (g_x * x_A + g_z) + B_u * (g_x * B_v + g_z)) +
                              g_u2 * (A_v * (g_x * x_A + g_z) - B_v * (g_x * x_B + g_z) +
                                      g_w * g_y * (A_u * g_x - B_u * g_x - A_u * x_A * g_z + B_u * x_B * g_z)) +
                              g_w * (A_v * g_w * (g_x * x_A + g_z) - B_v * g_w * (g_x * x_B + g_z) +
                                     (Math.Pow(g_v, 2) + Math.Pow(g_w, 2)) * g_y *
                                     (-B_u * g_x + B_u * x_B * g_z + A_u * (g_x - x_A * g_z)))) - 1 / (y_A - y_B) *
                (g_u * g_v * g_x * (-A_u * y_A + B_u * y_B) + Math.Pow(g_u, 2) * (g_x * (A_v * y_A - B_v * y_B) +
                    B_u * g_w * (Math.Pow(g_x, 2) + Math.Pow(g_y, 2) * y_B * g_z + Math.Pow(g_z, 2)) -
                    A_u * (Math.Pow(g_v, 2) + Math.Pow(g_w, 2)) *
                    (Math.Pow(g_x, 2) + Math.Pow(g_y, 2) * y_A * g_z + Math.Pow(g_z, 2)) + B_u *
                    (Math.Pow(g_v, 2) + Math.Pow(g_w, 2)) *
                    (Math.Pow(g_x, 2) + Math.Pow(g_y, 2) * y_B * g_z + Math.Pow(g_z, 2))));
            c = Math.Sqrt((Math.Pow(g_u, 2) + Math.Pow(g_w, 2)) * (Math.Pow(g_x, 2) + Math.Pow(g_z, 2))) / (x_A - x_B) *
                (g_u * (-B_u * g_x + B_u * x_B * g_z + A_u * (g_x - x_A * g_z)) +
                 g_v * (-B_v * g_x + B_v * x_B * g_z + A_v * (g_x - x_A * g_z))) -
                Math.Sqrt((Math.Pow(g_u, 2) + Math.Pow(g_w, 2)) * (Math.Pow(g_x, 2) + Math.Pow(g_z, 2))) / (y_A - y_B) *
                (g_u * (-B_u * g_y + B_u * y_B * g_z + A_u * (g_y - y_A * g_z)) +
                 g_v * (-B_v * g_y + B_v * y_B * g_z + A_v * (g_y - y_A * g_z)));

        }

        public double[,] rotationMatrixFromCameraToCertainRef(double g_x, double g_y, double g_z)
        {
            
            double denominator = Math.Sqrt(Math.Pow(g_x, 2) + Math.Pow(g_z, 2));

            double[] g_cam = { g_x, g_y, g_z};
            double[] m_cam = { g_z / denominator, 0, -g_x / denominator };
            double[] n_cam = { -g_x * g_y / denominator, denominator, -g_y * g_z / denominator };

            double[,] rotationMatrix = new double[3, 3]
            {
                { g_cam[0], m_cam[0], n_cam[0] },
                { g_cam[1], m_cam[1], n_cam[1] },
                { g_cam[2], m_cam[2], n_cam[2] }
            };

            return rotationMatrix;
        }
    }
}


