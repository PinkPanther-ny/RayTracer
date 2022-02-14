
namespace RayTracer
{
    /// <summary>
    /// Interface to represent an entity (object) in a ray traced scene. 
    /// All of our primitive types -- planes, triangles, spheres --
    /// implement this interface. If you complete stage 3 add-on C, you'll 
    /// notice this interface is also used to implement 3D OBJ models.
    /// 
    /// This interface allows us to "abstract" the specific implementation
    /// details for various object/shape collisions. From the perspective of
    /// the ray tracing code, it shouldn't matter exactly what the entity is. 
    /// All that matters is that we can test if a ray collides with it, and
    /// the associated collision/hit information if it does. Furthermore, 
    /// we need to be able to check what the material of the entity is in 
    /// order to figure out how the ray should interact with it.
    /// </summary>
    public interface SceneEntity
    {
        /// <summary>
        /// Check whether a given ray intersects with this entity.
        /// If so, return hit data. Otherwise, return null.
        /// </summary>
        /// <param name="ray">Ray data</param>
        /// <returns>Ray hit data, or null if no intersection</returns>
        RayHit Intersect(Ray ray);

        bool Inside(Vector3 position);

        /// <summary>
        /// The material assigned to this entity.
        /// </summary>
        Material Material { get; }
    }
}
