//#define ENEMYDEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MarioWorld1_1 {
    class EnemyCharacter : Character{
        public enum State { Alive, Dead, Dead1, Dead2 }//dead1, dead2 used for koopa exclusively
        public State CurrentState = State.Alive;
        protected float speed = 60.0f;
        protected bool moveUpDown = false;
        public bool IsSeen = false;
        public float Direction = 1.0f;
        protected float gravity = 150.0f; //same formula as in player character
        protected float deathTimer = 0.0f;
        protected EnemyCharacter(string spritePath, bool movingUpDown) : base(spritePath) {
            
        }
        public virtual void Update(float dTime) {
            bool xMovement = false;
            Animate(dTime);
            if (Direction > 0) {
                faceLeft = false;
            }
            else {
                faceLeft = true;
            }
            //apply gravity
            Position.Y += gravity * dTime;
            //floor collision lower left
            if (CurrentState != State.Dead) {
                if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Position.Y = intersection.Top - Rect.Height;
                        xMovement = true;
                    }
                }
                //floor collision lower right
                if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_RIGHT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Position.Y = intersection.Top - Rect.Height;
                        xMovement = true;
                    }
                }
                //horizontal movement
                if (xMovement) {
                    Position.X += Direction * speed * dTime;
                }
                //wall collision
                //upper left
                if (!Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_LEFT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Direction *= -1;
                        Position.X = intersection.Right;
#if ENEMYDEBUG
                    Console.WriteLine("Enemy Position: X: " + Position.X + " , Y: " + Position.Y);
#endif
                    }
                }
                //upper right
                if (!Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_RIGHT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Direction *= -1;
                        Position.X = intersection.Left - Rect.Width;
#if ENEMYDEBUG
                    Console.WriteLine("Enemy Position: X: " + Position.X + " , Y: " + Position.Y);
#endif
                    }
                }
                //lower left
                if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Direction *= -1;
                        Position.X = intersection.Right;

#if ENEMYDEBUG

                    Console.WriteLine("Enemy Position: X: " + Position.X + " , Y: " + Position.Y);
#endif
                    }
                }
                //lower right
                if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_RIGHT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Direction *= -1;
                        Position.X = intersection.Left - Rect.Width;

#if ENEMYDEBUG
                    Console.WriteLine("Enemy Position: X: " + Position.X + " , Y: " + Position.Y);
#endif
                    }
                }
            }//end alive

        }
        public void Die(float dTime) {
            SetSprite("Dead");
            //jump
            deathTimer += dTime;
            if (deathTimer > 1.25f) {
                deathTimer -= 1.25f;
                Destroy();
            }
        }
    }
}
