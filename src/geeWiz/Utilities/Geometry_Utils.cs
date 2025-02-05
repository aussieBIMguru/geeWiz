// The class belongs to the utility namespace
// using gGeo = geeWiz.Utilities.Geometry_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to geometry.
    /// </summary>
    public static class Geometry_Utils
    {
        #region Constants

        // Point and vector origins/zeroes
        public static readonly XYZ POINT_ZERO = XYZ.Zero;
        public static readonly XYZ VECTOR_ZERO = XYZ.Zero;

        // Axis (canonical vectors)
        public static readonly XYZ AXIS_X = XYZ.BasisX;
        public static readonly XYZ AXIS_Y = XYZ.BasisY;
        public static readonly XYZ AXIS_Z = XYZ.BasisZ;

        // Reverse axis (canonical negated vectors)
        public static readonly XYZ AXIS_NEGX = AXIS_X.Negate();
        public static readonly XYZ AXIS_NEGY = AXIS_Y.Negate();
        public static readonly XYZ AXIS_NEGZ = AXIS_Z.Negate();

        // UV values
        public static readonly UV UV_ZERO = UV.Zero;
        public static readonly UV UV_MID = new UV((double)0.5, (double)0.5);

        // Base planes
        public static readonly Plane PLANE_XY = Plane.CreateByNormalAndOrigin(AXIS_Z, POINT_ZERO);
        public static readonly Plane PLANE_YZ = Plane.CreateByNormalAndOrigin(AXIS_X, POINT_ZERO);
        public static readonly Plane PLANE_XZ = Plane.CreateByNormalAndOrigin(AXIS_Y, POINT_ZERO);

        #endregion
    }
}
