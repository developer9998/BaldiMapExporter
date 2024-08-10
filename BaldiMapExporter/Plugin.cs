using BaldiMapExporter.Models;
using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaldiMapExporter
{
    [BepInPlugin(Constants.GUID, Constants.Name, Constants.Version)]
    internal class Plugin : BaseUnityPlugin
    {
        public string SceneName => SceneManager.GetActiveScene().name;
        public bool IsProcessing;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                ProcessGame();
            }
        }

        public void ProcessGame()
        {
            if (SceneName != "Game" || IsProcessing) return;

            IsProcessing = true;

            Logger.LogInfo("Processing");

            Logger.LogInfo("step 1: defining existing refs");

            EnvironmentController ec = Singleton<BaseGameManager>.Instance.Ec;

            var mainHall = ec.mainHall;

            var rooms = ec.rooms;

            var elevators = ec.elevators;

            var elevatorPositions = elevators.Select(selector => IntVector2.GetGridPosition(selector.Door.transform.position).ToString()).ToArray();

            long PackVector3ToLong(Vector3 vector)
            {
                long num = (long)Mathf.Clamp(Mathf.RoundToInt(vector.x * 1024f) + 1048576, 0, 2097151);
                long num2 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.y * 1024f) + 1048576, 0, 2097151);
                long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(vector.z * 1024f) + 1048576, 0, 2097151);
                return num + (num2 << 21) + (num3 << 42);
            }

            short PackColor(Color col)
            {
                return (short)(Mathf.RoundToInt(col.r * 9f) + Mathf.RoundToInt(col.g * 9f) * 10 + Mathf.RoundToInt(col.b * 9f) * 100);
            }

            RoomWrapper GetRoomWrapper(RoomController rc, bool scanElevators)
            {
                RoomWrapper rw = new RoomWrapper();

                Logger.LogInfo($"creating wrapper for {rc.name}");

                rw.N = rc.name;

                rw.Flor = rc.florTex.name;

                rw.Wall = rc.wallTex.name;

                rw.Ceil = rc.ceilTex.name;

                rw.DoO = rc.doorMats.open.mainTexture.name;

                rw.DoS = rc.doorMats.shut.mainTexture.name;

                Logger.LogInfo("defining cells");

                rw.Cells = new CellWrapper[rc.cells.Count];

                for(int i = 0; i < rc.cells.Count; i++)
                {
                    if (rc.cells[i] == null) continue;

                    Cell cell = rc.cells[i];

                    Vector3 centrePosition = cell.Tile.transform.position;

                    if (scanElevators)
                    {
                        var elevatorCheckPosition = IntVector2.GetGridPosition(centrePosition);

                        if (elevatorPositions.Contains(elevatorCheckPosition.ToString())) continue;
                    }

                    CellWrapper cw = new CellWrapper();

                    cw.Pos = PackVector3ToLong(centrePosition);

                    cw.Sha = (int)rc.cells[i].shape;

                    cw.Dir = rc.cells[i].AllWallDirections.Select(selector => (int)selector).ToArray();

                    if (cell.Tile && cell.Tile.transform.Find("FluorescentLight(Clone)"))
                    {
                        cw.LO = 1;
                    }

                    float level = ec.LightLevel(centrePosition);

                    cw.Lvl = level;

                    rw.Cells[i] = cw;
                }

                var doors = rc.gameObject.GetComponentsInChildren<Door>(false);

                rw.DW = doors.Select(selector =>
                {
                    DoorWrapper dw = new DoorWrapper();
                    dw.T = selector.GetType().Name;
                    dw.Pos = PackVector3ToLong(Vector3.Lerp(selector.aTile.TileTransform.position, selector.bTile.TileTransform.position, 1f / 2f));
                    dw.Dir = (int)selector.direction;
                    dw.AR = selector.aTile.room.name;
                    dw.BR = selector.bTile.room.name;
                    return dw;
                }).ToArray();

                Logger.LogInfo("done");

                return rw;
            }

            Logger.LogInfo("step 2: creating the map wrapper");

            MapWrapper mw = new MapWrapper();

            mw.Seed = Singleton<CoreGameManager>.Instance.Seed();

            mw.Level = Singleton<CoreGameManager>.Instance.sceneObject.levelNo;
            
            mw.LevelTitle = Singleton<CoreGameManager>.Instance.sceneObject.levelTitle;

            mw.LevelSize = ec.levelSize;

            long pack = PackColor((Color)AccessTools.Field(ec.GetType(), "lighting").GetValue(ec)); // TODO: find functional method for getting lighting of map, this doesnt work and is equivalent to Color.white

            Logger.LogInfo(pack);

            mw.Lt = pack;

            Logger.LogInfo("step 3: define the rooms");

            mw.Rooms = new RoomWrapper[rooms.Count + 1];

            mw.Rooms[0] = GetRoomWrapper(mainHall, true);

            for (int i = 0; i < rooms.Count; i++)
            {
                mw.Rooms[i + 1] = GetRoomWrapper(rooms[i], rooms[i].category == RoomCategory.Special);
            }

            Logger.LogInfo("step 4: define the elevators");

            mw.Elv = new ElevatorWraper[elevators.Count];

            for(int i = 0; i < mw.Elv.Length; i++)
            {
                var elv = elevators[i];

                var pos = elv.Door.transform.position;

                ElevatorWraper ew = new ElevatorWraper();

                ew.Pos = PackVector3ToLong(pos);

                ew.Dir = (int)elv.Door.direction;

                mw.Elv[i] = ew;
            }

            Logger.LogInfo("step 5: write content");

            string contents = JsonConvert.SerializeObject(mw, Formatting.None);

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "RuntimeMapExport", $"MapExport ({mw.Seed}, {mw.LevelTitle}).txt"), contents);

            IsProcessing = false;

            Logger.LogInfo("Map success");
        }
    }
}
