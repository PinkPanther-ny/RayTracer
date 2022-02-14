using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent an (infinite) plane in a scene.
    /// </summary>
    public class Plane : SceneEntity
    {
        private Vector3 center;
        private Vector3 normal;
        private Material material;

        /// <summary>
        /// Construct an infinite plane object.
        /// </summary>
        /// <param name="center">Position of the center of the plane</param>
        /// <param name="normal">Direction that the plane faces</param>
        /// <param name="material">Material assigned to the plane</param>
        public Plane(Vector3 center, Vector3 normal, Material material)
        {
            this.center = center;
            this.normal = normal.Normalized();
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the plane, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            if (normal.Dot(ray.Direction) != 0)
            {
                double t = (center-ray.Origin).Dot(normal) / ray.Direction.Dot(normal);
                if (t >= 0)
                {
                    Vector3 intersection = ray.Origin + t * ray.Direction;
                    return new RayHit(intersection, normal.Normalized(), ray.Direction, this);
                }
            }
            return null;
        }

        /// <summary>
        /// The material of the plane.
        /// </summary>
        public Material Material { get { return this.material; } }

        public bool Inside(Vector3 position)
        {
            return Math.Abs((position - this.center).Dot(normal)) < 0.000001;
        }
    }

}
