using System.Drawing;

namespace MarioWorld1_1 {
    class Intersections {
        public enum SideHit { None, Left, Top, Right, Bottom }
        public static SideHit CollisionSide = SideHit.None;
        public static Rectangle Rect(Rectangle a, Rectangle b) {
            Rectangle result = new Rectangle(0, 0, 0, 0);
            if (a.Left < b.Right && a.Right > b.Left && a.Top<b.Bottom && a.Bottom > b.Top) {
                result.X = System.Math.Max(a.Left, b.Left);
                result.Y = System.Math.Max(a.Top, b.Top);
                int right = System.Math.Min(a.Right, b.Right);
                int bottom = System.Math.Min(a.Bottom, b.Bottom);
                result.Width = right - result.X;
                result.Height = bottom - result.Y;
            }
            return result;
        }
        public static SideHit CollisionDirection(Rectangle a, Rectangle b) {
            float w = 0.5f * (a.Width + b.Width);
            float h = 0.5f * (a.Height + b.Height);
            float dx = Center(a).X - Center(b).X;
            float dy = Center(a).Y - Center(b).Y;

            if (System.Math.Abs(dx) <= w && System.Math.Abs(dy) <= h) {
                /* collision! */
                float wy = w * dy;
                float hx = h * dx;

                if (wy > hx) {
                    if (wy > -hx) {
                        //top side collision
                        CollisionSide = SideHit.Top;
                    }
                    else {
                        //left side collision
                        CollisionSide = SideHit.Left;
                    }
                }
                else {
                    if (wy > -hx) {
                        //right side collision
                        CollisionSide = SideHit.Right;
                    }
                    else {
                        //bottom side collision
                        CollisionSide = SideHit.Bottom;
                    }
                }
            }
            return CollisionSide;
        }
        public static Point Center(Rectangle a) {
            int xCenter = a.X + (a.Width / 2);
            int yCenter = a.Y + (a.Height / 2);
            return new Point(xCenter, yCenter);
        }
    }
}
