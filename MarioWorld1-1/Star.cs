using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Star : Item {
        public int ItemValue = 0;
        public Star(string spriteSheet) : base(spriteSheet) {
            AddSprite("Default", new Rectangle(0, 50, 16, 16), new Rectangle(16, 50, 16, 16), new Rectangle(32, 50, 16, 16), new Rectangle(48, 50, 16, 16));
            currentSprite = "Default";
        }
        public override void Update(float dTime) {
            //animations
            Animate(dTime);
            //horizontal movement
            Position.X += (speed/2.0f) * dTime;
            //bounce height limit
            if ((Position.Y / Game.TILE_SIZE) < 6) {
                direction *= -1;
            }
            //vertical movement
            Position.Y += 1.5f*direction;
            //Ground Collision
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                    if (intersection.Width * intersection.Height > 0) {
                        direction *= -1;
                        Position.Y = intersection.Top - Rect.Height;
                    }
                }
                if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                    if (intersection.Width * intersection.Height > 0) {
                        direction *= -1;
                        Position.Y = intersection.Top - Rect.Height;
                    }
                }
            }

        }
    }
