using System;
using System.IO;
using CommandLine;

namespace RayTracer
{
    /// <summary>
    /// Main program. Modify this file **AT YOUR OWN RISK**. Doing so may break how
    /// our automated testing system checks your solution, since the command line
    /// arguments need to exactly match the specification. Note we have already
    /// parsed these for you here, and they are passed to the Scene class. If you feel
    /// the need to modify this file, you are probably doing something wrong.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Command line arguments configuration
        /// </summary>
        public class OptionsConf
        {
            [Option('f', "file", Required = true, HelpText = "Input file path (txt).")]
            public string InputFilePath { get; set; }

            [Option('o', "output", Required = true, HelpText = "Output file path (PNG).")]
            public string OutputFilePath { get; set; }

            [Option('w', "width", Default = (int)400, HelpText = "Output image width in pixels.")]
            public int OutputImageWidth { get; set; }

            [Option('h', "height", Default = (int)400, HelpText = "Output image height in pixels.")]
            public int OutputImageHeight { get; set; }

            [Option('x', "aa-mult", Default = (int)1, HelpText = "Anti-aliasing sampling multiplier.")]
            public int AAMultiplier { get; set; }

            [Option('l', "ambient", Default = (bool)false, HelpText = "Enable ambient lighting.")]
            public bool AmbientLightingEnabled { get; set; }

            [Option('p', "cam-pos", Default = (string)"0,0,0", HelpText = "Camera position in world coordinates in form: x,y,z")]
            public string CameraPosition { get; set; }

            [Option('a', "cam-axis", Default = (string)"0,0,1", HelpText = "Camera axis in world coordinates in form: x,y,z")]
            public string CameraAxis { get; set; }

            [Option('n', "cam-angle", Default = (double)0, HelpText = "Camera angle in degrees.")]
            public double CameraAngle { get; set; }

            [Option('r', "aperture-radius", Default = (double)0, HelpText = "Aperture radius of the camera.")]
            public double ApertureRadius { get; set; }

            [Option('t', "focal-length", Default = (double)1, HelpText = "Focal length of the camera.")]
            public double FocalLength { get; set; }

            // Customized options
            
            [Option('v', "horizontal-fov", Default = (double)60, HelpText = "Horizontal field of view in degrees.")]
            public double HorizontalFov { get; set; }

            [Option('R', "maxReflectDepth", Default = (int)10, HelpText = "Maximum reflect recursion depth.")]
            public int MaxRelectionDepth { get; set; }

            [Option('T', "renderThreadSquareRoot", Default = (int)1, HelpText = "Render threads square root number. eg,set to 4, the image will be divided and rendered as (4x4) blocks")]
            public int RenderThreadSquareRoot { get; set; }

            [Option('I', "realTimeRendererInterval", Default = (int)0, HelpText = "Real time rendering interval in seconds, set to 0 to disable.")]
            public int RealTimeRendererInterval { get; set; }
        }

        /// <summary>
        /// Helper to parse command line argument as Vector3
        /// </summary>
        /// <param name="cmdStr">String as vector in form #,#,#</param>
        /// <returns></returns>
        static Vector3 CmdStrToVector3(string cmdStr)
        {
            try
            {
                string[] s = cmdStr.Split(',');
                return new Vector3(double.Parse(s[0]), double.Parse(s[1]), double.Parse(s[2]));
            }
            catch (Exception)
            {
                throw new ArgumentException(@"Command line error: Expecting vector in form #,#,# but got " + cmdStr, "cmdLineParam");
            }
        }

        /// <summary>
        /// Main program entry point for the ray tracer.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<OptionsConf>(args)
                .WithParsed<OptionsConf>(options =>
                {
                    try
                    {
                        // Construct a new output image, with size according to command line args
                        Image outputImage = new Image(options.OutputImageWidth, options.OutputImageHeight);

                        // Read/parse the scene specification file
                        SceneReader sceneReader = new SceneReader(options.InputFilePath);

                        // Construct the scene
                        // - Pass options based on command line arguments
                        // - Populate the scene based on the parsed file
                        Scene scene = new Scene(new SceneOptions(
                            options.AAMultiplier,
                            options.AmbientLightingEnabled,
                            CmdStrToVector3(options.CameraPosition),
                            CmdStrToVector3(options.CameraAxis),
                            options.CameraAngle,
                            options.ApertureRadius,
                            options.FocalLength,
                            options.HorizontalFov,
                            options.MaxRelectionDepth,
                            options.RenderThreadSquareRoot,
                            options.RealTimeRendererInterval
                        ));
                        sceneReader.PopulateScene(scene);

                        // Render the scene by executing the core ray tracing logic
                        // (You should be implementing this method inside Scene.cs)
                        scene.Render(outputImage);

                        // Write output to file as PNG
                        outputImage.WritePNG(options.OutputFilePath);
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine("Input file not found.");
                    }
                    catch (SceneReader.ParseException e)
                    {
                        Console.WriteLine($"Input file invalid on line {e.Line}: {e.Message}");
                    }
                    catch (ArgumentException e)
                    {
                        if (e.ParamName == "cmdLineParam")
                        {
                            Console.WriteLine(e.Message);
                        }
                        else
                        {
                            // Still allow other argument exceptions through
                            throw e;
                        }
                    }
                });
        }
    }
}
