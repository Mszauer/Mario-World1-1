﻿using System.Drawing;
using GameFramework;
using System;

namespace MarioWorld1_1 {
    class Bullet {
        public PointF Position = new PointF(0f, 0f);
        public PointF Velocity = new PointF(0f, 0f);
        protected float gravity = 0.0f; //same formula as in player character
        protected float impulse = 0.0f;
        public bool ToDestroy = false;
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

            //collision with non-walkable tile
            if (!Game.Instance.GetTile(new PointF(Rect.X+ (float)Rect.Height,Rect.Y+ (float)Rect.Height)).Walkable) {
                PointF bottomLeft = new PointF(Rect.X + (float)Rect.Height, Rect.Y + (float)Rect.Height);
                //collision 
                Console.WriteLine("Collision Direction: " + Intersections.CollisionDirection(Rect, Game.Instance.GetTileRect(bottomLeft)));

                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(bottomLeft));
                if (intersection.Width*intersection.Height != 0) {
                    //bottom collision
                    if (Intersections.CollisionDirection(Rect, Game.Instance.GetTileRect(bottomLeft)) == Intersections.SideHit.Bottom) {
                        Jump(impulse);
                        Position.Y = intersection.Y - Rect.Height;
                    }
                    //left collision
                    else if (Intersections.CollisionDirection(Rect, Game.Instance.GetTileRect(bottomLeft)) == Intersections.SideHit.Left) {
                        Console.WriteLine("Destroy projectile: " + ToDestroy);
                        if (!ToDestroy) {
                            ToDestroy = true;
                        }
                    }
                    //right collision
                    else if (Intersections.CollisionDirection(Rect, Game.Instance.GetTileRect(bottomLeft)) == Intersections.SideHit.Right) {
                        Console.WriteLine("Destroy projectile: " + ToDestroy);
                        if (!ToDestroy) {
                            ToDestroy = true;
                        }
                    }
                    //top collision
                    else if (Intersections.CollisionDirection(Rect, Game.Instance.GetTileRect(bottomLeft)) == Intersections.SideHit.Top) {

                    }
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
            Rectangle worldRect = new Rectangle(0, 0, (Game.currentMap[0].Length - 1) * Game.TILE_SIZE, (Game.currentMap.Length - 1)*Game.TILE_SIZE);
            Rectangle intersect = Intersections.Rect(Rect, worldRect);
            bool inside = intersect.Width == Rect.Width && intersect.Height == Rect.Height;
            return inside;
        }
    }
}
