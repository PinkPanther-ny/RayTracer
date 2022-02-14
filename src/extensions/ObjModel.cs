using System;
using System.Collections.Generic;
using System.IO;

namespace RayTracer
{
    /// <summary>
    /// Add-on option C. You should implement your solution in this class template.
    /// </summary>
    public class ObjModel : SceneEntity
    {
        private Material material;

        private List<Vector3> vertexList;
        private List<Vector3> vertexNormalList;
        private List<Vector3> faceList;

        private List<Triangle> triangles;

        private double scale;
        private Vector3 offset;
        private double maxRadiusSq;

        private Sphere boundingSphere;
        /// <summary>
        /// Construct a new OBJ model.
        /// </summary>
        /// <param name="objFilePath">File path of .obj</param>
        /// <param name="offset">Vector each vertex should be offset by</param>
        /// <param name="scale">Uniform scale applied to each vertex</param>
        /// <param name="material">Material applied to the model</param>
        public ObjModel(string objFilePath, Vector3 offset, double scale, Material material)
        {
            this.material = material;
            this.vertexList = new List<Vector3>();
            this.vertexNormalList = new List<Vector3>();
            this.faceList = new List<Vector3>();
            
            this.boundingSphere = null;
            this.maxRadiusSq = 0;
            
            int lineNumber = 1;
            this.scale = scale;
            this.offset = offset;
            
            foreach (var line in File.ReadAllLines(objFilePath))
            {
                ProcessLine(line, lineNumber++);
            }
            this.boundingSphere = new Sphere(offset, Math.Sqrt(maxRadiusSq) + 0.2, material);
            this.triangles = new List<Triangle>();
            foreach (var vertIndex in faceList)
            {
                this.triangles.Add(new Triangle( 
                    vertexList[(int)vertIndex.X], 
                    vertexList[(int)vertIndex.Y], 
                    vertexList[(int)vertIndex.Z], 
                    this.material ));
            }

        }

        public void ProcessLine(string lineText, int lineNumber)
        {
            SceneReader.Line line = new SceneReader.Line(lineText, lineNumber);
            switch (line.Command())
            {
                case "v":
                    var v = line.ReadObjVector3();
                    // Update radius of the bounding sphere
                    var tempRadiusSq = v.LengthSq();
                    if (tempRadiusSq > maxRadiusSq)
                    {
                        maxRadiusSq = tempRadiusSq;
                    }
                    vertexList.Add(scale * v + offset);
                    break;
                case "vn":
                    vertexNormalList.Add(line.ReadObjVector3());
                    break;
                case "f":
                    faceList.Add(line.ReadFaceVector3());
                    break;
            }
            
        }

        /// <summary>
        /// Given a ray, determine whether the ray hits the object
        /// and if so, return relevant hit data (otherwise null).
        /// </summary>
        /// <param name="ray">Ray data</param>
        /// <returns>Ray hit data, or null if no hit</returns>
        public RayHit Intersect(Ray ray)
        {
            if (boundingSphere.Intersect(ray) == null)
            {
                return null;
            }
            
            RayHit rayHit = null;
            double minDist = 99999;
            foreach (var face in faceList)
            {
                Vector3 v0 = vertexList[(int)face.X], v1 = vertexList[(int)face.Y], v2 = vertexList[(int)face.Z];
                Vector3 center = new Vector3(
                    (v0.X + v1.X + v2.X) / 3, 
                    (v0.Y + v1.Y + v2.Y) / 3, 
                    (v0.Z + v1.Z + v2.Z) / 3
                    );
                Vector3 normal = (v1 - v0).Cross(v2 - v0).Normalized();
                if (normal.Dot(ray.Direction) != 0)
                {
                    // Inside-outside test
                    double t = (center-ray.Origin).Dot(normal) / ray.Direction.Dot(normal);
                    Vector3 p = ray.Origin + t * ray.Direction;
                
                    // Now it's a valid hit.
                    // Check if it's the closest.
                    if (normal.Dot((v1 - v0).Cross(p - v0)) >= 0 &&
                        normal.Dot((v2 - v1).Cross(p - v1)) >= 0 &&
                        normal.Dot((v0 - v2).Cross(p - v2)) >= 0 &&
                        t >= 0 && t < minDist)
                    {
                        rayHit = new RayHit(p + 0.000001 * normal, normal, ray.Direction, this);
                        minDist = t;
                    }
                
                }
            }
            return rayHit;
        }

        /// <summary>
        /// The material attached to this object.
        /// </summary>
        public Material Material { get { return this.material; } }

        public bool Inside(Vector3 position)
        {
            return false;
        }
    }

}
