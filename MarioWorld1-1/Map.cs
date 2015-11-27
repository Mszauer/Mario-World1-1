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
        public Tile[][] tileMap = null;
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
        public Point SpawnTile = new Point(0, 0);
        protected string tileSheet = null;
        protected Dictionary<string, Point> nextRoom = null;
        protected List<List<int>> mapFormat = null;
        protected Dictionary<int, Rectangle> spriteSources = null;
        protected Dictionary<int, int> breakableTiles = null;
        protected List<int> unwalkableTiles = null;
        protected List<EnemyCharacter> enemies = null;
        public static List<Item> items = null;
        public float Timer = 0;
        public int Score = 0;

        public Map(string mapPath, PlayerCharacter hero) {
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
                            SpawnTile = new Point(System.Convert.ToInt32(content[1]), System.Convert.ToInt32(content[2]));
#if DEBUG
                            Console.WriteLine("Starting tile: " + content[1] + ", " + content[2]);
#endif
                        }
                        //add enemies
                        else if (content[0] == "E") {
                            if (enemies == null) {
                                enemies = new List<EnemyCharacter>();
                            }
                            bool upDownMove = content[3] == "X" ? false : true;
                            if (content[1] == "G") {
                                enemies.Add(new Goomba(content[2], upDownMove));
                            }
                            else if (content[1] == "K") {
                                enemies.Add(new Koopa(content[2], upDownMove));
                            }
                            enemies[enemies.Count - 1].Position.X = System.Convert.ToInt32(content[4]) * Game.TILE_SIZE;
                            enemies[enemies.Count - 1].Position.Y = System.Convert.ToInt32(content[5]) * Game.TILE_SIZE;
                            if (content[6] == "R") {
                                enemies[enemies.Count - 1].Direction = 1;
                            }
                            else if (content[6] == "L") {
                                enemies[enemies.Count - 1].Direction = -1;
                            }
#if DEBUG
                            Console.WriteLine("Enemy added, Y Axis Movement: " + upDownMove);
                            Console.WriteLine("Enemy type: " + content[1]);
                            Console.WriteLine("Enemy sprite path: " + content[2]);
                            Console.WriteLine("Enemy starting location: " + content[4] + ", " + content[5]);
#endif

                        }
                        else if (content[0] == "//" || content[0] == " ") {
                            //used to make comments in txt file!
                        }
                        //which tiles are breakable
                        else if (content[0] == "B") {
                            Console.WriteLine("Breakable dict length: " + (content.Length - 1));
                            for (int i = 1; i < (content.Length - 1) / 2 + 1; i++) {
                                breakableTiles.Add(System.Convert.ToInt32(content[i]), System.Convert.ToInt32(content[content.Length - i]));
                                //Console.WriteLine("Breakable tile: " + System.Convert.ToInt32(content[i]) + " turns into: " + System.Convert.ToInt32(content[content.Length - i]));
                                //Console.WriteLine("i: " + i + ", content length-i: " + (content[content.Length - i]));
                            }
                        }
                        //add items to map
                        else if (content[0] == "I") {
                            Item.ItemSheet = content[1];
#if DEBUG
                            Console.WriteLine("Item sheet: " + content[1]);
#endif
                        }
                        //load rows
                        else if (System.Convert.ToInt32(content[0]) >= 0) {
                            //create new row
                            mapFormat.Add(new List<int>());
                            for (int i = 0; i < content.Length; i++) {
                                //add numbers to row
                                if (string.IsNullOrEmpty(content[i])) {
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
                            if (mapFormat[i][j] == w) {
                                //txt file indexed unwalkable tiles
                                tileMap[i][j].Walkable = false;
                            }
                        }
                        //assign items to tile
                        if (tileMap[i][j].TileValue == 33) {
                            tileMap[i][j].Item = "GrowMushroom";
                        }
                        else if (tileMap[i][j].TileValue == 34) {
                            tileMap[i][j].Item = "Star";
                        }
                        else if (tileMap[i][j].TileValue == 35) {
                            tileMap[i][j].Item = "OneUp";
                        }
                        else if (tileMap[i][j].TileValue == 36) {
                            tileMap[i][j].Item = "Coin";
                        }
                        else if (tileMap[i][j].TileValue == 37) {
                            tileMap[i][j].Item = "FireFlower";
                        }/*
                        
                        */
                    }
                }
                //set hero position
                hero.Position.X = SpawnTile.X * Game.TILE_SIZE;
                hero.Position.Y = SpawnTile.Y * Game.TILE_SIZE;

#if DEBUG
                Console.WriteLine("Map has been loaded!");
#endif
            }
            else {
                Console.WriteLine("Map not found!");
            }
        }
        public void Update(float dTime, PlayerCharacter hero) {
                //do update stuff in here
                Timer += dTime;
                //hero update/logic
                if ((hero.Position.Y + hero.Rect.Height) / Game.TILE_SIZE > tileMap.Length) {
                    //lose a life
                    hero.Lifes -= 1;
                    //start over
                    Game.Instance.CurrentState = Game.State.Start;
                }
#if DEBUG
                if (InputManager.Instance.KeyPressed(OpenTK.Input.Key.Number1)) {
                    hero.Position.X = 3 * Game.TILE_SIZE;
                }
                if (InputManager.Instance.KeyPressed(OpenTK.Input.Key.Number2)) {
                    hero.Position.X = 100 * Game.TILE_SIZE;
                }
                if (InputManager.Instance.KeyPressed(OpenTK.Input.Key.Number3)) {
                    hero.Position.X = 195 * Game.TILE_SIZE;
                }
#endif
                //enemy update/logic
                for (int k = enemies.Count - 1; k >= 0; k--) {
                    //has hero encountered enemy?
                    if (enemies[k].IsSeen) {
                        //update once seen
                        enemies[k].Update(dTime);
                        //Collide with a dead koopa shell
                        //if enemy is koopa, check collision with other enemies (double loop)
                        if (enemies[k] is Koopa) {
                            //loop through all enemies
                            for (int g = enemies.Count - 1; g >= 0; g--) {
                                //if update and run into a koopa
                                if (enemies[g] is Goomba && enemies[k].CurrentState == EnemyCharacter.State.Dead1) {
                                    //collision rect
                                    Rectangle collision = Intersections.Rect(enemies[k].Rect, enemies[g].Rect);
                                    //is there collision?
                                    if (collision.Left == enemies[k].Rect.Left) {
                                        //swap enemy collision
                                        enemies[g].Direction *= -1;
                                        enemies[g].Position.X = collision.Left;
                                    }
                                    else if (collision.Right == enemies[k].Rect.Right) {
                                        enemies[g].Direction *= -1;
                                        enemies[g].Position.X = collision.Right;
                                    }
                                }
                            }
                        }
                    }
                    //Killed by hero
                    Rectangle intersection = Intersections.Rect(hero.Rect, enemies[k].Rect);
                    if (intersection.Bottom == hero.Rect.Bottom && intersection.Top == enemies[k].Rect.Top && (intersection.Bottom - intersection.Top) < (enemies[k].Rect.Height / 2)) {
                        hero.Jump(hero.Impulse * 0.5f);
                        //if koopa, change into appropriate state
                        if (enemies[k] is Koopa) {
                            if (enemies[k].CurrentState == EnemyCharacter.State.Alive) {
                                Console.WriteLine("Pre koopa position: X: " + enemies[k].Position.X + ", Y: " + enemies[k].Position.Y);
                                enemies[k].CurrentState = EnemyCharacter.State.Dead1;
                                //add score
                                Score += 100;
#if DEBUG
                                Console.WriteLine("koopa state: " + enemies[k].CurrentState);
                                Console.WriteLine("Post koopa position: X: " + enemies[k].Position.X + ", Y: " + enemies[k].Position.Y);
#endif
                                continue;
                            }
                            else if (enemies[k].CurrentState == EnemyCharacter.State.Dead1) {
                                Console.WriteLine("Pre koopa position: X: " + enemies[k].Position.X + ", Y: " + enemies[k].Position.Y);
                                enemies[k].CurrentState = EnemyCharacter.State.Dead2;
#if DEBUG
                                Console.WriteLine("koopa state: " + enemies[k].CurrentState);
                                Console.WriteLine("Post koopa position: X: " + enemies[k].Position.X + ", Y: " + enemies[k].Position.Y);
#endif
                                //koopa direction logic (if hero.center > koopa.Center, go right otherwise go left)
                                enemies[k].Direction = (hero.Rect.X + (hero.Rect.Width / 2) > (enemies[k].Rect.X + (enemies[k].Rect.Width / 2))) ? -1 : 1;//1 = left, -1 = right
                                continue;
                            }
                            else if (enemies[k].CurrentState == EnemyCharacter.State.Dead2) {
                                Console.WriteLine("Pre koopa position: X: " + enemies[k].Position.X + ", Y: " + enemies[k].Position.Y);

                                enemies[k].CurrentState = EnemyCharacter.State.Dead1;
#if DEBUG
                                Console.WriteLine("koopa state: " + enemies[k].CurrentState);
                                Console.WriteLine("Post koopa position: X: " + enemies[k].Position.X + ", Y: " + enemies[k].Position.Y);
#endif
                                continue;
                            }
                        }
                        else {
#if DEBUG
                            Console.WriteLine("Enemy Removed: " + enemies[k]);
#endif
                            enemies[k].Destroy();
                            enemies.RemoveAt(k);
                        }
                        //add score
                        Score += 100;
                    }
                    //killed by hero that is invincible
                    else if (intersection.Height * intersection.Width > 0 && hero.CurrentState == PlayerCharacter.State.Invincible) {
                        //koopa logic
                        if (enemies[k] is Koopa) {
                            //koopa shell logic
                            if (enemies[k].CurrentState == EnemyCharacter.State.Dead1) {
                                enemies[k].CurrentState = EnemyCharacter.State.Dead2;
                            }
                            else if (enemies[k].CurrentState == EnemyCharacter.State.Dead2) {
                                enemies[k].CurrentState = EnemyCharacter.State.Dead1;
                            }
                        }
                        //all other enemies
                        else {
                            enemies[k].Destroy();
                            enemies.RemoveAt(k);
                        }
                        //add score
                        Score += 100;
                    }
                    //hero killed by enemy
                    else if (intersection.Height * intersection.Width > 0) {
                        Console.WriteLine("Collision with enemy!");
                        //subtract lifes
                        hero.Lifes -= 1;
                        if (hero.Lifes < 0) {
                        }
                        else {
                            Game.Instance.CurrentState = Game.State.Dying;
                        }
                    }
                    //enemy off map, X axis
                    if (enemies[k].Position.X / Game.TILE_SIZE < 0 || enemies[k].Position.X / Game.TILE_SIZE > tileMap[(int)enemies[k].Position.Y / Game.TILE_SIZE].Length) {
                        enemies[k].Destroy();
                        enemies.RemoveAt(k);
                    }
                    //Enemy off map, Y axis
                    else if (enemies[k].Position.Y / Game.TILE_SIZE < 0 || (enemies[k].Position.Y + enemies[k].Rect.Height) / Game.TILE_SIZE >= tileMap.Length - 1) {
                        enemies[k].Destroy();
                        enemies.RemoveAt(k);
                    }
                }
                //items update/logic
                for (int i = items.Count - 1; i >= 0; i--) {

                    //update items, if spawned
                    if (items[i].IsSpawned) {
                        items[i].Update(dTime);
                    }
                    //hero picked up item
                    Rectangle intersection = Intersections.Rect(hero.Rect, items[i].Rect);
                    if (intersection.Height * intersection.Width > 0) {
                        //update items
                        items[i].Update(dTime);

                        //mushroom logic
                        if (items[i] is GrowMushroom) {
                            if (!hero.Large) {
                                hero.ChangeForm("Large");
                                hero.CurrentSprite = "LargeStand";
                                hero.Position.Y -= Game.TILE_SIZE;
                            }
                        }
                        //fireflower logic
                        else if (items[i] is FireFlower) {
                            if (hero.CurrentState != PlayerCharacter.State.Fire) {
                                hero.ChangeForm("Fire");
                                hero.CurrentSprite = "FireStand";
                                if (hero.Large) {
                                    hero.CurrentSprite = "LargeFireStand";
                                    hero.Position.Y -= Game.TILE_SIZE;
                                }
                            }
                        }
                        //star logic
                        else if (items[i] is Star) {
                            if (hero.CurrentState != PlayerCharacter.State.Invincible) {
                                hero.ChangeForm("Invincible");
                                hero.CurrentSprite = "InvincibleStand";
                                if (hero.Large) {
                                    hero.CurrentSprite = "InvincibleLargeStand";
                                    hero.Position.Y -= Game.TILE_SIZE;
                                }
                            }
                        }
                        //one up logic
                        else if (items[i] is OneUp) {
                            hero.Lifes += 1;
#if DEBUG
                            Console.WriteLine("Added extra life: " + hero.Lifes);
#endif
                        }
                        //add points
                        Score += 1000;
                        //destroy/remove item
                        items[i].Destroy();
                        items.RemoveAt(i);
                        continue;
                    }
                    //mushroom going off map
                    if (items.Count > 0 && ((items[i] is GrowMushroom) || (items[i] is OneUp))) {
                        //item off map, x axis
                        if (items[i].Position.X / Game.TILE_SIZE < 0 || items[i].Position.X / Game.TILE_SIZE > tileMap[(int)items[i].Position.Y / Game.TILE_SIZE].Length) {
                            items[i].Destroy();
                            items.RemoveAt(i);
                            continue;
                        }
                        //item off map, y axis
                        else if (items[i].Position.Y / Game.TILE_SIZE < 0 || (items[i].Position.Y + items[i].Rect.Height) / Game.TILE_SIZE > tileMap.Length - 1) {
                            items[i].Destroy();
                            items.RemoveAt(i);
                            continue;
                        }
                    }
                    //coin life span
                    if (items.Count > 0 && (items[i] is Coin)) {
                        items[i].TimeAlive += dTime;
                        if (items[i].TimeAlive > 0.5f) {
                            items[i].TimeAlive -= 0.5f;
                            items[i].Destroy();
                            items.RemoveAt(i);
                        }
                    }
                }
                //projectiles update
                if (hero.Projectiles != null) {
                    //PlayerCharacter controlls creation and update logic
                    for (int i = hero.Projectiles.Count - 1; i >= 0; i--) {
                        int yPos = (int)hero.Projectiles[i].Position.Y / Game.TILE_SIZE;
                        int xPos = (int)hero.Projectiles[i].Position.X / Game.TILE_SIZE;
                        if (hero.Projectiles[i].ToDestroy) {
                            hero.Projectiles.RemoveAt(i);
                            continue;
                        }
                        //out of bounds on the y
                        if (yPos > tileMap.Length - 1 || yPos < 0) {
                            hero.Projectiles.RemoveAt(i);
                            continue;
                        }
                        //out of bounds on the x
                        if (xPos < 0 || xPos > tileMap[yPos].Length - 1) {
                            hero.Projectiles.RemoveAt(i);
                            continue;
                        }

                        //collision with enemies
                        for (int j = enemies.Count - 1; j >= 0; j--) {
                            Rectangle intersection = Intersections.Rect(hero.Projectiles[i].Rect, enemies[j].Rect);
                            if (intersection.Width * intersection.Height > 0) {
                                if (enemies[i] is Koopa) {
                                    if (enemies[i].CurrentState == EnemyCharacter.State.Dead1) {
                                        enemies[i].CurrentState = EnemyCharacter.State.Dead2;
                                    }
                                    else if (enemies[i].CurrentState == EnemyCharacter.State.Dead2) {
                                        enemies[i].CurrentState = EnemyCharacter.State.Dead1;
                                    }
                                }
                                hero.Projectiles.RemoveAt(i);
                                enemies[j].Destroy();
                                enemies.RemoveAt(j);
                                break;
                            }
                    }
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
            //add score
            Score += 50;
        }
    }
}
