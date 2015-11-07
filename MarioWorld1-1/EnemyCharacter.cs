using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MarioWorld1_1 {
    class EnemyCharacter : Character{
        public enum State { Alive, Dead1, Dead2 }
        public State CurrentState = State.Alive;
        protected float speed = 60.0f;
        protected bool moveUpDown = false;
        public bool IsSeen = false;
        protected float directions = 1.0f;
        protected EnemyCharacter(string spritePath, bool movingUpDown) : base(spritePath) {
            
        }
        public virtual void Update(float dTime) {
            //need to add death!
            Animate(dTime);
            //movement
            Position.X += directions * speed * dTime;
            if (directions > 0) {
                faceLeft = false;
            }
            else {
                faceLeft = true;
            } 
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
