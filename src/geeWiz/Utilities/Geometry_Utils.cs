// Revit API
using Autodesk.Revit.DB;

// The class belongs to the utility namespace
// using gGeo = geeWiz.Utilities.Geometry_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Static methods container related to geometry.
    /// </summary>
    public static class Geometry_Utils
    {
        #region Constants

        /// <summary>
        /// The world origin.
        /// </summary>
        public static readonly XYZ POINT_ZERO = XYZ.Zero;

        /// <summary>
        /// A vector of zero length.
        /// </summary>
        public static readonly XYZ VECTOR_ZERO = XYZ.Zero;

        /// <summary>
        /// Canonical X vector.
        /// </summary>
        public static readonly XYZ AXIS_X = XYZ.BasisX;

        /// <summary>
        /// Canonical Y vector.
        /// </summary>
        public static readonly XYZ AXIS_Y = XYZ.BasisY;

        /// <summary>
        /// Canonical Z vector.
        /// </summary>
        public static readonly XYZ AXIS_Z = XYZ.BasisZ;

        /// <summary>
        /// Canonical reversed X vector.
        /// </summary>
        public static readonly XYZ AXIS_NEGX = AXIS_X.Negate();

        /// <summary>
        /// Canonical reversed Y vector.
        /// </summary>
        public static readonly XYZ AXIS_NEGY = AXIS_Y.Negate();

        /// <summary>
        /// Canonical reversed Z vector.
        /// </summary>
        public static readonly XYZ AXIS_NEGZ = AXIS_Z.Negate();

        /// <summary>
        /// UV value of 0.0, 0.0.
        /// </summary>
        public static readonly UV UV_ZERO = UV.Zero;

        /// <summary>
        /// UV value of 0.5, 0.5.
        /// </summary>
        public static readonly UV UV_MID = new UV((double)0.5, (double)0.5);

        /// <summary>
        /// Canonical reversed XY vector.
        /// </summary>
        public static readonly Plane PLANE_XY = Plane.CreateByNormalAndOrigin(AXIS_Z, POINT_ZERO);

        /// <summary>
        /// Canonical reversed YZ vector.
        /// </summary>
        public static readonly Plane PLANE_YZ = Plane.CreateByNormalAndOrigin(AXIS_X, POINT_ZERO);

        /// <summary>
        /// Canonical reversed XZ vector.
        /// </summary>
        public static readonly Plane PLANE_XZ = Plane.CreateByNormalAndOrigin(AXIS_Y, POINT_ZERO);

        #endregion

        #region SpatialElements

        /// <summary>
        /// Returns a boundary option for spatial elements (rooms etc.).
        /// </summary>
        /// <param name="boundaryLocation">A boundary location (finish by default).</param>
        /// <returns>A SpatialElementBoundaryOptions object.</returns>
        public static SpatialElementBoundaryOptions CreateSpaceBoundaryOptions(
            SpatialElementBoundaryLocation boundaryLocation = SpatialElementBoundaryLocation.Finish)
        {
            // Return the optiuons
            return new SpatialElementBoundaryOptions() { SpatialElementBoundaryLocation = boundaryLocation };
        }

        #endregion
    }
}
