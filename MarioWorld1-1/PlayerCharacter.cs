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
        public PlayerCharacter(string spritePath) : base(spritePath) {
            AddSprite("Right", new Rectangle(12, 6, 14, 14));
            SetSprite("Right");
            SetJump(2 * Game.TILE_SIZE, 0.75f);
        }
        public void Render(/*offset here*/) {
            Point renderPosition = new Point((int)Position.X, (int)Position.Y);
            //offset position stuff
            //GraphicsManager.Instance.DrawRect(new Rectangle(renderPosition.X,renderPosition.Y,SpriteSources[currentSprite][currentFrame].Width, SpriteSources[currentSprite][currentFrame].Height), Color.Red);
            TextureManager.Instance.Draw(Sprite, renderPosition, 1.0f, SpriteSources[currentSprite][currentFrame]);
        }
        public void Update(float dTime) {
            InputManager i = InputManager.Instance;
            //move left
            if (i.KeyDown(OpenTK.Input.Key.Left)|| i.KeyDown(OpenTK.Input.Key.A)) {
                if (velocity == gravity) {
                    //set sprite left
                }
                //call animate
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
            }
            //move right
            if (i.KeyDown(OpenTK.Input.Key.Right)|| i.KeyDown(OpenTK.Input.Key.D)) {
                if (velocity == gravity) {
                    //set sprite right
                }
                //call animate
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
            }
            //jump!
            if (i.KeyDown(OpenTK.Input.Key.W)|| i.KeyDown(OpenTK.Input.Key.Up)) {
                velocity = impulse;
                //set jump sprite
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
            //keep on the tiles
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_RIGHT]));
                if (intersection.Width * intersection.Height > 0) {
                    Position.Y = intersection.Top - Rect.Height;
                    Console.WriteLine("Position.Y: "+Position.Y);
                    if (velocity != gravity) {
                        //SetSprite("Down");
                    }
                    velocity = gravity;
                }
            }
            //hit tile from below
            /*
            if (!Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_LEFT]));
                if (intersection.Width * intersection.Height > 0) {
                    Position.Y = intersection.Bottom;
                    velocity = Math.Abs(velocity);
                }
            }
            //hit tile from below
            if (!Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_RIGHT]));
                if (intersection.Width * intersection.Height > 0) {
                    Position.Y = intersection.Bottom;
                    velocity = Math.Abs(velocity);
                }
            }
            */
            Console.WriteLine("Position.Y: " + Position.Y);
        }//end update
        protected void SetJump(float height,float duration) {
            impulse = 2 * height / duration;
            impulse *= -1;
            gravity = -impulse / duration;
        }
    }
}
