using System;

namespace BaldiMapExporter.Models
{
    [Serializable]
    internal class MapWrapper
    {
        public int Seed;

        public int Level;

        public string LevelTitle;

        public IntVector2 LevelSize;

        public long Lt;

        public RoomWrapper[] Rooms;

        public ElevatorWraper[] Elv;
    }

    [Serializable]
    internal class RoomWrapper
    {
        public string N;

        public string Flor;

        public string Wall;

        public string Ceil;

        public CellWrapper[] Cells;

        public DoorWrapper[] DW;
    }

    [Serializable]
    internal class CellWrapper
    {
        public long Pos;

        public float Lvl;

        public int Sha;

        public int LO;

        public int[] Dir;
    }

    [Serializable]
    internal class ElevatorWraper
    {
        public long Pos;
        public int Dir;
    }

    [Serializable]
    internal class DoorWrapper
    {
        public long Pos;

        public int Dir;

        public string T;

        public string AR;

        public string BR;
    }
}
