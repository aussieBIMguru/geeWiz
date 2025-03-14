﻿// The class belongs to the utility namespace
// using gSpa = geeWiz.Utilities.Spatial_Utils

namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to SpatialElements
    /// </summary>
    public static class Spatial_Utils
    {
        #region Matrix of rooms by placement

        /// <summary>
        /// Constructs a list of list of rooms, where each list is by a placement status.
        /// 
        /// Lists are as follows:
        /// - Index 0: Placed and valid
        /// - Index 1: Redundant
        /// - Index 2: Unenclosed
        /// - Index 3: Unplaced
        /// 
        /// </summary>
        /// <param name="rooms">The Rooms to sort.</param>
        /// <param name="doc">The related document.</param>
        /// <returns>A matrix (list of lists) of Rooms.</returns>
        public static List<List<SpatialElement>> RoomsMatrixByPlacement(List<SpatialElement> rooms, Document doc)
        {
            // Rooms to return
            var roomsValid = new List<SpatialElement>();
            var roomsRedundant = new List<SpatialElement>();
            var roomsUnenclosed = new List<SpatialElement>();
            var roomsUnplaced = new List<SpatialElement>();

            // Construct the boundary location object
            var boundaryLocation = AreaVolumeSettings
                .GetAreaVolumeSettings(doc)
                .GetSpatialElementBoundaryLocation(SpatialElementType.Room);

            // Construct the spatial element boundary options
            var options = new SpatialElementBoundaryOptions();
            options.SpatialElementBoundaryLocation = boundaryLocation;

            // For each room...
            foreach (var room in rooms)
            {
                // Area greater than 0 = Placed
                if (room.Area != 0)
                {
                    roomsValid.Add(room);
                }
                // Edges found = Redundant
                else if (room.GetBoundarySegments(options).Count > 0)
                {
                    roomsRedundant.Add(room);
                }
                // No location = Unplaced
                else if (room.Location is null)
                {
                    roomsUnplaced.Add(room);
                }
                // Otherwise = Unenclosed
                else
                {
                    roomsUnenclosed.Add(room);
                }
            }

            // Return the rooms in a matrix
            return new List<List<SpatialElement>>()
            {
                roomsValid, roomsRedundant, roomsUnenclosed, roomsUnplaced
            };
        }

        #endregion
    }
}