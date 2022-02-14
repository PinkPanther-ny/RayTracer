using System;
using System.Collections.Generic;
using System.Threading;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a ray traced scene, including the objects,
    /// light sources, and associated rendering logic.
    /// </summary>
    public class Scene
    {
        private SceneOptions options;
        private ISet<SceneEntity> entities;
        private ISet<PointLight> lights;
        private static readonly object Locker = new object();
        private static int _progress;

        /// <summary>
        /// Construct a new scene with provided options.
        /// </summary>
        /// <param name="options">Options data</param>
        public Scene(SceneOptions options = new SceneOptions())
        {
            this.options = options;
            entities = new HashSet<SceneEntity>();
            lights = new HashSet<PointLight>();
            _progress = 1;
        }

        /// <summary>
        /// Add an entity to the scene that should be rendered.
        /// </summary>
        /// <param name="entity">Entity object</param>
        public void AddEntity(SceneEntity entity)
        {
            entities.Add(entity);
        }

        /// <summary>
        /// Add a point light to the scene that should be computed.
        /// </summary>
        /// <param name="light">Light structure</param>
        public void AddPointLight(PointLight light)
        {
            lights.Add(light);
        }

        public double Degree2Radian(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public double Radian2Degree(double radian)
        {
            return (180 / Math.PI) * radian;
        }

        public Vector3 GetVectorFromAngle(double horizontalAngle, double verticalAngle)
        {
            return new Vector3(
                -Math.Sin(horizontalAngle) * Math.Cos(verticalAngle),
                Math.Sin(verticalAngle),
                Math.Cos(horizontalAngle) * Math.Cos(verticalAngle)
            );
        }

        public RayHit GetHitEntity(Ray ray)
        {
            RayHit rayHit = null;
            double minDist = 99999;
            foreach (var entity in entities)
            {
                var tempHit = entity.Intersect(ray);
                if (tempHit != null)
                {
                    var tempDest = ray.Origin.DistanceTo(tempHit.Position);
                    if (tempDest > 0.00001 && tempDest < minDist)
                    {
                        minDist = tempDest;
                        rayHit = tempHit;
                    }
                }
            }

            return rayHit;
        }

        public Color GetDiffuseColor(RayHit rayHit)
        {
            SceneEntity hitEntity = rayHit.HitEntity;
            Color color = new Color(0, 0, 0);
            foreach (var light in this.lights)
            {
                if (!CheckRayBlocked(rayHit.Position, light.Position, rayHit.Normal))
                {
                    var diffuseFactor = rayHit.Normal.Normalized().Dot((light.Position - rayHit.Position).Normalized());
                    color += (diffuseFactor > 0 ? diffuseFactor : 0) * (hitEntity.Material.Color * light.Color);

                }
            }

            return color;
        }

        public double GetMediumRefractionIndex(Vector3 position)
        {
            foreach (var entity in entities)
            {
                if (entity.Inside(position))
                {
                    return entity.Material.RefractiveIndex;
                }
            }

            return 1;
        }

        public Color GetRayPointAtColor(Ray ray, int depth)
        {
            if (depth == options.MaxReflectionDepth)
            {
                return new Color(0, 0, 0);
            }

            RayHit rayHit = GetHitEntity(ray);
            Ray outRay = default;
            if (rayHit != null)
            {
                SceneEntity hitEntity = rayHit.HitEntity;

                switch (hitEntity.Material.Type)
                {
                    case Material.MaterialType.Diffuse:
                        return GetDiffuseColor(rayHit);

                    case Material.MaterialType.Reflective:
                        outRay = rayHit.GetReflectedRay();
                        break;
                    case Material.MaterialType.Refractive:
                        double Min(double d, double d1) => d1 < d ? d1 : d;
                        double Max(double d, double d1) => d1 > d ? d1 : d;

                        // Adopted from https://www.cnblogs.com/jietian331/p/5564901.html
                        // double bias = 0.08, scale = 1.3, power = 2;
                        double bias = 0, scale = 1, power = 2;
                        double fresnelCoef = Max(0,
                            Min(1, bias + scale * Math.Pow(1 + rayHit.Incident.Dot(rayHit.Normal), power)));
                        outRay = rayHit.GetRefractedRay(GetMediumRefractionIndex(ray.Origin) /
                                                        GetMediumRefractionIndex(rayHit.Position));
                        var reflRay = rayHit.GetReflectedRay();

                        var finalColor = fresnelCoef * GetRayPointAtColor(reflRay, depth + 1) +
                                         (1 - fresnelCoef) * GetRayPointAtColor(outRay, depth + 1);
                        return finalColor;
                }

                return GetRayPointAtColor(outRay, depth + 1);
            }

            return new Color(0, 0, 0);
        }

        public bool CheckRayBlocked(Vector3 from, Vector3 dest, Vector3 normal)
        {
            Ray ray = new Ray(from + 0.0000001 * (normal), dest - from);
            foreach (var entity in entities)
            {
                RayHit rayHit = entity.Intersect(ray);
                if (rayHit != null)
                {

                    if (ray.Origin.DistanceTo(dest) > ray.Origin.DistanceTo(rayHit.Position))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Render the scene to an output image. This is where the bulk
        /// of your ray tracing logic should go... though you may wish to
        /// break it down into multiple functions as it gets more complex!
        /// </summary>
        /// <param name="outputImage">Image to store render output</param>
        public void Render(Image outputImage)
        {
            TimeSpan t0= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));

            int sqrtThreadNum = options.RenderThreadSquareRoot;
            if (sqrtThreadNum == 1)
            {
                // Real-time rendering
                var realTimeRenderer = new Thread(() => RealTimeRender(outputImage));
                realTimeRenderer.Start();
                PartialRender(outputImage, 0, 0, outputImage.Width, outputImage.Height);
                
            }
            else
            {

                // Real-time rendering
                var realTimeRenderer = new Thread(() => RealTimeRender(outputImage));
                realTimeRenderer.Start();
                
                // Divide the image to multiple blocks, multi-thread rendering
                Thread[] pool = new Thread[sqrtThreadNum * sqrtThreadNum];

                int countThread = 0;
                int xStep = outputImage.Width / sqrtThreadNum, yStep = outputImage.Height / sqrtThreadNum;
                for (int i = 0; i < sqrtThreadNum; i++)
                {
                    for (int j = 0; j < sqrtThreadNum; j++, countThread++)
                    {
                        int i1 = i, j1 = j;
                        pool[countThread] = new Thread(() => PartialRender(outputImage,
                            j1 * xStep, i1 * yStep,
                            j1 + 1 == sqrtThreadNum ? outputImage.Width : (j1 + 1) * xStep,
                            i1 + 1 == sqrtThreadNum ? outputImage.Height : (i1 + 1) * yStep)
                        );
                        pool[countThread].Start();
                    }
                }
                foreach (var t in pool)
                {
                    t.Join();
                }
            }
            
            TimeSpan t1= DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc));
            Console.Out.WriteLine($"Image rendered in {Math.Round(t1.TotalSeconds - t0.TotalSeconds, 3)} seconds.");
        }

        // Multi threads method
        public void PartialRender(Image outputImage, int startX, int startY, int endX, int endY)
        {
            // Raytracing
            var pixelWid = Math.Tan(Degree2Radian(options.HorizontalFov / 2)) / (outputImage.Width / 2.0);
            var northWestConor = new Vector3(
                -Math.Tan(Degree2Radian(options.HorizontalFov / 2)),
                pixelWid * outputImage.Width / 2, 1);

            int aaSqr = options.AAMultiplier * options.AAMultiplier;
            Random rand = new Random();
            
            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    // Progress recorder for real time display
                    lock (Locker)
                    {
                        _progress += 1;
                    }
                    // Main ray tracing logic part
                    
                    // Random AA sampling
                    Color[] cs = new Color[aaSqr];
                    for (int i = 0; i < aaSqr; i++)
                    {
                        var nx = aaSqr != 1 ? rand.NextDouble() - 0.5 : 0;
                        var ny = aaSqr != 1 ? rand.NextDouble() - 0.5 : 0;
                        Ray ray = new Ray(
                            new Vector3(0, 0, 0),
                            new Vector3(
                                northWestConor.X + (0.5 + nx + x) * pixelWid,
                                northWestConor.Y - (0.5 + ny + y) * pixelWid,
                                1
                            )
                        );

                        // Find closest intersect object
                        RayHit rayHit = GetHitEntity(ray);
                        if (rayHit != null)
                        {
                            // Recursively get ray hit color
                            cs[i] = GetRayPointAtColor(ray, 0);
                        }
                    }

                    // Mix AA samples' color.
                    double r = 0, g = 0, b = 0;
                    foreach (var c in cs)
                    {
                        r += c.R;
                        g += c.G;
                        b += c.B;
                    }
                    outputImage.SetPixel(x, y, new Color(r / aaSqr, g / aaSqr, b / aaSqr));
                    
                }
            }
        }

        // Display realtime progress and image per time interval.
        private void RealTimeRender(Image outputImage)
        {
            int renderInterval = options.RealTimeRendererInterval;
            if (renderInterval <= 0)
            {
                return;
            }

            double GetCurrentTime() => DateTime.Now.Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                ).TotalSeconds;
            
            int totalPixel = outputImage.Height * outputImage.Width;
            double startTime = GetCurrentTime();
            string startTimeStr = DateTime.Now.ToString("h:mm:ss tt");
            double t0 = GetCurrentTime();
            
            var threads = options.RenderThreadSquareRoot;
            var allInfo = "RAY TRACER REAL-TIME RENDERING\n\n" +
                          $"Ray tracer started at {startTimeStr}\n" +
                          "Image render options:\n" +
                          "Resolution:           " +
                          $"{outputImage.Height}x{outputImage.Width}\n" +
                          "Anti-Aliasing:        " +
                          $"{(options.AAMultiplier != 1 ? options.AAMultiplier + "xAA" : "OFF")}\n" +
                          "Field of view:        " +
                          $"{options.HorizontalFov} degrees\n" +
                          "Reflection max depth: " +
                          $"{options.MaxReflectionDepth} times\n" +
                          "Multi-Threading:      " +
                          $"{(threads != 1 ? "(" + threads + 'x' + threads + ")" + " threads rendering" : "OFF")}\n";
            
            // Wait for the first round.
            Thread.Sleep(renderInterval * 1000);
            
            while (_progress <= totalPixel)
            {
                if (GetCurrentTime() - t0 > renderInterval)
                {
                    t0 = GetCurrentTime();
                    Console.WriteLine(allInfo);
                    ShowProgressAndRender(outputImage);
                    // Let other working threads occupy cpu instead of wasting on the while loop.
                    Console.WriteLine($"Already rendered " +
                                      $"{Math.Round(GetCurrentTime() - startTime, 2)} seconds.\n");
                    Thread.Sleep(renderInterval * 1000);
                    Console.Clear();
                }
            }
            Console.WriteLine(allInfo);
            ShowProgressAndRender(outputImage);
        }

        private void ShowProgressAndRender(Image outputImage)
        {
            var total = outputImage.Height * outputImage.Width;
            try
            {
                outputImage.WritePNG("realTime.png");
                Console.WriteLine("The current time is {0:h:mm:ss tt}", DateTime.Now);
                Console.WriteLine($"Rendering {_progress - 1}/{total}... " +
                                  $"({Math.Round((_progress / (double)total) * 100, 1)}%)");
            }
            catch (System.IO.IOException e)
            {
                Console.Out.WriteLine("IOException caught, no worries, close realTime.png " +
                                      "which is being used by another process to see further real time update.\n");
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e);
            }
        }
    }
}
