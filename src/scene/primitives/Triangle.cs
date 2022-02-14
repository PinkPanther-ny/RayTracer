using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a triangle in a scene represented by three vertices.
    /// </summary>
    public class Triangle : SceneEntity
    {
        private Vector3 v0, v1, v2;
        private Vector3 normal;
        private Material material;

        /// <summary>
        /// Construct a triangle object given three vertices.
        /// </summary>
        /// <param name="v0">First vertex position</param>
        /// <param name="v1">Second vertex position</param>
        /// <param name="v2">Third vertex position</param>
        /// <param name="material">Material assigned to the triangle</param>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Material material)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.normal = (v1 - v0).Cross(v2 - v0);
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the triangle, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            Vector3 center = new Vector3((v0.X + v1.X + v2.X) / 3, (v0.Y + v1.Y + v2.Y) / 3, (v0.Z + v1.Z + v2.Z) / 3);
            if (normal.Dot(ray.Direction) != 0)
            {
                // Inside-outside test
                double t = (center-ray.Origin).Dot(normal) / ray.Direction.Dot(normal);
                Vector3 p = ray.Origin + t * ray.Direction;
                
                if (normal.Dot((v1 - v0).Cross(p - v0)) >= 0 &&
                    normal.Dot((v2 - v1).Cross(p - v1)) >= 0 &&
                    normal.Dot((v0 - v2).Cross(p - v2)) >= 0 &&
                    t >= 0)
                {
                    return new RayHit(p, normal.Normalized(), ray.Direction, this);
                }
                
            }
            return null;
        }

        /// <summary>
        /// The material of the triangle.
        /// </summary>
        public Material Material { get { return this.material; } }
        
        public bool Inside(Vector3 position)
        {
            Vector3 center = new Vector3((v0.X + v1.X + v2.X) / 3, (v0.Y + v1.Y + v2.Y) / 3, (v0.Z + v1.Z + v2.Z) / 3);
            bool onThePlane = Math.Abs((position - center).Dot(normal)) < 0.000001;
            
            return (normal.Dot((v1 - v0).Cross(position - v0)) >= 0 &&
                    normal.Dot((v2 - v1).Cross(position - v1)) >= 0 &&
                    normal.Dot((v0 - v2).Cross(position - v2)) >= 0 &&
                    onThePlane
                );

        }
        
    }

}
