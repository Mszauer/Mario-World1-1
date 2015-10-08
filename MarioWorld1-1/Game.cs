using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using System.Drawing;

namespace MarioWorld1_1 {
    class Game {
        public static readonly int TILE_SIZE = 16;
        protected Map currentMap = null;
        PointF offsetPosition = new PointF();
        public bool GameOver = false;

        protected string startingMap = "Assets/world1-1.txt";
        
        //rows before columns, map[y][x]
        public Tile GetTile(PointF pixelPoint) {
            return currentMap[(int)pixelPoint.Y / TILE_SIZE][(int)pixelPoint.X / TILE_SIZE];
        }
        public Rectangle GetTileRect(PointF pixelPoint) {
            int xTile = (int)pixelPoint.X / TILE_SIZE; //integer math
            int yTile = (int)pixelPoint.Y / TILE_SIZE;
            Rectangle result = new Rectangle(xTile * TILE_SIZE, yTile * TILE_SIZE, TILE_SIZE, TILE_SIZE);
            return result;
        }
        protected static Game instance = null;
        public static Game Instance {
            get {
                if (instance == null) {
                    instance = new Game();
                }
                return instance;
            }
        }

        protected Game() {

        }
        public void Initialize() {
            TextureManager.Instance.UseNearestFiltering = true;
            currentMap = new Map(startingMap);
        }
        public void Update(float dt) {
            //currentMap = currentMap.ResolveDoors(hero);
            //currentMap.Update(dt, hero, projectiles);
        }
        public void Render() {
            //add offsetPosition once hero gets added and accompanying
            //logic with it
            currentMap.Render(new PointF(0,0),new PointF(0,0));
        }
        public void Shutdown() {
            currentMap.Destroy();
        }
    }
}
