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
        public  Tile[][] tileMap = null;

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
        protected List<List<int>> mapFormat = null;
        protected Dictionary<int, Rectangle> spriteSources = null;
        protected Dictionary<int, int> breakableTiles = null;
        protected List<int> unwalkableTiles = null;
        protected List<EnemyCharacter> enemies = null;
        public static List<Item> items = null;

        public Map(string mapPath,PlayerCharacter hero) {
            if (System.IO.File.Exists(mapPath)) {
                unwalkableTiles = new List<int>();
                List<int> doorIndex = new List<int>();
                spriteSources = new Dictionary<int, Rectangle>();
                mapFormat = new List<List<int>>();
                Dictionary<int, string> nextMap = new Dictionary<int, string>();
                nextRoom = new Dictionary<string, Point>();
                breakableTiles = new Dictionary<int, int>();
                enemies = new List<EnemyCharacter>();
                items = new List<Item>();
                //load map
                using (TextReader reader = File.OpenText(mapPath)) {
                    string contents = reader.ReadLine();
                    while (contents != null) {
                        string[] content = contents.Split(',');
                        if (content.Length == 1) {
                            content = contents.Split(' ');
                        }

                        //Console.WriteLine(content[0]);

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
                            if (enemies == null) {
                                enemies = new List<EnemyCharacter>();
                            }
                            bool upDownMove = content[2] == "X" ? false : true;
                            enemies.Add(new EnemyCharacter(content[1],upDownMove));
                            enemies[enemies.Count-1].Position.X = System.Convert.ToInt32(content[3])*Game.TILE_SIZE;
                            enemies[enemies.Count-1].Position.Y = System.Convert.ToInt32(content[4])*Game.TILE_SIZE;
#if DEBUG
                            Console.WriteLine("Enemy added, Y Axis Movement: " + upDownMove);
                            Console.WriteLine("Enemy sprite path: "+content[1]);
                            Console.WriteLine("Enemy starting location: " + content[3]+", " + content[4]);
#endif
                            
                        }
                        else if(content[0] == "//" || content[0] == " ") {
                            //used to make comments in txt file!
                        }
                        //which tiles are breakable
                        else if (content[0] == "B") {
                            Console.WriteLine("Breakable dict length: " + (content.Length-1));
                            for (int i = 1; i < (content.Length-1)/2+1; i++) {
                                breakableTiles.Add(System.Convert.ToInt32(content[i]), System.Convert.ToInt32(content[content.Length-i]));
                                //Console.WriteLine("Breakable tile: " + System.Convert.ToInt32(content[i]) + " turns into: " + System.Convert.ToInt32(content[content.Length - i]));
                                //Console.WriteLine("i: " + i + ", content length-i: " + (content[content.Length - i]));
                            }
                        }
                        //add items to map
                        else if(content[0] == "I") {
                            Item.ItemSheet = content[1];
#if DEBUG
                            Console.WriteLine("Item sheet: "+content[1]);
#endif
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
                        //assign tile values
                        tileMap[i][j].TileValue = mapFormat[i][j];
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
                        //breakable?
                        for (int k = 0; k < breakableTiles.Count; k++) {
                            int b = tileMap[i][j].TileValue;
                            //check to see if the tile value is in the list of breakable values
                            tileMap[i][j].Breakable = breakableTiles.ContainsKey(b);
                            
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
                        //assign items to tile
                        if (tileMap[i][j].TileValue == 33) {
                            tileMap[i][j].Item = "GrowMushroom";
                        }
                        /*
                        else if (tileMap[i][j].TileValue == 34) {
                            tileMap[i][j].Item = "Star";
                        }
                        else if (tileMap[i][j].TileValue == 35) {
                            tileMap[i][j].Item = "OneUp";
                        }
                        else if (tileMap[i][j].TileValue == 36) {
                            tileMap[i][j].Item = "Coin";
                        }
                        */
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
        public void Update(float dTime,PlayerCharacter hero) {
            //do update stuff in here
            for (int i = enemies.Count - 1; i >= 0; i--) {
                if (enemies[i].IsSeen) {
                    enemies[i].Update(dTime);
                }
                Rectangle intersection = Intersections.Rect(hero.Rect, enemies[i].Rect);
                if (intersection.Bottom == hero.Rect.Bottom && intersection.Top == enemies[i].Rect.Top && (intersection.Bottom-intersection.Top) < (enemies[i].Rect.Height/2)) {
                    enemies[i].Destroy();
                    enemies.RemoveAt(i);
                }
                else if (intersection.Height*intersection.Width > 0) {
                    Console.WriteLine("Collision with enemy!");
                    //game over
                }

            }
        }
        /*
        public Map ResolveDoors(PlayerCharacter hero) {
            resolve doors here
        }
        */
        public void Render(PointF offsetPosition,PointF cameraCenter,PlayerCharacter hero) {
            int minX = (int)cameraCenter.X - 16 * Game.TILE_SIZE-Game.TILE_SIZE;
            int maxX = (int)cameraCenter.X + 16 * Game.TILE_SIZE+Game.TILE_SIZE;
            int minY = (int)cameraCenter.Y - 9 * Game.TILE_SIZE - Game.TILE_SIZE;
            int maxY = (int)cameraCenter.Y + 13 * Game.TILE_SIZE + Game.TILE_SIZE;
            minX /= Game.TILE_SIZE;
            maxX /= Game.TILE_SIZE;
            minY /= Game.TILE_SIZE;
            maxY /= Game.TILE_SIZE;
            for (int h = minY; h < maxY; h++) {
                for (int w = minX;w< maxX; w++) {
                    if (h < 0 || w < 0) {
                        continue;
                    }
                    if (h >= tileMap.Length || w >= tileMap[h].Length) {
                        continue;
                    }
                    tileMap[h][w].Render(offsetPosition);
                    
                }
            }
            for (int i = enemies.Count - 1; i >= 0; i--) { 
                if (Math.Abs(enemies[i].Position.X - hero.Position.X) < Program.Window.Width / 2) {
                    enemies[i].IsSeen = true;
                }
                enemies[i].Render(offsetPosition);
            }
            //render items
            for (int i = 0; i < items.Count; i++) {
                items[i].Render(offsetPosition);
            }
        }
        public void Destroy() {
            //destroy map
            for (int h = 0; h < tileMap.Length; h++) {
                for(int w = 0; w < tileMap[h].Length; w++) {
                    tileMap[h][w].Destroy();
                }
            }
            //destroy items
            for (int i = items.Count - 1; i >= 0; i--) {
                items[i].Destroy();
            }
            //destroy enemies
            for (int i = enemies.Count - 1; i >= 0; i--) {
                enemies[i].Destroy();
            }
        }
        public void ChangeTile(PointF location) {
            //new value is used to find source rect for textures only
            int yTile = (int)location.Y / Game.TILE_SIZE;
            int xTile = (int)location.X / Game.TILE_SIZE;
            int xPos = ((int)location.X / Game.TILE_SIZE) * Game.TILE_SIZE;
            int yPos = ((int)location.Y / Game.TILE_SIZE) * Game.TILE_SIZE;
            //new value holds the tile value of what it turns into
            int oldValue = tileMap[yTile][xTile].TileValue;
            Console.WriteLine("Tile old value: " + oldValue);
            tileMap[yTile][xTile].Destroy();

            tileMap[yTile][xTile] = new Tile(tileSheet, spriteSources[breakableTiles[oldValue]]);
            Console.WriteLine("Source rect: " + spriteSources[breakableTiles[oldValue]]);

            tileMap[yTile][xTile].TileValue = breakableTiles[oldValue];
            mapFormat[yTile][xTile] = tileMap[yTile][xTile].TileValue;
            Console.WriteLine("Tile new value: " + tileMap[yTile][xTile].TileValue);
            tileMap[yTile][xTile].Walkable = true;
            foreach (int w in unwalkableTiles) {
                if (mapFormat[yTile][xTile] == w) {
                    tileMap[yTile][xTile].Walkable = false;
                }
            }
            for (int k = 0; k < breakableTiles.Count; k++) {
                int b = tileMap[yTile][xTile].TileValue;
                tileMap[yTile][xTile].Breakable = breakableTiles.ContainsKey(b);

            }
            Console.WriteLine("Tile Location PreAdjustment, X: " + tileMap[yTile][xTile].WorldPosition.X + " , Y: " + tileMap[yTile][xTile].WorldPosition.Y);
            tileMap[yTile][xTile].WorldPosition = new Point(xPos, yPos);
            Console.WriteLine("Tile Location PostAdjustment, X: " + tileMap[yTile][xTile].WorldPosition.X + " , Y: " + tileMap[yTile][xTile].WorldPosition.Y);
            tileMap[yTile][xTile].Scale = 1.0f;
        }
    }
}
