using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Bullet {
        public PointF Position = new PointF(0f, 0f);
        public PointF Velocity = new PointF(0f, 0f);
        protected float gravity = 100.0f; //same formula as in player character
        protected float impulse = 50.0f;
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
        }
        public void Update(float dTime) {
            //movement
            Position.X += Velocity.X * dTime;
            Position.Y += Velocity.Y * dTime;
            //apply gravity
            Position.Y += gravity * dTime;
            //collision with ground
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                if (intersection.Width*intersection.Height > 0) {
                    //add impulse
                    Position.Y += impulse;
                }
            }
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_RIGHT]));
                if (intersection.Width * intersection.Height > 0) {
                    //add impulse
                    Position.Y += impulse;
                }
            }
        }
        public void Render(PointF offsetPosition) {
            Rectangle renderRect = new Rectangle(0, 0, 10, 10);
            renderRect.X = (int)(Position.X - 5.0f) - (int)offsetPosition.X;
            renderRect.Y = (int)(Position.Y - 5.0f) - (int)offsetPosition.Y;
            GraphicsManager.Instance.DrawRect(renderRect, Color.Red);
        }

    }
}
