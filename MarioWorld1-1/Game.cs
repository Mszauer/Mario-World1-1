using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using System.Drawing;

namespace MarioWorld1_1 {
    class Game {
        public enum State { Start, Play }
        public State CurrentState = State.Start;
        public static readonly int TILE_SIZE = 16;
        public static Map currentMap = null;
        PointF offsetPosition = new PointF();
        public bool GameOver = false;
        protected PlayerCharacter hero = null;
        protected string heroSheet = "Assets/Mario.png";

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
            hero = new PlayerCharacter(heroSheet);
            currentMap = new Map(startingMap,hero);
        }
        public void Update(float dt) {
            //currentMap = currentMap.ResolveDoors(hero);
            if (CurrentState == State.Start) {

            }
            else if (CurrentState == State.Play) {
                currentMap.Update(dt, hero);
                hero.Update(1 / 30.0f);
            }

        }
        public void Render() {
            Size windowSize = new Size(Program.Window.Size.Width,Program.Window.Height);
            if (CurrentState == State.Start) {
                GraphicsManager.Instance.DrawRect(new Rectangle(new Point(0, 0), windowSize),Color.Black);
                GraphicsManager.Instance.DrawString("Mario, World 1-1", new Point((windowSize.Width /2) - 85, windowSize.Height / 2-20),Color.White);
                GraphicsManager.Instance.DrawString("Press Space to Begin!", new Point(windowSize.Width / 2 - 100,windowSize.Height/2+30), Color.White);
                GraphicsManager.Instance.DrawString("Created by: Martin Szauer", new Point(15, windowSize.Height - 20),Color.White);
                if (InputManager.Instance.KeyPressed(OpenTK.Input.Key.Space)) {
                    CurrentState = State.Play;
                }
            }
            else if (CurrentState == State.Play) {
                PointF offsetPosition = new PointF();
                offsetPosition.X = hero.Position.X - (float)(7.625 * TILE_SIZE);//7.625 == half of map displayed
                if (hero.Position.X < 7.625 * TILE_SIZE) {
                    offsetPosition.X = 0;
                }
                if (hero.Position.X > (currentMap[0].Length - 7.625) * TILE_SIZE) {
                    offsetPosition.X = (currentMap.Length - 14) * TILE_SIZE;
                }
                currentMap.Render(offsetPosition,hero.Center,hero);
                hero.Render(new PointF(offsetPosition.X,offsetPosition.Y-1));
            }
            GraphicsManager.Instance.DrawString("Lives:" + hero.Lifes, new Point(windowSize.Width - 80, 4), Color.White);
            GraphicsManager.Instance.DrawString("Lives:" + hero.Lifes, new Point(windowSize.Width - 81, 5), Color.Black);

        }
        public void Shutdown() {
            currentMap.Destroy();
            hero.Destroy();
        }
    }
}
