using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using System.Drawing;

namespace MarioWorld1_1 {
    class PlayerCharacter : Character{
        //what type is hero
        protected enum State { Normal, Large};
        protected State CurrentState { get; set; }

        public float speed = 3*Game.TILE_SIZE;
        public int Lifes = 3; //default amount of lifes
        public float Impulse = 0.0f;
        protected float velocity = 0.0f;
        protected float gravity = 0f;
        private bool isJumping = false;

        public PointF[] BottomCorners {
            get {
                PointF[] allCorners = Corners;
                if (CurrentState == State.Normal) {
                    return allCorners;
                }
                allCorners[0].Y += 0.5f * Rect.Height - 1;
                allCorners[1].Y += 0.5f * Rect.Height - 1;
                return allCorners;
            }
        }
        public PointF[] TopCorners {
            get {
                PointF[] allCorners = Corners;
                allCorners[2].Y -= Rect.Height * 0.5f - 1;
                allCorners[3].Y -= Rect.Height * 0.5f - 1;
                return allCorners;
            }
        }

        public PlayerCharacter(string spritePath) : base(spritePath) {
            AddSprite("Stand", new Rectangle(12, 6, 16, 16));
            AddSprite("Run", new Rectangle(30, 27, 16, 16), new Rectangle(47, 27, 16, 16), new Rectangle(64, 27, 16, 16));
            AddSprite("Jump", new Rectangle(29, 6, 16, 16));
            AddSprite("LargeStand", new Rectangle(10, 65, 16, 32));
            AddSprite("LargeRun",new Rectangle(30,105,16,32),new Rectangle(50,105,16,32),new Rectangle(70,105,16,32));
            AddSprite("LargeJump", new Rectangle(30, 65, 16, 32));
            SetSprite("Stand");
        }
        public void Update(float dTime) {
            CurrentState = State.Large;

            InputManager i = InputManager.Instance;
            if (CurrentState == State.Normal) {
                SetJump(/*3.50f*/4 * Game.TILE_SIZE, 0.75f);
            }
            else {
                SetJump(5 * Game.TILE_SIZE, 1.0f);
            }
            //move left
            if (i.KeyDown(OpenTK.Input.Key.Left)|| i.KeyDown(OpenTK.Input.Key.A)) {
                if (velocity == gravity) {
                    faceLeft = true;
                    if (CurrentState == State.Normal) {
                        SetSprite("Run");
                    }
                    else if (CurrentState == State.Large) {
                        SetSprite("LargeRun");
                    }
                    Animate(dTime);
                }
                Position.X -= speed * dTime;
                if (CurrentState == State.Large) {
                    if (!Game.Instance.GetTile(TopCorners[CORNER_TOP_LEFT]).Walkable) {
                        Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(TopCorners[CORNER_TOP_LEFT]));
                        if (intersection.Width * intersection.Height > 0) {
                            Position.X = intersection.Right;
                        }
                    }
                }
                if (!Game.Instance.GetTile(BottomCorners[CORNER_TOP_LEFT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(BottomCorners[CORNER_TOP_LEFT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Position.X = intersection.Right;
                    }
                }
                if (!Game.Instance.GetTile(BottomCorners[CORNER_BOTTOM_LEFT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(BottomCorners[CORNER_BOTTOM_LEFT]));
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
                    if (CurrentState == State.Normal) {
                        SetSprite("Run");
                    }
                    else if (CurrentState == State.Large) {
                        SetSprite("LargeRun");
                    }
                    Animate(dTime);
                }
                Position.X += speed * dTime;
                if (CurrentState == State.Large) {
                    if (!Game.Instance.GetTile(TopCorners[CORNER_TOP_RIGHT]).Walkable) {
                        Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(TopCorners[CORNER_TOP_RIGHT]));
                        if (intersection.Width * intersection.Height > 0) {
                            Position.X = intersection.Left - Rect.Width;
                        }
                    }
                }
                if (!Game.Instance.GetTile(BottomCorners[CORNER_TOP_RIGHT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(BottomCorners[CORNER_TOP_RIGHT]));
                    if (intersection.Width*intersection.Height > 0) {
                        Position.X = intersection.Left - Rect.Width;
                    }
                }
                if (!Game.Instance.GetTile(BottomCorners[CORNER_BOTTOM_RIGHT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(BottomCorners[CORNER_BOTTOM_RIGHT]));
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
                    Jump(Impulse);
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
                        //what item will spawn?
                        if (Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Item != null) {
                            //create item by adding it into map
                            Item item = Item.SpawnItem(Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Item);
                            Map.items.Add(item);
                            //set items position
                            Map.items[Map.items.Count - 1].Position.X = Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).WorldPosition.X;
                            Map.items[Map.items.Count - 1].Position.Y = Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).WorldPosition.Y - Game.TILE_SIZE;
                            Map.items[Map.items.Count - 1].IsSpawned = true;
                        }
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
                        //what item will spawn?
                        if (Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Item != null) {
                            Item item = Item.SpawnItem(Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Item);
                            Map.items.Add(item);
                            //set item position
                            Map.items[Map.items.Count - 1].Position.X = Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).WorldPosition.X;
                            Map.items[Map.items.Count - 1].Position.Y = Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).WorldPosition.Y-Game.TILE_SIZE;
                            Map.items[Map.items.Count - 1].IsSpawned = true;
                        }
                        Game.currentMap.ChangeTile(Corners[CORNER_TOP_RIGHT]);
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
                        if (CurrentState == State.Normal) {
                            SetSprite("Stand");
                        }
                        else if(CurrentState == State.Large) {
                            SetSprite("LargeStand");
                        }
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
                        if (CurrentState == State.Normal) {
                            SetSprite("Stand");
                        }
                        else if (CurrentState == State.Large) {
                            SetSprite("LargeStand");
                        }
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
            Impulse = 2 * height / duration;
            Impulse *= -1;
            gravity = -Impulse / duration;
        }
        public void Jump(float impulse) {
            velocity = impulse;
            if (CurrentState == State.Normal) {
                SetSprite("Jump");
            }
            else if (CurrentState == State.Large) {
                SetSprite("LargeJump");
            }
        }
        public void ChangeForm(string newForm) {
            if (newForm == "Large") {
                CurrentState = State.Large;
            }
        }
        public override void Render(PointF offsetPosition) {
            if (CurrentState == State.Normal) {
                base.Render(offsetPosition);
                
            }
            else {
                Point renderPosition = new Point((int)Position.X, (int)Position.Y);
                renderPosition.X -= (int)offsetPosition.X - 1;
                renderPosition.Y -= 1;
                Rectangle renderRect = SpriteSources[currentSprite][currentFrame];
                renderRect.X -= 1;
                renderRect.Y -= 1;
                if (!faceLeft) {
                    TextureManager.Instance.Draw(Sprite, renderPosition, 1.0f, renderRect);
                }
                else {
                    TextureManager.Instance.Draw(Sprite, new Point(renderPosition.X + renderRect.Width, renderPosition.Y), new Point(-1, 1), renderRect);
                }

                foreach (PointF corner in Corners) {
                    RectangleF draw = new RectangleF(corner.X - 3 - offsetPosition.X, corner.Y - 3 - offsetPosition.Y, 6, 6);
                    GraphicsManager.Instance.DrawRect(draw, Color.Black);
                }
                foreach (PointF corner in BottomCorners) {
                    RectangleF draw = new RectangleF(corner.X - 2 - offsetPosition.X, corner.Y - 2 - offsetPosition.Y, 4, 4);
                    GraphicsManager.Instance.DrawRect(draw, Color.Blue);
                }
                foreach (PointF corner in TopCorners) {
                    RectangleF draw = new RectangleF(corner.X - 2 - offsetPosition.X, corner.Y - 2 - offsetPosition.Y, 4, 4);
                    GraphicsManager.Instance.DrawRect(draw, Color.Red);
                }
            }
            
        }
    }
}
