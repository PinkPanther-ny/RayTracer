using System;

namespace RayTracer
{
    /// <summary>
    /// Immutable structure representing various scene options.
    /// You may not utilise all of these variables - this will depend
    /// on which add-ons you choose to implement.
    /// </summary>
    public readonly struct SceneOptions
    {
        private readonly int aaMultiplier;
        private readonly bool ambientLightingEnabled;
        private readonly Vector3 cameraPosition, cameraAxis;
        private readonly double cameraAngle;
        private readonly double apertureRadius, focalLength;
        
        private readonly double horizontalFov;
        private readonly int maxReflectDepth;
        private readonly int renderThreadSquareRoot;
        private readonly int realTimeRendererInterval;
        
        /// <summary>
        /// Construct scene options object.
        /// </summary>
        /// <param name="aaMultiplier">Anti-aliasing multiplier</param>
        /// <param name="ambientLightingEnabled">Flag to enable ambient lighting</param>
        /// <param name="cameraPosition">Camera position</param>
        /// <param name="cameraAxis">Camera rotation axis</param>
        /// <param name="cameraAngle">Camera rotation angle</param>
        /// <param name="apertureRadius">Physical camera aperture radius</param>
        /// <param name="focalLength">Physical camera focal length</param>
        /// <param name="horizontalFov">Horizontal field of view in degrees</param>
        /// <param name="maxReflectDepth">Maximum reflect recursion depth.</param>
        /// <param name="renderThreadSquareRoot">Render threads square root number.</param>
        /// <param name="realTimeRendererInterval">Real time rendering interval in seconds, set to 0 to disable</param>
        public SceneOptions(
            int aaMultiplier,
            bool ambientLightingEnabled,
            Vector3 cameraPosition,
            Vector3 cameraAxis,
            double cameraAngle,
            double apertureRadius,
            double focalLength,
            double horizontalFov,
            int maxReflectDepth,
            int renderThreadSquareRoot,
            int realTimeRendererInterval)
        {
            this.aaMultiplier = aaMultiplier;
            this.ambientLightingEnabled = ambientLightingEnabled;
            this.cameraPosition = cameraPosition;
            this.cameraAxis = cameraAxis;
            this.cameraAngle = cameraAngle;
            this.apertureRadius = apertureRadius;
            this.focalLength = focalLength;
            
            this.horizontalFov = horizontalFov;
            this.maxReflectDepth = maxReflectDepth;
            this.renderThreadSquareRoot = renderThreadSquareRoot;
            this.realTimeRendererInterval = realTimeRendererInterval;
        }

        /// <summary>
        /// Anti-aliasing multiplier. Specifies how many samples per pixel in 
        /// each axis. e.g. 2 => 4 samples, 3 => 9 samples, etc.
        /// </summary>
        public int AAMultiplier { get { return this.aaMultiplier; } }

        /// <summary>
        /// Whether ambient lighting computation should be enabled in the scene.
        /// </summary>
        public bool AmbientLightingEnabled { get { return this.ambientLightingEnabled; } }

        /// <summary>
        /// Camera position in the scene.
        /// </summary>
        public Vector3 CameraPosition { get { return this.cameraPosition; } }

        /// <summary>
        /// Camera rotation axis to specify orientation.
        /// </summary>
        public Vector3 CameraAxis { get { return this.cameraAxis; } }

        /// <summary>
        /// Camera rotation angle (lefthand-clockwise around rotation axis).
        /// </summary>
        public double CameraAngle { get { return this.cameraAngle; } }

        /// <summary>
        /// Aperture radius for simulating physical camera depth of field effects.
        /// </summary>
        public double ApertureRadius { get { return this.apertureRadius; } }

        /// <summary>
        /// Focal length for simulating physical camera depth of field effects.
        /// </summary>
        public double FocalLength { get { return this.focalLength; } }

        /// <summary>
        /// Horizontal field of view in degrees.
        /// </summary>
        public double HorizontalFov { get { return this.horizontalFov; } }

        /// <summary>
        /// Maximum reflect recursion depth.
        /// </summary>
        public int MaxReflectionDepth { get { return this.maxReflectDepth; } }

        /// <summary>
        /// Render threads square root number.
        /// </summary>
        public int RenderThreadSquareRoot { get { return this.renderThreadSquareRoot; } }

        /// <summary>
        /// Real time rendering interval in seconds, set to 0 to disable.
        /// </summary>
        public int RealTimeRendererInterval { get { return this.realTimeRendererInterval; } }
    }
}
