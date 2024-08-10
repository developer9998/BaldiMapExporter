using System;

namespace BaldiMapExporter.Models
{
    [Serializable]
    internal class MapWrapper
    {
        /// <summary>
        /// The seed of the map.
        /// </summary>
        public int Seed;

        /// <summary>
        /// The level of the map that represents the zero-based floor.
        /// </summary>
        public int Level;

        /// <summary>
        /// The level title of the map.
        /// </summary>
        public string LevelTitle;

        /// <summary>
        /// The level size of the map.
        /// </summary>
        public IntVector2 LevelSize;

        /// <summary>
        /// The lighting colour of the map.
        /// </summary>
        public long Lt;

        /// <summary>
        /// The collection of rooms in the map.
        /// </summary>
        public RoomWrapper[] Rooms;

        /// <summary>
        /// The collection of elevators in the map.
        /// </summary>
        public ElevatorWraper[] Elv;
    }

    [Serializable]
    internal class RoomWrapper
    {
        /// <summary>
        /// The name of the room.
        /// </summary>
        public string N;

        /// <summary>
        /// The name of the floor texture the room uses.
        /// </summary>
        public string Flor;
        
        /// <summary>
        /// The name of the wall texture the room uses.
        /// </summary>
        public string Wall;

        /// <summary>
        /// The name of the ceiling texture the room uses.
        /// </summary>
        public string Ceil;

        /// <summary>
        /// The collection of cells, or tiles in the room.
        /// </summary>
        public CellWrapper[] Cells;

        /// <summary>
        /// The collection of doors in the room. 
        /// </summary>
        public DoorWrapper[] DW;
    }

    [Serializable]
    internal class CellWrapper
    {
        /// <summary>
        /// The position of the cell.
        /// </summary>
        public long Pos;

        /// <summary>
        /// The light level of the cell.
        /// </summary>
        public float Lvl;

        /// <summary>
        /// The shape of the cell.
        /// </summary>
        public int Sha;

        /// <summary>
        /// The lighting object index accociated with the cell.
        /// </summary>
        public int LO;

        /// <summary>
        /// The wall directions of the cell.
        /// </summary>
        public int[] Dir;
    }

    [Serializable]
    internal class ElevatorWraper
    {
        /// <summary>
        /// The position of the elevator.
        /// </summary>
        public long Pos;

        /// <summary>
        /// The direction of the elevator.
        /// </summary>
        public int Dir;
    }

    [Serializable]
    internal class DoorWrapper
    {
        /// <summary>
        /// The position of the door.
        /// </summary>
        public long Pos;

        /// <summary>
        /// The direction of the door.
        /// </summary>
        public int Dir;

        /// <summary>
        /// The type of door (Door and other classes that inherit the class, such as StandardDoor, SwingDoor, LockdownDoor, Window, etc).
        /// </summary>
        public string T;

        /// <summary>
        /// The name of the room on the A-side of the door.
        /// </summary>
        public string AR;

        /// <summary>
        /// The name of the room on the B-side of the door.
        /// </summary>
        public string BR;
    }
}
