using System;

namespace RayTracer
{
    /// <summary>
    /// Immutable structure to represent a three-dimensional vector.
    /// </summary>
    public readonly struct Vector3
    {
        private readonly double x, y, z;

        /// <summary>
        /// Construct a three-dimensional vector.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Convert vector to a readable string.
        /// </summary>
        /// <returns>Vector as string in form (x, y, z)</returns>
        public override string ToString()
        {
            return "(" + x + "," + y + "," + z + ")";
        }
        
        /// <summary>
        /// Convert vector to color.
        /// </summary>
        /// <returns>Vector as color in form (x, y, z)</returns>
        public Color ToColor()
        {
            return new Color(x, y, z);
        }

        /// <summary>
        /// Compute the length of the vector squared.
        /// This should be used if there is a way to perform a vector
        /// computation without needing the actual length, since
        /// a square root operation is expensive.
        /// </summary>
        /// <returns>Length of the vector squared</returns>
        public double LengthSq()
        {
            return x * x + y * y + z * z;
        }

        /// <summary>
        /// Compute the length of the vector.
        /// </summary>
        /// <returns>Length of the vector</returns>
        public double Length()
        {
            return Math.Sqrt(LengthSq());
        }

        /// <summary>
        /// Compute a length 1 vector in the same direction.
        /// </summary>
        /// <returns>Normalized vector</returns>
        public Vector3 Normalized()
        {
            double magnitude = Length();
            return new Vector3(x/magnitude, y/magnitude, z/magnitude);
        }

        /// <summary>
        /// Compute the dot product with another vector.
        /// </summary>
        /// <param name="with">Vector to dot product with</param>
        /// <returns>Dot product result</returns>
        public double Dot(Vector3 with)
        {
            return x * with.x + y * with.y + z * with.z;
        }

        /// <summary>
        /// Compute the cross product with another vector.
        /// </summary>
        /// <param name="with">Vector to cross product with</param>
        /// <returns>Cross product result</returns>
        public Vector3 Cross(Vector3 with)
        {
            return new Vector3(y*with.z-z*with.y, z*with.x-x*with.z, x*with.y-y*with.x);
        }

        /// <summary>
        /// Sum two vectors together (using + operator).
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Summed vector</returns>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x+b.x, a.y+b.y, a.z+b.z);
        }

        /// <summary>
        /// Negate a vector (using - operator)
        /// </summary>
        /// <param name="a">Vector to negate</param>
        /// <returns>Negated vector</returns>
        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        /// <summary>
        /// Subtract one vector from another.
        /// </summary>
        /// <param name="a">Original vector</param>
        /// <param name="b">Vector to subtract</param>
        /// <returns>Subtracted vector</returns>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x-b.x, a.y-b.y, a.z-b.z);
        }

        /// <summary>
        /// Multiply a vector by a scalar value.
        /// </summary>
        /// <param name="a">Original vector</param>
        /// <param name="b">Scalar multiplier</param>
        /// <returns>Multiplied vector</returns>
        public static Vector3 operator *(Vector3 a, double b)
        {
            return new Vector3(b * a.x, b * a.y, b * a.z);
        }

        /// <summary>
        /// Multiply a vector by a scalar value (opposite operands).
        /// </summary>
        /// <param name="b">Scalar multiplier</param>
        /// <param name="a">Original vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector3 operator *(double b, Vector3 a)
        {
            return new Vector3(b * a.x, b * a.y, b * a.z);
        }

        /// <summary>
        /// Divide a vector by a scalar value.
        /// </summary>
        /// <param name="a">Original vector</param>
        /// <param name="b">Scalar divisor</param>
        /// <returns>Divided vector</returns>
        public static Vector3 operator /(Vector3 a, double b)
        {
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        /// <summary>
        /// Divide a vector by a scalar value.
        /// </summary>
        /// <param name="a">Original vector</param>
        /// <param name="b">Scalar divisor</param>
        /// <returns>Divided vector</returns>
        public double DistanceTo(Vector3 a)
        {
            return Math.Sqrt((a.x - x) * (a.x - x) + (a.y - y) * (a.y - y) + (a.z - z) * (a.z - z));
        }
        
        /// <summary>
        /// X component of the vector.
        /// </summary>
        public double X { get { return x; } }

        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public double Y { get { return y; } }

        /// <summary>
        /// Z component of the vector.
        /// </summary>
        public double Z { get { return z; } }
    }
}
