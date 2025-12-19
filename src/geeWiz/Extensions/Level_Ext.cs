// geeWiz
using gGeo = geeWiz.Utilities.Geometry_Utils;

// The class belongs to the extensions namespace
// Level level.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to DesignOptions.
    /// </summary>
    public static class Level_Ext
    {
        #region NameKey

        /// <summary>
        /// Process a Level into a name key.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="includeId">Include the ElementId on the end.</param>
        /// <returns>The name key.</returns>
        public static string Ext_ToLevelNameKey(this Level level, bool includeId = false)
        {
            // Null catch
            if (level is null) { return "???"; }

            // Return namekey
            return level.Ext_FinalizeKey(level.Name, includeId);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Returns the plane at a Revit level's elevation.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>A Plane.</returns>
        public static Plane Ext_GetElevationPlane(this Level level)
        {
            // Null catch
            if (level == null) { return null; }

            // Define the origin point of the plane using the level's elevation
            XYZ origin = new XYZ(0, 0, level.Elevation);

            // Return the Revit Plane
            return Plane.CreateByNormalAndOrigin(gGeo.AXIS_Z, origin);
        }

        #endregion
    }
}