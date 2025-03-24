using System.Runtime.InteropServices;

namespace AssettoNet.Network.Struct
{
    /// <summary>
    /// Represents a 3D vector with <see cref="X"/>, <see cref="Y"/>, and <see cref="Z"/> components.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct Vec3
    {
        /// <summary>
        /// The X component of the vector.
        /// </summary>
        [FieldOffset(0)]
        public float X;

        /// <summary>
        /// The Y component of the vector.
        /// </summary>
        [FieldOffset(4)]
        public float Y;

        /// <summary>
        /// The Z component of the vector.
        /// </summary>
        [FieldOffset(8)]
        public float Z;

        /// <summary>
        /// Returns a string representation of the vector in the format: Vec3(X, Y, Z).
        /// </summary>
        /// <returns>A string representing the <see cref="Vec3"/>.</returns>
        public override string ToString()
        {
            return $"Vec3({X:F1}, {Y:F1}, {Z:F1})";
        }
    }
}
