using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using System.Drawing;

namespace MarioWorld1_1 {
    class PlayerCharacter : Character{
        public float speed = 3*Game.TILE_SIZE;
        protected float impulse = 0.0f;
        protected float velocity = 0.0f;
        protected float gravity = 0f;
        private bool isJumping = false;
        public PlayerCharacter(string spritePath) : base(spritePath) {
            AddSprite("Stand", new Rectangle(12, 6, 16, 16));
            AddSprite("Run", new Rectangle(30, 27, 16, 16), new Rectangle(47, 27, 16, 16), new Rectangle(64, 27, 16, 16));
            AddSprite("Jump", new Rectangle(29, 6, 16, 16));
            SetSprite("Stand");
            SetJump(/*3.50f*/4 * Game.TILE_SIZE, 0.75f);
        }
        public void Update(float dTime) {
            InputManager i = InputManager.Instance;
            
            //move left
            if (i.KeyDown(OpenTK.Input.Key.Left)|| i.KeyDown(OpenTK.Input.Key.A)) {
                if (velocity == gravity) {
                    faceLeft = true;
                    SetSprite("Run");
                    Animate(dTime);
                }
                Position.X -= speed * dTime;
                if (!Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_LEFT]));
                    if (intersection.Width * intersection.Height>0) {
                        Position.X = intersection.Right;
                    }
                }
                if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Position.X = intersection.Right;
                    }
                }
                //map boundry check
                if (Position.X < 0) {
                    Position.X = 0;
                }
            }
            //move right
            if (i.KeyDown(OpenTK.Input.Key.Right)|| i.KeyDown(OpenTK.Input.Key.D)) {
                if (velocity == gravity) {
                    faceLeft = false;
                    SetSprite("Run");
                    Animate(dTime);
                }
                Position.X += speed * dTime;
                if (!Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_RIGHT]));
                    if (intersection.Width*intersection.Height > 0) {
                        Position.X = intersection.Left - Rect.Width;
                    }
                }
                if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_RIGHT]));
                    if (intersection.Width*intersection.Height > 0) {
                        Position.X = intersection.Left - Rect.Width;
                    }
                }
                //map boundry check
                if (Position.X > 206f * Game.TILE_SIZE) {
                    Position.X = 206f * Game.TILE_SIZE;
                }
            }
            //jump!
            if (!isJumping) {
                isJumping = true;
                if (i.KeyDown(OpenTK.Input.Key.W) || i.KeyDown(OpenTK.Input.Key.Up)||i.KeyDown(OpenTK.Input.Key.Space)) {
                    velocity = impulse;
                    SetSprite("Jump");
                }
            }
            //S/Down = special case tile / go down pipe
            if (velocity != gravity) {
                //animate(dtime);
            }
            velocity += gravity * dTime;
            if (velocity > gravity) {
                velocity = gravity;
            }
            Position.Y += velocity * dTime;
            
            //hit tile from below
            if (!Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_LEFT]));
                if (intersection.Width * intersection.Height > 0) {
                    //break tile
                    if (Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Breakable) {
                        Console.WriteLine("Tile broken!");
                        Game.currentMap.ChangeTile(Corners[CORNER_TOP_LEFT]);
                    }
                    Position.Y = intersection.Bottom;
                    velocity = Math.Abs(velocity);
                }
            }
            //hit tile from below
            if (!Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_RIGHT]));
                if (intersection.Width * intersection.Height > 0) {
                    if (Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Breakable) {
                        Console.WriteLine("Tile broken!");
                        Game.currentMap.ChangeTile(Corners[CORNER_TOP_RIGHT]);
                        //what item will spawn?
                        if (Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).TileValue == 33) {
                            
                        }
                    }
                    Position.Y = intersection.Bottom;
                    velocity = Math.Abs(velocity);
                }
            }
            //keep on the tiles
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                if (intersection.Width * intersection.Height > 0) {
                    Position.Y = intersection.Top - Rect.Height;
                    if (velocity != gravity) {
                        SetSprite("Stand");
                    }
                    velocity = gravity;
                    isJumping = false;
                }
            }
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_RIGHT]));
                if (intersection.Width * intersection.Height > 0) {
                    Position.Y = intersection.Top - Rect.Height;
                    if (velocity != gravity) {
                        SetSprite("Stand");
                    }
                    velocity = gravity;
                    isJumping = false;
                }
            }
            if (i.KeyPressed(OpenTK.Input.Key.P)) {
                Console.WriteLine("Player Position, X: " + Position.X + " Y: " + Position.Y);
                Console.WriteLine("Player Position, X: " + (int)(Position.X/Game.TILE_SIZE) + " Y: " + (int)(Position.Y/Game.TILE_SIZE));
            }
        }//end update
        protected void SetJump(float height,float duration) {
            impulse = 2 * height / duration;
            impulse *= -1;
            gravity = -impulse / duration;
        }
    }
}
