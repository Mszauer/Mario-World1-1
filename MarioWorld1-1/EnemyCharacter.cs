using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MarioWorld1_1 {
    class EnemyCharacter : Character{
        float speed = 60.0f;
        bool moveUpDown = false;
        public bool IsSeen = false;
        float directions = 1.0f;
        public EnemyCharacter(string spritePath, bool movingUpDown) : base(spritePath) {
            AddSprite("Walk", new Rectangle(1, 4, 16, 16), new Rectangle(22, 4, 16, 16));
            AddSprite("Death", new Rectangle(44, 4, 16, 16));
            SetSprite("Walk");
            moveUpDown = movingUpDown;
        }
        public void Update(float dTime) {
            //need to add death!
            Animate(dTime);
            //movement
            Position.X += directions * speed * dTime;
            //wall collision
            //upper left
            if (!Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_LEFT]));
                if (intersection.Width*intersection.Height > 0) {
                    directions *= -1;
                    Position.X = intersection.Right;
                }
            }
            //upper right
            if (!Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_RIGHT]));
                if (intersection.Width*intersection.Height > 0) {
                    directions *= -1;
                    Position.X = intersection.Left - Rect.Width;
                }
            }
            //lower left
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                if (intersection.Width * intersection.Height > 0) {
                    directions *= -1;
                    Position.X = intersection.Right;
                }
            }
            //lower right
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_RIGHT]));
                if (intersection.Width*intersection.Height > 0) {
                    directions *= -1;
                    Position.X = intersection.Left - Rect.Width;
                }
            }
        }
    }
}
