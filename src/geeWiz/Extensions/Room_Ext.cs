// Autodesk
using Autodesk.Revit.DB.Architecture;
// geeWiz
using gGeo = geeWiz.Utilities.Geometry_Utils;

// The class belongs to the extensions namespace
// Room room.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to Rooms.
    /// </summary>
    public static class Room_Ext
    {
        #region Namekey

        /// <summary>
        /// Returns a room's name.
        /// </summary>
        /// <param name="room">A Room (extended).</param>
        /// <returns>A string.</returns>
        public static string Ext_GetRoomName(this Room room)
        {
            // Null check
            if (room is null) { return "???"; }

            // Get the parameter
            var parameter = room.Ext_GetBuiltInParameter(BuiltInParameter.ROOM_NAME);

            // Return the name
            return parameter.AsString();
        }

        /// <summary>
        /// Constructs a name key based on a Room.
        /// </summary>
        /// <param name="room">A Room (extended).</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToRoomNameKey(this Room room, bool includeId = false)
        {
            // Null check
            if (room is null) { return "???"; }

            // Return key with Id
            if (includeId)
            {
                return $"{room.Number}: {room.Ext_GetRoomName()} [{room.Id.ToString()}]";
            }
            // Return key without Id
            else
            {
                return $"{room.Number}: {room.Ext_GetRoomName()}";
            }
        }

        #endregion

        #region Room geometry

        /// <summary>
        /// Returns the boundaries of a room, as curveloops.
        /// </summary>
        /// <param name="room">A Room (extended).</param>
        /// <param name="options">Options to use (optional).</param>
        /// <returns>A list of CurveLoops.</returns>
        public static List<CurveLoop> Ext_GetRoomBoundaries(this Room room, SpatialElementBoundaryOptions options = null)
        {
            // Default options
            options ??= gGeo.CreateSpaceBoundaryOptions();

            // List of curveloops to output
            var curveLoops = new List<CurveLoop>();

            // For each boundary...
            foreach (var boundary in room.GetBoundarySegments(options))
            {
                // Make a new curveloop
                var curveLoop = new CurveLoop();

                // For each edge in the boundary, add its curve
                foreach (var edge in boundary)
                {
                    curveLoop.Append(edge.GetCurve());
                }
                
                // Add the curveloop
                curveLoops.Add(curveLoop);
            }

            // Return the curveloops list
            return curveLoops;
        }

        /// <summary>
        /// Returns the edge curves of a Room.
        /// </summary>
        /// <param name="room">A Room (extended).</param>
        /// <param name="options">Options to use (optional).</param>
        /// <returns>A list of Curves.</returns>
        public static List<Curve> Ext_GetRoomCurves(this Room room, SpatialElementBoundaryOptions options = null)
        {
            // Default options
            options ??= gGeo.CreateSpaceBoundaryOptions();

            // List of curves to output
            var curves = new List<Curve>();

            // For each boundary...
            foreach (var boundary in room.GetBoundarySegments(options))
            {
                // Add each curve
                foreach (var edge in boundary)
                {
                    curves.Add(edge.GetCurve());
                }
            }
            
            // Return the curves
            return curves;
        }

        /// <summary>
        /// Returns the solid of a Room.
        /// </summary>
        /// <param name="room">A Room (extended).</param>
        /// <param name="options">Options to use (optional).</param>
        /// <returns>A Solid.</returns>
        public static Solid Ext_GetRoomSolid(this Room room, SpatialElementBoundaryOptions options = null, Document doc = null)
        {
            // Default options and document
            options ??= gGeo.CreateSpaceBoundaryOptions();
            doc ??= room.Document;

            // Calculate the room solid
            var geometryCalculator = new SpatialElementGeometryCalculator(doc, options);
            var spatialGeometry = geometryCalculator.CalculateSpatialElementGeometry(room);

            // Return the geometry
            return spatialGeometry.GetGeometry();
        }

        #endregion
    }
}