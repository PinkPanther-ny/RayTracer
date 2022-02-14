
namespace RayTracer
{
    /// <summary>
    /// Class to represent a material that is associated with an entity
    /// that is to be rendered by the ray tracer. Materials contain
    /// properties that describe how light should interact with an object.
    /// </summary>
    public class Material
    {
        /// <summary>
        /// The type of a material specifies what sort of ray tracing
        /// behaviour should be expected.
        /// </summary>
        public enum MaterialType
        {
            Diffuse,
            Reflective,
            Refractive,
            Emissive,
            Glossy
        }

        private MaterialType type;
        private Color color;
        private double refractiveIndex;

        /// <summary>
        /// Construct a new material object.
        /// </summary>
        /// <param name="type">The type of the material</param>
        /// <param name="color">The color of the material</param>
        /// <param name="refractiveIndex">The refractive index of the material (if applicable)</param>
        public Material(MaterialType type, Color color, double refractiveIndex = 1)
        {
            this.type = type;
            this.color = color;
            this.refractiveIndex = refractiveIndex;
        }

        /// <summary>
        /// The type of the material.
        /// </summary>
        public MaterialType Type { get { return this.type; } }

        /// <summary>
        /// The base color of the material.
        /// </summary>
        public Color Color { get { return this.color; } }

        /// <summary>
        /// The refractive index of the material.
        /// </summary>
        public double RefractiveIndex { get { return this.refractiveIndex; } }
    }
}
