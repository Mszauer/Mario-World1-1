using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Bullet {
        public PointF Position = new PointF(0f, 0f);
        public PointF Velocity = new PointF(0f, 0f);
        protected float gravity = 0.0f; //same formula as in player character
        protected float impulse = 0.0f;
        public Rectangle Rect {
            get {
                return new Rectangle((int)Position.X - 5, (int)Position.Y - 5, 10, 10);
            }
        }
        public PointF[] Corners {
            get {
                float w = Rect.Width;
                float h = Rect.Height;
                return new PointF[] {
                    new PointF(Rect.X,Rect.Y),
                    new PointF(Rect.X+w,Rect.Y),
                    new PointF(Rect.X,Rect.Y+h),
                    new PointF(Rect.X+w,Rect.Y+h)
                };
            }
        }
        public static readonly int CORNER_TOP_LEFT = 0;
        public static readonly int CORNER_TOP_RIGHT = 1;
        public static readonly int CORNER_BOTTOM_LEFT = 2;
        public static readonly int CORNER_BOTTOM_RIGHT = 3;
        public Bullet(PointF pos, PointF vel) {
            Position = pos;
            Velocity = vel;
            SetJump(2f * Game.TILE_SIZE, 1.00f);
        }
        public void Update(float dTime) {
            //movement
            Position.X += Velocity.X * dTime;
            //apply gravity
            Velocity.Y += gravity * dTime;
            Position.Y += Velocity.Y * dTime;

            //collision with ground
            if (!Game.Instance.GetTile(new PointF(Position.X+ (float)Rect.Height,Position.Y+ (float)Rect.Height)).Walkable ) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(new PointF(Position.X + (float)Rect.Height, Position.Y + (float)Rect.Height)));
                if (intersection.Width * intersection.Height > 0) {
                    Jump(impulse);
                    Position.Y = intersection.Y - Rect.Height;
                }
            }
        }
        public void Render(PointF offsetPosition) {
            Rectangle renderRect = new Rectangle(0, 0, 10, 10);
            renderRect.X = (int)(Position.X - 5.0f) - (int)offsetPosition.X;
            renderRect.Y = (int)(Position.Y - 5.0f) - (int)offsetPosition.Y;
            GraphicsManager.Instance.DrawRect(renderRect, Color.Red);
        }
        protected void SetJump(float height, float duration) {
            impulse = 2 * height / duration;
            impulse *= -1;
            gravity = -impulse / duration;
        }
        public void Jump(float impulse) {
            Velocity.Y = impulse;
        }
        public bool InBounds() {
            //upper left
            if (Position.X /Game.TILE_SIZE < 0 || Position.X /Game.TILE_SIZE > Game.currentMap[0].Length) { //length of map x
                return false;
            }
            //upper right
            if ((Position.X + Rect.Width) /Game.TILE_SIZE < 0 || (Position.X+Rect.Width) / Game.TILE_SIZE > Game.currentMap[0].Length) {
                return false;
            }
            //lower left
            if ((Position.X + Rect.Height) / Game.TILE_SIZE < 0 || (Position.X+Rect.Height) > Game.currentMap[0].Length) {
                return false;
            }
            //lower right
            if ((Position.X +Rect.Height+Rect.Width) / Game.TILE_SIZE < 0 || (Position.X+Rect.Height+Rect.Width) / Game.TILE_SIZE > Game.currentMap[0].Length) {
                return false;
            }
            return true;
        }
    }
}
