using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using System.Drawing;

namespace MarioWorld1_1 {
    class Game {
        //map state
        public enum State { Start, Play, Dying, Win, Won}
        public State CurrentState = State.Start;
        //meta data
        public static readonly int TILE_SIZE = 16;
        public static Map currentMap = null;
        protected string startingMap = "Assets/world1-1.txt";

        PointF offsetPosition = new PointF();
        public bool GameOver = false;
        //hero stuff
        protected PlayerCharacter hero = null;
        protected string heroSheet = "Assets/Mario.png";
        protected float deathTimer = 0.0f;

        //sounds
        public Dictionary<string, int> SoundBank = null;

        //rows before columns, map[y][x]
        public Tile GetTile(PointF pixelPoint) {
            Tile result = currentMap[(int)pixelPoint.Y / TILE_SIZE][(int)pixelPoint.X / TILE_SIZE];
            if ((int)pixelPoint.Y / TILE_SIZE > currentMap.Length-1) {
                return null;
            }
            return result;
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
            //load hero
            hero = new PlayerCharacter(heroSheet);
            //load map
            currentMap = new Map(startingMap,hero);
            //load sounds
            SoundBank = new Dictionary<string, int>();
            SoundBank.Add("Coin", SoundManager.Instance.LoadWav("Assets/coin.mp3"));
            SoundBank.Add("BreakBlock", SoundManager.Instance.LoadWav("Assets/breakblock.mp3"));
            SoundBank.Add("HeroDeath", SoundManager.Instance.LoadWav("Assets/mariodie.mp3"));
            SoundBank.Add("HeroJump", SoundManager.Instance.LoadWav("Assets/jump.mp3"));
            SoundBank.Add("OneUp", SoundManager.Instance.LoadWav("Assets/1up.mp3"));
            SoundBank.Add("Projectile", SoundManager.Instance.LoadWav("Assets/fireball.mp3"));
            SoundBank.Add("Grow", SoundManager.Instance.LoadWav("Assets/grow.mp3"));
            SoundBank.Add("Win", SoundManager.Instance.LoadWav("Assets/win.mp3"));
            SoundBank.Add("ItemSpawn", SoundManager.Instance.LoadWav("Assets/itemspawn.mp3"));
            SoundBank.Add("Stomp", SoundManager.Instance.LoadWav("Assets/stomp.mp3"));
            SoundBank.Add("Background", SoundManager.Instance.LoadMp3("Assets/background.mp3"));
            SoundBank.Add("DudBrick", SoundManager.Instance.LoadWav("Assets/dudbrick.wav"));
            SoundBank.Add("CourseClear", SoundManager.Instance.LoadWav("Assets/CourseClear.wav"));
            SoundBank.Add("Invincible", SoundManager.Instance.LoadMp3("Assets/invincible.mp3"));
    }
        public void Update(float dt) {
            //currentMap = currentMap.ResolveDoors(hero);
            if (CurrentState == State.Start) {
                hero.SetSprite("Stand");
                hero.Position = new Point(currentMap.SpawnTile.X * Game.TILE_SIZE, currentMap.SpawnTile.Y * Game.TILE_SIZE);
                if (InputManager.Instance.KeyPressed(OpenTK.Input.Key.Space)) {
                    CurrentState = State.Play;
                }
            }
            else if (CurrentState == State.Play) {
                currentMap.Update(dt, hero);
                hero.Update(1 / 30.0f);
                if (hero.CurrentState == PlayerCharacter.State.Invincible) {
                    if (SoundManager.Instance.IsPlaying(SoundBank["Background"])) {
                        SoundManager.Instance.StopSound(SoundBank["Background"]);
                    }
                    if (!SoundManager.Instance.IsPlaying(SoundBank["Invincible"])) {
                        SoundManager.Instance.PlaySound(SoundBank["Invincible"]);
                    }
                }
                else {
                    if (SoundManager.Instance.IsPlaying(SoundBank["Invincible"])) {
                        SoundManager.Instance.StopSound(SoundBank["Invincible"]);
                    }
                    if (!SoundManager.Instance.IsPlaying(SoundBank["Background"])) {
                        SoundManager.Instance.PlaySound(SoundBank["Background"]);
                    }
                }
            }
            else if (CurrentState == State.Dying) {
                SoundManager.Instance.StopSound(SoundBank["Background"]);
                deathTimer += dt;
                hero.Update(1 / 30.0f);
                hero.Die(dt);
            }
            else if (CurrentState == State.Win) {
                if (SoundManager.Instance.IsPlaying(SoundBank["Background"])) {
                    SoundManager.Instance.StopSound(SoundBank["Background"]);
                }
                if (!SoundManager.Instance.IsPlaying(SoundBank["CourseClear"])) {
                    SoundManager.Instance.PlaySound(SoundBank["CourseClear"]);
                }
                hero.Win(dt);
                currentMap.Update(dt, hero);
            }
            else if (CurrentState == State.Won) {
                if (InputManager.Instance.KeyPressed(OpenTK.Input.Key.Space)) {
                    Initialize();
                    CurrentState = State.Start;
                }
            }
        }
        public void Render() {
            Size windowSize = new Size(Program.Window.Size.Width,Program.Window.Height);
            if (CurrentState == State.Start) {
                GraphicsManager.Instance.DrawRect(new Rectangle(new Point(0, 0), windowSize),Color.Black);
                GraphicsManager.Instance.DrawString("Press Space to Begin!", new Point(windowSize.Width / 2 - 100,windowSize.Height-40), Color.White);
                GraphicsManager.Instance.DrawString("Created by: Martin Szauer", new Point(15, windowSize.Height - 20),Color.White);
                GraphicsManager.Instance.DrawString("Lives:" + hero.Lifes, new Point(windowSize.Width /2 - 49, windowSize.Height / 2 + 1), Color.Black);
                GraphicsManager.Instance.DrawString("Lives:" + hero.Lifes, new Point(windowSize.Width /2 - 48, windowSize.Height / 2), Color.White);
            }
            else if (CurrentState == State.Play || CurrentState == State.Dying || CurrentState == State.Win) {
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
            else if (CurrentState == State.Won) {
                GraphicsManager.Instance.DrawRect(new Rectangle(new Point(0, 0), windowSize), Color.Black);
                GraphicsManager.Instance.DrawString("Press Space to Replay!", new Point(windowSize.Width / 2 - 100, windowSize.Height - 40), Color.White);
                GraphicsManager.Instance.DrawString("Created by: Martin Szauer", new Point(15, windowSize.Height - 20), Color.White);
                GraphicsManager.Instance.DrawString("You win!", new Point(windowSize.Width / 2 - 49, windowSize.Height / 2 + 1), Color.Black);
                GraphicsManager.Instance.DrawString("You win!", new Point(windowSize.Width / 2 - 48, windowSize.Height / 2), Color.White);
            }
            //HUD
            //score
            GraphicsManager.Instance.DrawString("Mario", new Point(5 + 1, 6), Color.Black);
            GraphicsManager.Instance.DrawString("Mario", new Point(5, 5), Color.White);
            GraphicsManager.Instance.DrawString(System.Convert.ToString(currentMap.Score), new Point(6 + 1, 20), Color.Black);
            GraphicsManager.Instance.DrawString(System.Convert.ToString(currentMap.Score), new Point(5, 19), Color.White);
            //world
            GraphicsManager.Instance.DrawString("World", new Point(2*(windowSize.Width / 4)+1, 6), Color.Black);
            GraphicsManager.Instance.DrawString("World", new Point(2*(windowSize.Width / 4), 5), Color.White);
            GraphicsManager.Instance.DrawString("1-1", new Point(2 * (windowSize.Width / 4) + 10, 20), Color.Black);
            GraphicsManager.Instance.DrawString("1-1", new Point(2 * (windowSize.Width / 4) + 9, 19), Color.White);
            //fps - done in program.cs
           //time
            GraphicsManager.Instance.DrawString("Time", new Point(3 * (windowSize.Width / 4) + 1, 6), Color.Black);
            GraphicsManager.Instance.DrawString("Time", new Point(3 * (windowSize.Width / 4), 5), Color.White);
            GraphicsManager.Instance.DrawString(System.Convert.ToString((int)currentMap.Timer), new Point(3 * (windowSize.Width / 4) + 10, 20), Color.Black);
            GraphicsManager.Instance.DrawString(System.Convert.ToString((int)currentMap.Timer), new Point(3 * (windowSize.Width / 4) + 9, 19), Color.White);

        }
        public void Shutdown() {
            currentMap.Destroy();
            hero.Destroy();
            //get rid of the sounds
            foreach (KeyValuePair<string,int> sound in SoundBank) {
                SoundManager.Instance.UnloadSound(sound.Value);
                SoundBank.Remove(sound.Key);
            }
        }
        public void Reset() {
            currentMap.Destroy();
            hero.Destroy();
            Initialize();
        }
    }
}
