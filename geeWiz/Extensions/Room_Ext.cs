// geeWiz
using Parameter = Autodesk.Revit.DB.Parameter;
using Room = Autodesk.Revit.DB.Architecture.Room;

// The class belongs to the extensions namespace
// Room room.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to Rooms.
    /// </summary>
    public static class Room_Ext
    {
        /// <summary>
        /// Returns the Room's name.
        /// </summary>
        /// <param name="room">A Revit Room.</param>
        /// <returns>A string.</returns>
        public static string Ext_GetRoomName(this Room room)
        {
            // Null catch
            if (room is null) { return "???"; }

            // Get Room name parameter
            Parameter parameter = (room as Element).Ext_GetBuiltInParameter(BuiltInParameter.ROOM_NAME);

            // Return the name
            return parameter.AsString();
        }

        /// <summary>
        /// Constructs a name key based on a Revit Room.
        /// </summary>
        /// <param name="room">A Revit Room.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToRoomKey(this Room room, bool includeId = false)
        {
            // Null catch
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
    }
}