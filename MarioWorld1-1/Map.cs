using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using GameFramework;

namespace MarioWorld1_1 {
    class Map {
        protected Tile[][] tileMap = null;

        public Tile[] this[int i] {
            get {
                //Console.WriteLine("tileMap length: " + tileMap.Length);
                //Console.WriteLine("[i]: " + i);
                //Console.WriteLine("tileMap[i].Length "+tileMap[i].Length);
                return tileMap[i];
            }
        }
        public int Length {
            get {
                return tileMap.Length;
            }
        }
        protected Point spawnTile = new Point(0, 0);
        protected string tileSheet = null;
        protected Dictionary<string, Point> nextRoom = null;

        //protected List<EnemyCharacter> enemies = null;
        public Map(string mapPath,PlayerCharacter hero) {
            if (System.IO.File.Exists(mapPath)) {
                List<int> unwalkableTiles = new List<int>();
                List<int> doorIndex = new List<int>();
                Dictionary<int, Rectangle> spriteSources = new Dictionary<int, Rectangle>();
                List<List<int>> mapFormat = new List<List<int>>();
                Dictionary<int, string> nextMap = new Dictionary<int, string>();
                nextRoom = new Dictionary<string, Point>();

                //load map
                using (TextReader reader = File.OpenText(mapPath)) {
                    string contents = reader.ReadLine();
                    while (contents != null) {
                        string[] content = contents.Split(',');
                        if (content.Length == 1) {
                            content = contents.Split(' ');
                        }
                        //load texture
                        if (content[0] == "T") {
                            string path = content[1];
                            tileSheet = path;
#if DEBUG
                            Console.WriteLine("Texture Path: " + tileSheet);
#endif
                        }
                        //load source rects
                        else if (content[0] == "R") {
                            //source rect
                            Rectangle r = new Rectangle(System.Convert.ToInt32(content[2]), System.Convert.ToInt32(content[3]), System.Convert.ToInt32(content[4]), System.Convert.ToInt32(content[5]));
                            //adds rect index and source rect to dictionary
                            spriteSources.Add(System.Convert.ToInt32(content[1]), r);
#if DEBUG
                            Console.WriteLine("Rectangle Added: " + r);
#endif
                        }
                        //figure out walkable tiles
                        else if (content[0] == "U") {
                            for (int i = 1; i < content.Length; i++) {
                                unwalkableTiles.Add(System.Convert.ToInt32(content[i]));
#if DEBUG
                                Console.WriteLine("Unwalkable Tiles: " + content[i]);
#endif
                            }
                        }
                        //door Tiles
                        else if (content[0] == "D") {
                            //identifies which tile is a door
                            doorIndex.Add(System.Convert.ToInt32(content[1]));
                            //door destination
                            nextRoom.Add(content[2], new Point(System.Convert.ToInt32(content[3]), System.Convert.ToInt32(content[4])));
                            //door spawn destination
                            nextMap.Add(System.Convert.ToInt32(content[1]), content[2]);
#if DEBUG
                            Console.WriteLine("Door tile index: " + content[1]);
                            Console.WriteLine("Next room path: " + content[2]);
                            Console.WriteLine("Door spaw location: " + content[3] + "," + content[4]);
#endif
                        }
                        //starting tile
                        else if (content[0] == "S") {
                            spawnTile = new Point(System.Convert.ToInt32(content[1]), System.Convert.ToInt32(content[2]));
#if DEBUG
                            Console.WriteLine("Starting tile: " + content[1] + ", " + content[2]);
#endif
                        }
                        //add enemies
                        else if(content[0] == "E") {
                            /*if (enemies == null) {
                                enemies = new List<EnemyCharacter>();
                            }
                            bool upDownMove = content[2] == "X" ? false : true;
                            enemies.Add(new EnemyCharacter(content[1],upDownMove));
                            enemies[enemies.Count-1].Position.X = System.Convert.ToInt32(content[3])*Game.TILE_SIZE;
                            enemies[enemies.Count-1].Position.Y = System.Convert.ToInt32(content[4])*Game.TILE_SIZE;
                            #if DEBUG
                            Console.WriteLine("Enemy added, Y Axis Movement: " + upDownMove);
                            Console.WriteLine("Enemy sprite path: "+content[1]);
                            #endif
                            */
                        }
                        else if(content[0] == "/") {
                            //used to make comments in txt file!
                        }
                        //load rows
                        else if(System.Convert.ToInt32(content[0]) >= 0) {
                            //create new row
                            mapFormat.Add(new List<int>());
                            for (int i = 0; i < content.Length; i++) {
                                //add numbers to row
                                if (string.IsNullOrEmpty(content[i])){
                                    continue;
                                }
                                mapFormat[mapFormat.Count - 1].Add(System.Convert.ToInt32(content[i]));
                            }
#if DEBUG
                            Console.WriteLine("Row created");
#endif
                        }
                        contents = reader.ReadLine();
                    }
                }
                //create map
                int rows = mapFormat.Count;
                int cols = mapFormat[0].Count;
                tileMap = new Tile[rows][];
                for (int i = 0; i < rows; i++) {
                    tileMap[i] = new Tile[cols];
                    //create individual tile
                    for (int j = 0; j < cols; j++) {
                        //mapFormat[i][j] = individual tile
                        Rectangle source = spriteSources[mapFormat[i][j]];
                        Point worldPosition = new Point();
                        worldPosition.X = (j * source.Width);
                        worldPosition.Y = (i * source.Height);
                        tileMap[i][j] = new Tile(tileSheet, source);
                        tileMap[i][j].Walkable = true;
                        //check if it's a door
                        for (int k = 0; k < doorIndex.Count; k++) {
                            tileMap[i][j].IsDoor = mapFormat[i][j] == doorIndex[k] ? true : false;
                            if (tileMap[i][j].IsDoor) {
                                break;
                            }
                        }
                        //assign doorpath if it's a door
                        if (tileMap[i][j].IsDoor) {
                            tileMap[i][j].DoorPath = nextMap[mapFormat[i][j]];
                        }
                        tileMap[i][j].WorldPosition = worldPosition;
                        tileMap[i][j].Scale = 1.0f;
                        //check if tile is walkable
                        foreach (int w in unwalkableTiles) {
                            if(mapFormat[i][j] == w) {
                                //txt file indexed unwalkable tiles
                                tileMap[i][j].Walkable = false;
                            }
                        }
                    }
                }
                //set hero position
                hero.Position.X = spawnTile.X * Game.TILE_SIZE;
                hero.Position.Y = spawnTile.Y * Game.TILE_SIZE;
#if DEBUG
                Console.WriteLine("Map has been loaded!");
#endif
            }
            else {
                Console.WriteLine("Map not found!");
            }
        }
        public void Update(float dTime) {
            //do update stuff in here
        }
        /*
        public Map ResolveDoors(PlayerCharacter hero) {
            resolve doors here
        }
        */
        public void Render(PointF offsetPosition, PointF cameraCenter) {
            int minX = (int)cameraCenter.X - 8 * Game.TILE_SIZE - Game.TILE_SIZE;
            int minY = (int)cameraCenter.Y - 4 * Game.TILE_SIZE - Game.TILE_SIZE;
            int maxX = (int)cameraCenter.X + 8 * Game.TILE_SIZE + Game.TILE_SIZE;
            int maxY = (int)cameraCenter.Y + 4 * Game.TILE_SIZE + Game.TILE_SIZE;
            minX /= Game.TILE_SIZE;
            minY /= Game.TILE_SIZE;
            maxX /= Game.TILE_SIZE;
            maxY /= Game.TILE_SIZE;
            for(int h = minY; h< maxY; h++) {
                for(int w = minX;w< maxX; w++) {
                    if (h < 0 || w < 0) {
                        continue;
                    }
                    if (h >= tileMap.Length || w >= tileMap[h].Length) {
                        continue;
                    }
                    tileMap[h][w].Render(offsetPosition);
                }
            }
            for (int i = 0; i < tileMap.Length; i++) {
                for (int j = 0; j < tileMap[i].Length; j++) {
                    tileMap[i][j].Render(offsetPosition);
                }
            }
            //render items
            //render enemies
        }
        public void Destroy() {
            for (int h = 0; h < tileMap.Length; h++) {
                for(int w = 0; w < tileMap[h].Length; w++) {
                    tileMap[h][w].Destroy();
                }
            }
            //destroy items
            //destroy enemies
        }
    }
}
