using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent ray hit data, including the position and
    /// normal of a hit (and optionally other computed vectors).
    /// </summary>
    public class RayHit
    {
        private Vector3 position;
        private Vector3 normal;
        private Vector3 incident;

        private SceneEntity hitEntity;
        public RayHit(Vector3 position, Vector3 normal, Vector3 incident, SceneEntity hitEntity)
        {
            this.position = position;
            this.normal = normal;
            this.incident = incident;
            this.hitEntity = hitEntity;
        }

        // You may wish to write methods to compute other vectors, 
        // e.g. reflection, transmission, etc

        public Vector3 Position { get { return this.position; } }

        public Vector3 Normal { get { return this.normal; } }

        public Vector3 Incident { get { return this.incident; } }
        public SceneEntity HitEntity { get { return this.hitEntity; } }

        public Ray GetReflectedRay()
        {
            // Adopted from https://www.youtube.com/watch?v=k3rrw2iOsCM
            var cosTheta = (-incident).Dot(normal);
            var d = (incident + 2 * cosTheta * normal).Normalized();
            return new Ray(position - 0.000001 * d, d);
        }
        
        public Ray GetRefractedRay(double eta)
        {
            // eta = 1 / eta;
            // Adopted from NVIDIA
            double cosi = (-incident).Dot(normal);
            double cost2 = 1 - eta * eta * (1 - cosi * cosi);
            Vector3 t = (eta * incident + (eta * cosi - Math.Sqrt(Math.Abs(cost2))) * normal).Normalized();
            // Full reflection
            if (cost2 <= 0)
            {
                return this.GetReflectedRay();
            }
            return new Ray(position - 0.000001 * t, cost2 > 0 ? t : new Vector3(0, 0, 0));

        }
        
    }
}
