using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent an (infinite) plane in a scene.
    /// </summary>
    public class Sphere : SceneEntity
    {
        public Vector3 center;
        private double radius;
        private Material material;

        /// <summary>
        /// Construct a sphere given its center point and a radius.
        /// </summary>
        /// <param name="center">Center of the sphere</param>
        /// <param name="radius">Radius of the spher</param>
        /// <param name="material">Material assigned to the sphere</param>
        public Sphere(Vector3 center, double radius, Material material)
        {
            this.center = center;
            this.radius = radius;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the sphere, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            // Maths from :
            // https://www.cs.princeton.edu/courses/archive/fall00/cs426/lectures/raycast/sld013.htm
            var L = center - ray.Origin;
            var t_ca = L.Dot(ray.Direction);
            // Check if the sphere is behind the ray
            if (t_ca <= 0)
            {
                return null;
            }
            
            var dSquare = L.LengthSq() - t_ca * t_ca;
            // Check if the ray hits the sphere
            if (dSquare > radius * radius)
            {
                return null;
            }

            var t_hc = Math.Sqrt(radius * radius - dSquare);
            var t1 = t_ca - t_hc;
            var t2 = t_ca + t_hc;

            
            if (Math.Abs(t_ca - t_hc) < 0.00001)// could be 5 zeros
            {
                // Greater t indicate it's on the other side of the sphere
                var t = t1 > t2 ? t1 : t2;
                var pInside = ray.Origin + t * ray.Direction;
                var normalInside = -((pInside - center) / (pInside - center).Length()).Normalized();
                
                return new RayHit(pInside - 0.000001*normalInside, normalInside, ray.Direction, this);
                
            }
            else
            {
                // Closest point will be draw when hit from outside
                var t = t1 < t2 ? t1 : t2;
                var p = ray.Origin + t * ray.Direction;
                var normal = ((p - center) / (p - center).Length()).Normalized();

                return new RayHit(p - 0.000001*normal, normal, ray.Direction, this);
            }
        }

        /// <summary>
        /// The material of the sphere.
        /// </summary>
        public Material Material { get { return this.material; } }
        public double Radius { get { return this.radius; } }
        public Vector3 Center { get { return this.center; } }
        
        public bool Inside(Vector3 position)
        {
            return position.DistanceTo(center) < radius;
        }
        
    }

}
