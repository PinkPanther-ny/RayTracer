using System;

namespace RayTracer
{
    /// <summary>
    /// Immutable structure to represent a point light source (position, color).
    /// </summary>
    public readonly struct PointLight
    {
        private readonly Vector3 position;
        private readonly Color color;

        /// <summary>
        /// Construct a new point light.
        /// </summary>
        /// <param name="position">Position of the light</param>
        /// <param name="color">Color of the light</param>
        public PointLight(Vector3 position, Color color)
        {
            this.position = position;
            this.color = color;
        }

        /// <summary>
        /// The position of the light
        /// </summary>
        public Vector3 Position { get { return this.position; } }

        /// <summary>
        /// The color of the light
        /// </summary>
        public Color Color { get { return this.color; } }
    }

}
