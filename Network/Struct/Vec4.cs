using System.Runtime.InteropServices;

namespace AssettoNet.Network.Struct
{
    /// <summary>
    /// Represents a 4D vector with <see cref="X"/>, <see cref="Y"/>, <see cref="Z"/>, and <see cref="W"/> components.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct Vec4
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
        /// The W component of the vector.
        /// </summary>
        [FieldOffset(12)]
        public float W;

        /// <summary>
        /// Returns a string representation of the vector in the format: Vec4(X, Y, Z, W).
        /// </summary>
        /// <returns>A string representing the <see cref="Vec4"/>.</returns>
        public override string ToString()
        {
            return $"Vec4({X:F1}, {Y:F1}, {Z:F1}, {W:F1})";
        }
    }
}
