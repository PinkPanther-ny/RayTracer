using System;

namespace RayTracer
{
    /// <summary>
    /// Immutable structure to represent a ray (origin, direction).
    /// </summary>
    public readonly struct Ray
    {
        private readonly Vector3 origin;
        private readonly Vector3 direction;
        private readonly double mediumRefractionIndex;

        /// <summary>
        /// Construct a new ray.
        /// </summary>
        /// <param name="origin">The starting position of the ray</param>
        /// <param name="direction">The direction of the ray</param>
        /// <param name="mediumRefractionIndex">The medium in which the rays are placed.</param>
        public Ray(Vector3 origin, Vector3 direction, double mediumRefractionIndex = 1)
        {
            this.origin = origin;
            this.direction = direction.Normalized();
            this.mediumRefractionIndex = mediumRefractionIndex;
        }

        /// <summary>
        /// The starting position of the ray.
        /// </summary>
        public Vector3 Origin { get { return this.origin; } }

        /// <summary>
        /// The direction of the ray.
        /// </summary>
        public Vector3 Direction { get { return this.direction; } }
    }
}
