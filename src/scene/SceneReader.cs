using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace RayTracer
{
    /// <summary>
    /// Class to read/parse a scene file. You don't need to understand
    /// how it works in detail - just know that it loads the scene 
    /// data for you so you don't need to worry about that aspect of 
    /// the project. Modify this file **AT YOUR OWN RISK** (i.e. don't!)
    /// </summary>
    public class SceneReader
    {
        /// <summary>
        /// Custom exception to handle poorly formed scene files.
        /// </summary>
        public class ParseException : Exception
        {
            private readonly int line;

            public ParseException() { }

            public ParseException(string message, int line)
                : base(message)
            {
                this.line = line;
            }

            public int Line { get { return line; } }
        }

        /// <summary>
        /// Representation of a single line in a scene input file.
        /// </summary>
        public class Line
        {
            private int lineNumber;
            private string command;
            private Queue<String> tokens;

            /// <summary>
            /// Construct line by parsing given line data string.
            /// </summary>
            /// <param name="line">Line data</param>
            /// <param name="lineNumber">Line number (for error logging)</param>
            public Line(string line, int lineNumber)
            {
                this.lineNumber = lineNumber;
                this.tokens = new Queue<String>(Regex.Split(line, @"(\s|,|""|\(|\))").Where(s => !string.IsNullOrWhiteSpace(s)));
                this.command = null;
                if (this.tokens.Count > 0)
                {
                    Next(out this.command);
                }
            }

            /// <summary>
            /// The first token of a line (command for that line).
            /// </summary>
            /// <returns></returns>
            public String Command()
            {
                return this.command;
            }

            /// <summary>
            /// Consume line input and interpret next sequence as string.
            /// </summary>
            /// <returns>Interpreted string (if valid)</returns>
            public String ReadString()
            {
                string value;
                Next("\"");
                Next(out value);
                Next("\"");
                return value;
            }

            /// <summary>
            /// Consume line input and interpret next sequence as a double precision float.
            /// </summary>
            /// <returns>Interpreted double (if valid)</returns>
            public double ReadDouble()
            {
                double value;
                Next(out value);
                return value;
            }

            /// <summary>
            /// Consume line input and interpret next sequence as specified enum (typed).
            /// Uses templating/reflection to automatically parse enum values.
            /// </summary>
            /// <returns>Interpreted enum (if valid)</returns>
            public T ReadEnum<T>() where T : Enum
            {
                string raw = Next();
                try
                {
                    return (T)Enum.Parse(typeof(T), raw);
                }
                catch (ArgumentException)
                {
                    throw new ParseException($"Expected specific enum value but got '{raw}' instead.", this.lineNumber);
                }
            }

            /// <summary>
            /// Consume line input and interpret next sequence as vector 3.
            /// </summary>
            /// <returns>Interpreted Vector3 structure (if valid)</returns>
            public Vector3 ReadVector3()
            {
                double x, y, z;
                Next("(");
                Next(out x);
                Next(",");
                Next(out y);
                Next(",");
                Next(out z);
                Next(")");
                return new Vector3(x, y, z);
            }
            
            public Vector3 ReadFaceVector3()
            {
                Queue<String> newTokens = new Queue<string>();
                while (tokens.Count != 0)
                {
                    var s = tokens.Dequeue();
                    s = s.Split('/')[0];
                    newTokens.Enqueue(s);
                }

                tokens = newTokens;
                
                double x, y, z;
                Next(out x);
                Next(out y);
                Next(out z);
                // .obj file count faces start from 1
                return new Vector3(x-1, y-1, z-1);
            }
            
                        
            public Vector3 ReadObjVector3()
            {
                double x, y, z;
                Next(out x);
                Next(out y);
                Next(out z);
                return new Vector3(x, y, z);
            }

            /// <summary>
            /// Consume line input and interpret next sequence as color.
            /// </summary>
            /// <returns>Interpreted color object (if valid)</returns>
            public Color ReadColor()
            {
                double r, g, b;
                Next("(");
                Next(out r);
                Next(",");
                Next(out g);
                Next(",");
                Next(out b);
                Next(")");
                return new Color(r, g, b);
            }

            /// <summary>
            /// Get next token from the line, with optional expected form.
            /// Throws error if expected form does not match, or no more tokens available.
            /// </summary>
            /// <returns>Next token</returns>
            private string Next(string expect = null)
            {
                try
                {
                    string value = this.tokens.Dequeue();
                    if (expect != null && value != expect)
                    {
                        throw new ParseException($"Expected '{expect}' but got '{value}'.", this.lineNumber);
                    }
                    return value;
                }
                catch (InvalidOperationException)
                {
                    throw new ParseException($"Line ended prematurely (expected additional tokens).", this.lineNumber);
                }
            }

            /// <summary>
            /// Get next token as string.
            /// </summary>
            /// <param name="value">Next token string</param>
            private void Next(out string value)
            {
                value = Next();
            }

            /// <summary>
            /// Get next token as double precision float.
            /// Throw error if it cannot be interpreted as a numeric value.
            /// </summary>
            /// <param name="value">Next token float</param>
            private void Next(out double value)
            {
                string next = Next();
                try
                {
                    value = Double.Parse(next);
                }
                catch (Exception)
                {
                    throw new ParseException($"Expected numeric value but got '{next}'.", this.lineNumber);
                }
            }

            /// <summary>
            /// The corresponding line number for this line.
            /// </summary>
            public int LineNumber { get { return this.lineNumber; } }
        }

        private Dictionary<String, Material> materials = new Dictionary<String, Material>();
        private Dictionary<String, SceneEntity> entities = new Dictionary<String, SceneEntity>();
        private Dictionary<String, PointLight> lights = new Dictionary<String, PointLight>();

        /// <summary>
        /// Construct a new scene reader object and parse it.
        /// </summary>
        /// <param name="filePath">File path to scene</param>
        public SceneReader(string filePath)
        {
            string[] lines = System.IO.File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                ParseLine(lines[i], i + 1);
            }
        }

        /// <summary>
        /// Populate a given scene with already parsed data.
        /// </summary>
        /// <param name="scene">Scene to populate</param>
        public void PopulateScene(Scene scene)
        {
            foreach (var entity in this.entities)
            {
                scene.AddEntity(entity.Value);
            }

            foreach (var light in this.lights)
            {
                scene.AddPointLight(light.Value);
            }
        }

        /// <summary>
        /// Parse a line in the scene file.
        /// </summary>
        /// <param name="lineText">Line data string</param>
        /// <param name="lineNumber">Line number (for error output)</param>
        private void ParseLine(string lineText, int lineNumber)
        {
            Line line = new Line(lineText, lineNumber);
            switch (line.Command())
            {
                case "Material":
                    ParseMaterial(line);
                    break;

                case "PointLight":
                    ParsePointLight(line);
                    break;

                case "Plane":
                    ParsePlane(line);
                    break;

                case "Sphere":
                    ParseSphere(line);
                    break;

                case "Triangle":
                    ParseTriangle(line);
                    break;

                case "ObjModel":
                    ParseObjModel(line);
                    break;

                case null:
                    break;

                default:
                    throw new ParseException($"Unknown command '{line.Command()}'.", lineNumber);
            }
        }

        /// <summary>
        /// Parse remaining line tokens as a material and store by key.
        /// </summary>
        /// <param name="line">Line to parse</param>
        private void ParseMaterial(Line line)
        {
            string identifier = line.ReadString();
            if (this.materials.ContainsKey(identifier))
            {
                throw new ParseException($"Material identifier '{identifier}' already in use.", line.LineNumber);
            }
            this.materials.Add(
                identifier,
                new Material(line.ReadEnum<Material.MaterialType>(), line.ReadColor(), line.ReadDouble())
            );
        }

        /// <summary>
        /// Parse remaining line tokens as a point light and store by key.
        /// </summary>
        /// <param name="line">Line to parse</param>
        private void ParsePointLight(Line line)
        {
            string identifier = line.ReadString();
            if (this.lights.ContainsKey(identifier))
            {
                throw new ParseException($"Light identifier '{identifier}' already in use.", line.LineNumber);
            }
            this.lights.Add(
                identifier,
                new PointLight(line.ReadVector3(), line.ReadColor())
            );
        }

        /// <summary>
        /// Parse remaining line tokens as a plane entity and store by key.
        /// </summary>
        /// <param name="line">Line to parse</param>
        private void ParsePlane(Line line)
        {
            string identifier = line.ReadString();
            if (this.entities.ContainsKey(identifier))
            {
                throw new ParseException($"Entity identifier '{identifier}' already in use.", line.LineNumber);
            }
            this.entities.Add(
                identifier,
                new Plane(line.ReadVector3(), line.ReadVector3(), ReadMaterial(line))
            );
        }

        /// <summary>
        /// Parse remaining line tokens as a sphere entity and store by key.
        /// </summary>
        /// <param name="line">Line to parse</param>
        private void ParseSphere(Line line)
        {
            string identifier = line.ReadString();
            if (this.entities.ContainsKey(identifier))
            {
                throw new ParseException($"Entity identifier '{identifier}' already in use.", line.LineNumber);
            }
            this.entities.Add(
                identifier,
                new Sphere(line.ReadVector3(), line.ReadDouble(), ReadMaterial(line))
            );
        }

        /// <summary>
        /// Parse remaining line tokens as a triangle entity and store by key.
        /// </summary>
        /// <param name="line">Line to parse</param>
        private void ParseTriangle(Line line)
        {
            string identifier = line.ReadString();
            if (this.entities.ContainsKey(identifier))
            {
                throw new ParseException($"Entity identifier '{identifier}' already in use.", line.LineNumber);
            }
            this.entities.Add(
                identifier,
                new Triangle(line.ReadVector3(), line.ReadVector3(), line.ReadVector3(), ReadMaterial(line))
            );
        }

        /// <summary>
        /// Parse remaining line tokens as an object (.obj) model and store by key.
        /// </summary>
        /// <param name="line">Line to parse</param>
        private void ParseObjModel(Line line)
        {
            string identifier = line.ReadString();
            if (this.entities.ContainsKey(identifier))
            {
                throw new ParseException($"Entity identifier '{identifier}' already in use.", line.LineNumber);
            }
            this.entities.Add(
                identifier,
                new ObjModel(line.ReadString(), line.ReadVector3(), line.ReadDouble(), ReadMaterial(line))
            );
        }

        /// <summary>
        /// Auto-lookup material by reading material key as next token.
        /// Throws error if material key does not exist.
        /// </summary>
        /// <param name="line">Line to parse next token</param>
        private Material ReadMaterial(Line line)
        {
            string identifier = line.ReadString();
            try
            {
                return this.materials[identifier];
            }
            catch (Exception)
            {
                throw new ParseException($"Referencing undefined material ('{identifier}').", line.LineNumber);
            }
        }
    }
}
