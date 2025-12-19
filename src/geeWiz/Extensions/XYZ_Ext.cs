// geeWiz
using gCnv = geeWiz.Utilities.Convert_Utils;
using gGeo = geeWiz.Utilities.Geometry_Utils;

// The class belongs to the extensions namespace (partial class)
// XYZ xyz.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to points and vectors (XYZ).
    /// </summary>
    public static class XYZ_Ext
    {
        #region Point operations

        /// <summary>
        /// Overrides the values of an XYZ.
        /// </summary>
        /// <param name="point">The XYZ to modify.</param>
        /// <param name="x">Optional X value.</param>
        /// <param name="y">Optional Y value.</param>
        /// <param name="z">Optional X value.</param>
        /// <returns>A new XYZ.</returns>
        public static XYZ Ext_OverrideValues(this XYZ point, double? x = null, double? y = null, double? z = null)
        {
            // Null check
            if (point is null) { return null; }

            // Return a new point using the values
            return new XYZ(x ?? point.X, y ?? point.Y,z ?? point.Z);
        }

        /// <summary>
        /// Creates a line from a point.
        /// </summary>
        /// <param name="startPoint">The point to start at.</param>
        /// <param name="direction">The direction to project the line in.</param>
        /// <param name="length">The length of the line.</param>
        /// <returns>A Line.</returns>
        public static Line Ext_ToLine(this XYZ startPoint, XYZ direction, double length)
        {
            // Null checks
            if (startPoint is null || direction is null) { return null; }

            // Normalize the direction, project the point
            var normalizedDirection = direction.Normalize();
            var endPoint = startPoint + normalizedDirection * length;

            // Return the resultant line
            return Line.CreateBound(startPoint, endPoint);
        }

        /// <summary>
        /// Pulls a point to a plane.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="plane">The plane to pull it to.</param>
        /// <returns>A new XYZ.</returns>
        public static XYZ Ext_PullToPlane(this XYZ point, Plane plane)
        {
            // Null checks
            if (point is null || plane is null) { return point; }

            // Calculate the vector from the plane point to the given point
            XYZ vector = point - plane.Origin;

            // Calculate the distance from the point to the plane along the normal
            double distance = vector.DotProduct(plane.Normal);

            // Calculate the projection of the point onto the plane
            return point - (distance * plane.Normal);
        }

        /// <summary>
        /// Pulls a point to a curve.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="curve">The curve to pull it to.</param>
        /// <returns>A new XYZ.</returns>
        public static XYZ Ext_PullToCurve(this XYZ point, Curve curve)
        {
            // Null checks
            if (point is null || curve is null) { return point; }

            // Nearest point on curve result
            IntersectionResult result = curve.Project(point);

            // Catch projection failure, return point
            return result?.XYZPoint ?? point;
        }

        /// <summary>
        /// Gets the parameter at a point on a curve.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="curve">The curve to check.</param>
        /// <returns>A nullable double.</returns>
        public static double? Ext_ParameterAtPoint(this XYZ point, Curve curve)
        {
            // Null check
            if (curve == null || point == null) { return null; }

            // Project the point onto the curve
            IntersectionResult result = curve.Project(point);
            if (result == null) { return null; }

            // Get the parameter domain
            double parameterStart = curve.GetEndParameter(0);
            double parameterEnd = curve.GetEndParameter(1);
            double bound = parameterEnd - parameterStart;

            // Account for very small curves (avoid divide by zero)
            if (Math.Abs(parameterEnd - parameterStart) < 1e-9) { return null; }

            // Compute normalized parameter
            double parameter = (result.Parameter - parameterStart) / bound;

            // Clamp between 0 and 1 (just in case outside slightly)
            parameter = Math.Max(0.0, Math.Min(1.0, parameter));
            return parameter;
        }

        /// <summary>
        /// Bounces a ray from the given point.
        /// </summary>
        /// <param name="startPoint">The point to start from.</param>
        /// <param name="direction">The direction to cast the ray in.</param>
        /// <param name="intersector">The intersector to apply.</param>
        /// <returns>An XYZ point.</returns>
        public static XYZ Ext_RayBounceFromPoint(this XYZ startPoint, XYZ direction, ReferenceIntersector intersector)
        {
            // Null checks
            if (startPoint is null || direction is null || intersector is null) { return null; }

            // Create the ray intersection test
            var bounceResult = intersector.FindNearest(startPoint, direction);

            // If we have a valid result, return its point
            if (bounceResult is ReferenceWithContext)
            {
                return bounceResult.GetReference().GlobalPoint;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Vector operations

        /// <summary>
        /// Rotates a vector around an axis and point by an angle.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="angle">The angle to rotate by.</param>
        /// <param name="inputInDegrees">Is the angle in degrees.</param>
        /// <param name="rotatationPoint">The point to rotate around (origin by default).</param>
        /// <param name="axis">The axis to rotate around (Z axis by default).</param>
        /// <returns></returns>
        public static XYZ Ext_RotateVector(this XYZ vector, double angle, bool inputInDegrees = true, XYZ rotatationPoint = null, XYZ axis = null)
        {
            // Assign default values
            rotatationPoint ??= gGeo.POINT_ZERO;
            axis ??= gGeo.AXIS_Z;

            // Convert to radians if needed
            if (inputInDegrees) { angle = gCnv.DegreesToRadians(angle); }

            // Rotate the vector by a transform
            var transform = Transform.CreateRotationAtPoint(axis, angle, rotatationPoint);
            return transform.OfVector(vector);
        }

        /// <summary>
        /// Returns the angle from one vector to another in the clockswise direction.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="otherVector">The other vector.</param>
        /// <param name="returnInRadians">Return in degrees.</param>
        /// <returns>The angle.</returns>
        public static double Ext_AngleToVectorOnXyPlane(this XYZ vector, XYZ otherVector = null, bool returnInRadians = false)
        {
            // Default reference vector is +Y axis
            otherVector ??= gGeo.AXIS_Y;

            // Dervive X and Y components
            double vx = vector.X;
            double vy = vector.Y;
            double ox = otherVector.X;
            double oy = otherVector.Y;

            // Compute angle using atan2, cross and dot in XY
            double radians = Math.Atan2(vx * oy - vy * ox, vx * ox + vy * oy);

            // Convert negative angles to positive 0–2π
            if (radians < 0) { radians += 2 * gCnv.MATH_PI; }

            // Return the angle
            return returnInRadians ? radians : gCnv.RadiansToDegrees(radians);
        }

        #endregion

        #region Coordinate conversion

        /// <summary>
        /// Converts an XYZ from internal to actual coordinates.
        /// </summary>
        /// <param name="point">The point to transform.</param>
        /// <param name="doc">The related document.</param>
        /// <returns>An XYZ.</returns>
        public static XYZ Ext_ToActualCoordinates(this XYZ point, Document doc = null)
        {
            // Null checks
            if (point is null) { return null; }
            doc ??= Globals.CurrentDocument();

            // Return the transformed point
            return gCnv.InternalToActualTransform(doc).OfPoint(point);
        }

        /// <summary>
        /// Converts an XYZ from actual to internal coordinates.
        /// </summary>
        /// <param name="point">The point to transform.</param>
        /// <param name="doc">The related document.</param>
        /// <returns>An XYZ.</returns>
        public static XYZ Ext_ToInternalCoordinates(this XYZ point, Document doc = null)
        {
            // Null checks
            if (point is null) { return null; }
            doc ??= Globals.CurrentDocument();

            // Return the transformed point
            return gCnv.ActualToInternalTransform(doc).OfPoint(point);
        }

        #endregion
    }
}