#define TILEDEBUG
//#define WINDEBUG
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
        public enum State { Normal, Fire, Invincible};
        public State CurrentState { get; set; }
        public float speed = 3*Game.TILE_SIZE;
        public int Lifes = 3; //default amount of lifes
        public float Impulse = 0.0f;
        protected float velocity = 0.0f;
        protected float gravity = 0f;
        private bool isJumping = false;
        public bool Large = false;
        public bool Invincible = false;
        protected float deathTimer = 0.0f;
        public float InvincibilityTimer = 0.0f; // used when mario bumps into enemy
        protected float invincibilityTime = 0.0f; //used for invincibility
        BreakEffect breakFX = null;
        bool xMovement = false;
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
            AddSprite("FireRun", new Rectangle(295, 25, 16, 16), new Rectangle(313,25,16,16),new Rectangle(277,25,16,16));
            AddSprite("FireStand", new Rectangle(260, 5, 16, 16));
            AddSprite("FireJump", new Rectangle(277, 5, 16, 16));
            AddSprite("LargeStand", new Rectangle(10, 65, 16, 32));
            AddSprite("LargeRun",new Rectangle(30,105,16,32),new Rectangle(50,105,16,32),new Rectangle(70,105,16,32));
            AddSprite("LargeJump", new Rectangle(30, 65, 16, 32));
            AddSprite("LargeCrouch", new Rectangle(53, 67, 16, 32));
            AddSprite("LargeFireStand", new Rectangle(258, 67, 16, 32));
            AddSprite("LargeFireRun", new Rectangle(300,105,16,32),new Rectangle(280,105,16,32),new Rectangle(320,105,16,32));
            AddSprite("LargeFireJump", new Rectangle(279, 67, 16, 32));
            AddSprite("LargeFireCrouch", new Rectangle(300, 65, 16, 32));
            AddSprite("InvincibleStand", new Rectangle(12, 6, 16, 16), new Rectangle(260, 5, 16, 16), new Rectangle(502, 6, 16, 16));
            AddSprite("InvincibleLargeStand", new Rectangle(10, 65, 16, 32), new Rectangle(258, 67, 16, 32), new Rectangle(500, 65, 16, 32));
            AddSprite("InvincibleJump", new Rectangle(29, 6, 16, 16), new Rectangle(277, 5, 16, 16), new Rectangle(520, 5, 16, 16));
            AddSprite("InvincibleLargeJump", new Rectangle(30, 65, 16, 32), new Rectangle(279, 67, 16, 32), new Rectangle(523, 67, 16, 32));
            AddSprite("InvincibleRun", new Rectangle(30, 26, 16, 16), new Rectangle(313, 25, 16, 16), new Rectangle(558, 25, 16, 16));
            AddSprite("InvincibleLargeRun", new Rectangle(30, 105, 16, 32), new Rectangle(300, 105, 16, 32), new Rectangle(565, 105, 16, 32));
            AddSprite("InvincibleLargeCrouch", new Rectangle(300, 65, 16, 32), new Rectangle(300, 65, 16, 32), new Rectangle(545, 65, 16, 32));
            AddSprite("Dead", new Rectangle(49, 6, 16, 16));
            SetSprite("Stand");
            SetJump(5 * Game.TILE_SIZE, 1.0f);

        }
        public void Update(float dTime) {
            bool dead = Game.Instance.CurrentState == Game.State.Dying;
            InputManager i = InputManager.Instance;
            //death invincibility
            if (InvincibilityTimer > 0.0f) {
                InvincibilityTimer -= dTime;
                if (InvincibilityTimer < 0.0f) {
                    InvincibilityTimer = 0.0f;
                }
            }
            if (!dead) {
                //move left
                if (i.KeyDown(OpenTK.Input.Key.Left) || i.KeyDown(OpenTK.Input.Key.A)) {
                    if (velocity == gravity) {
                        faceLeft = true;
                        if (CurrentState == State.Normal && Large) {
                            SetSprite("LargeRun");
                        }
                        else if (CurrentState == State.Normal) {
                            SetSprite("Run");
                        }
                        else if (CurrentState == State.Fire && Large) {
                            SetSprite("LargeFireRun");
                        }
                        else if (CurrentState == State.Fire) {
                            SetSprite("FireRun");
                        }
                        else if (CurrentState == State.Invincible && Large) {
                            SetSprite("InvincibleLargeRun");
                        }
                        else if (CurrentState == State.Invincible) {
                            SetSprite("InvincibleRun");
                        }
                    }
                    Position.X -= speed * dTime;
                    if (Position.Y > 0 && ((CurrentState == State.Normal || CurrentState == State.Invincible) && Large)) {
                        if (!Game.Instance.GetTile(TopCorners[CORNER_TOP_LEFT]).Walkable && Game.Instance.GetTile(TopCorners[CORNER_TOP_LEFT]).TileValue != 35) {
                            Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(TopCorners[CORNER_TOP_LEFT]));
                            if (intersection.Width * intersection.Height > 0) {
                                Position.X = intersection.Right;
                            }
                        }
                        if (!Game.Instance.GetTile(TopCorners[CORNER_BOTTOM_LEFT]).Walkable && Game.Instance.GetTile(TopCorners[CORNER_BOTTOM_LEFT]).TileValue != 35) {
                            Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(TopCorners[CORNER_BOTTOM_LEFT]));
                            if (intersection.Width * intersection.Height > 0) {
                                Position.X = intersection.Right;
                            }
                        }
                    }
                    if (Position.Y > 0 && (!Game.Instance.GetTile(BottomCorners[CORNER_TOP_LEFT]).Walkable && Game.Instance.GetTile(BottomCorners[CORNER_TOP_LEFT]).TileValue != 35)) {
                        Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(BottomCorners[CORNER_TOP_LEFT]));
                        if (intersection.Width * intersection.Height > 0) {
                            Position.X = intersection.Right;
                        }
                    }
                    if (Position.Y > 0 && (!Game.Instance.GetTile(BottomCorners[CORNER_BOTTOM_LEFT]).Walkable && Game.Instance.GetTile(BottomCorners[CORNER_BOTTOM_LEFT]).TileValue != 35)) {
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
                else if (i.KeyDown(OpenTK.Input.Key.Right) || i.KeyDown(OpenTK.Input.Key.D)) {
                    if (velocity == gravity) {
                        faceLeft = false;
                        if (CurrentState == State.Normal && Large) {
                            SetSprite("LargeRun");
                        }
                        else if (CurrentState == State.Normal) {
                            SetSprite("Run");
                        }
                        else if (CurrentState == State.Fire && Large) {
                            SetSprite("LargeFireRun");
                        }
                        else if (CurrentState == State.Fire) {
                            SetSprite("FireRun");
                        }
                        else if (CurrentState == State.Invincible && Large) {
                            SetSprite("InvincibleLargeRun");
                        }
                        else if (CurrentState == State.Invincible) {
                            SetSprite("InvincibleRun");
                        }
                    }
                    Position.X += speed * dTime;
                    if (Position.Y > 0 && ((CurrentState == State.Normal || CurrentState == State.Invincible) && Large)) {
                        if (!Game.Instance.GetTile(TopCorners[CORNER_TOP_RIGHT]).Walkable && Game.Instance.GetTile(TopCorners[CORNER_TOP_RIGHT]).TileValue != 35) {
                            Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(TopCorners[CORNER_TOP_RIGHT]));
                            if (intersection.Width * intersection.Height > 0) {
                                Position.X = intersection.Left - Rect.Width;
                            }
                        }
                        if (!Game.Instance.GetTile(TopCorners[CORNER_BOTTOM_RIGHT]).Walkable && Game.Instance.GetTile(TopCorners[CORNER_BOTTOM_RIGHT]).TileValue != 35) {
                            Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(TopCorners[CORNER_BOTTOM_RIGHT]));
                            if (intersection.Width * intersection.Height > 0) {
                                Position.X = intersection.Left - Rect.Width;
                            }
                        }
                    }
                    if (Position.Y > 0 && (!Game.Instance.GetTile(BottomCorners[CORNER_TOP_RIGHT]).Walkable && Game.Instance.GetTile(BottomCorners[CORNER_TOP_RIGHT]).TileValue != 35)) {
                        Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(BottomCorners[CORNER_TOP_RIGHT]));
                        if (intersection.Width * intersection.Height > 0) {
                            Position.X = intersection.Left - Rect.Width;
                        }
                    }
                    if (Position.Y > 0 && (!Game.Instance.GetTile(BottomCorners[CORNER_BOTTOM_RIGHT]).Walkable && Game.Instance.GetTile(BottomCorners[CORNER_BOTTOM_RIGHT]).TileValue != 35)) {
                        Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(BottomCorners[CORNER_BOTTOM_RIGHT]));
                        if (intersection.Width * intersection.Height > 0) {
                            Position.X = intersection.Left - Rect.Width;
                        }
                    }
                    //map boundry check
                    if (Position.X > 206f * Game.TILE_SIZE) {
                        Position.X = 206f * Game.TILE_SIZE;
                    }
                }
                //Courch when large
                else if (i.KeyDown(OpenTK.Input.Key.Down) || i.KeyDown(OpenTK.Input.Key.S)) {
                    if (Large && CurrentState == State.Normal) {
                        SetSprite("LargeCrouch");
                    }
                    else if (Large && CurrentState == State.Fire) {
                        SetSprite("LargeFireCrouch");
                    }
                    else if (Large && CurrentState == State.Invincible) {
                        SetSprite("InvincibleLargeCrouch");
                    }
                }
                else {
                    if (Large && CurrentState == State.Normal) {
                        SetSprite("LargeStand");
                    }
                    else if (Large && CurrentState == State.Fire) {
                        SetSprite("LargeFireStand");
                    }
                    else if (Large && CurrentState == State.Invincible) {
                        SetSprite("InvincibleLargeStand");
                    }
                    else if (CurrentState == State.Normal) {
                        SetSprite("Stand");
                    }
                    else if (CurrentState == State.Fire) {
                        SetSprite("FireStand");
                    }
                    else if (CurrentState == State.Invincible) {
                        SetSprite("InvincibleStand");
                    }
                }
                //jump!
                if (!isJumping) {
                    if (i.KeyDown(OpenTK.Input.Key.W) || i.KeyDown(OpenTK.Input.Key.Up)) {
                        //play jump sound
                        if (!SoundManager.Instance.IsPlaying(Game.Instance.SoundBank["HeroJump"])) {
                            SoundManager.Instance.PlaySound(Game.Instance.SoundBank["HeroJump"]);
                        }
                        //jump
                        Jump(Impulse);
                    }
                }
                
            }
            velocity += gravity * dTime;
            if (velocity > gravity) {
                velocity = gravity;
            }
            Position.Y += velocity * dTime;
            if (!dead && Position.Y > 0) {
                //hit tile from below
                if (!Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_LEFT]));
                    if (intersection.Width * intersection.Height > 0) {
                        //break tile
                        if (Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Breakable && (Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Item != null ||Large)) {
                            //what item will spawn?
#if TILEDEBUG
                            Console.WriteLine("Tile broken!");
                            Console.WriteLine("Tile Value: " + Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).TileValue);
                            Console.WriteLine("Tile contains: " + Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Item);
#endif
                            if (Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Item != null) { 
                                //item spawn sounds
                                if (Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Item == "Coin") {
                                    SoundManager.Instance.PlaySound(Game.Instance.SoundBank["Coin"]);
                                }
                                else {
                                    SoundManager.Instance.PlaySound(Game.Instance.SoundBank["ItemSpawn"]);
                                }
                                //create item by adding it into map
                                Item item = Item.SpawnItem(Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Item);
                                Game.currentMap.Items.Add(item);
                                //set items position
                                Game.currentMap.Items[Game.currentMap.Items.Count - 1].Position.X = Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).WorldPosition.X;
                                Game.currentMap.Items[Game.currentMap.Items.Count - 1].Position.Y = Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).WorldPosition.Y - Game.TILE_SIZE;
                                Game.currentMap.Items[Game.currentMap.Items.Count - 1].IsSpawned = true;
                                Game.currentMap.Items[Game.currentMap.Items.Count - 1].StartPos = Game.currentMap.Items[Game.currentMap.Items.Count - 1].Position;
                            }
                            else {
                                //block break sound
                                SoundManager.Instance.PlaySound(Game.Instance.SoundBank["BreakBlock"]);
                                //break block effect
                                Game.currentMap.breakFX.Add(new BreakEffect("Assets/brickexplode.jpg", Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).WorldPosition));
                            }
                            Game.currentMap.ChangeTile(Corners[CORNER_TOP_LEFT]);
                            //create broken brick effect
                        }//end break if
                        else if (Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Breakable && Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).TileValue == 2) {
                            //set tile bumpage height
                            Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).YOffset = 3.75f; //3.75 is arbritrary that fits
#if TILEDEBUG
                            Console.WriteLine("Tile Bumped");
#endif
                            //play dud brick sound
                            SoundManager.Instance.PlaySound(Game.Instance.SoundBank["DudBrick"]);
                        }
                        else {
                            //play dud brick sound
                            SoundManager.Instance.PlaySound(Game.Instance.SoundBank["DudBrick"]);
                        }
                        Position.Y = intersection.Bottom;
                        velocity = Math.Abs(velocity);
                    }
                }
                //hit tile from below
                if (Position.Y > 0 && !Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Walkable) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_RIGHT]));
                    if (intersection.Width * intersection.Height > 0) {
                        if (Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Breakable && (Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Item != null ||Large)) {
                            //what item will spawn?
#if TILEDEBUG
                            Console.WriteLine("Tile broken!");
                            Console.WriteLine("Tile Value: "+ Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).TileValue);
                            Console.WriteLine("Tile contains: " + Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Item);
#endif
                            if (Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Item != null) {
                                //item spawn sounds
                                if (Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Item == "Coin") {
                                    SoundManager.Instance.PlaySound(Game.Instance.SoundBank["Coin"]);
                                }
                                else {
                                    SoundManager.Instance.PlaySound(Game.Instance.SoundBank["ItemSpawn"]);
                                }
                                Item item = Item.SpawnItem(Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Item);
                                Game.currentMap.Items.Add(item);
                                //set item position
                                Game.currentMap.Items[Game.currentMap.Items.Count - 1].Position.X = Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).WorldPosition.X;
                                Game.currentMap.Items[Game.currentMap.Items.Count - 1].Position.Y = Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).WorldPosition.Y - Game.TILE_SIZE;
                                Game.currentMap.Items[Game.currentMap.Items.Count - 1].IsSpawned = true;
                            }
                            
                            else {
                                //block break sound
                                SoundManager.Instance.PlaySound(Game.Instance.SoundBank["BreakBlock"]);
                                //break block effect
                                Game.currentMap.breakFX.Add(new BreakEffect("Assets/brickexplode.jpg", Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).WorldPosition));
                            }
                            Game.currentMap.ChangeTile(Corners[CORNER_TOP_RIGHT]);
                        }
                        //mario is small and can't break the brick
                        else if (Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Breakable && Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).TileValue == 2){
                            //set tile bumpage height
                            Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).YOffset = 3.75f; //3.75 is arbritrary that fits
#if TILEDEBUG
                            Console.WriteLine("Tile Bumped");
#endif
                            //play dud brick sound
                            SoundManager.Instance.PlaySound(Game.Instance.SoundBank["DudBrick"]);
                        }
                        else {
                            SoundManager.Instance.PlaySound(Game.Instance.SoundBank["DudBrick"]);
                        }
                        Position.Y = intersection.Bottom;
                        velocity = Math.Abs(velocity);
                    }
                }
                //keep on the tiles
                if (Position.Y > 0 && (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).Walkable && Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).TileValue != 35)) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Position.Y = intersection.Top - Rect.Height;
                        if (velocity != gravity) {
                            if (CurrentState == State.Normal && Large) {
                                SetSprite("LargeStand");
                            }
                            else if (CurrentState == State.Normal) {
                                SetSprite("Stand");
                            }
                            else if (CurrentState == State.Fire && Large) {
                                SetSprite("LargeFireStand");
                            }
                            else if (CurrentState == State.Fire) {
                                SetSprite("FireStand");
                            }
                            else if (CurrentState == State.Invincible && Large) {
                                SetSprite("InvincibleLargeStand");
                            }
                            else if (CurrentState == State.Invincible) {
                                SetSprite("InvincibleStand");
                            }
                        }
                        velocity = gravity;
                        isJumping = false;
                    }
                }
                if (Position.Y > 0 && (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable && Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).TileValue != 35)) {
                    Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_RIGHT]));
                    if (intersection.Width * intersection.Height > 0) {
                        Position.Y = intersection.Top - Rect.Height;
                        if (velocity != gravity) {
                            if (CurrentState == State.Normal && Large) {
                                SetSprite("LargeStand");
                            }
                            else if (CurrentState == State.Normal) {
                                SetSprite("Stand");
                            }
                            else if (CurrentState == State.Fire && Large) {
                                SetSprite("LargeFireStand");
                            }
                            else if (CurrentState == State.Fire) {
                                SetSprite("FireStand");
                            }
                            else if (CurrentState == State.Invincible && Large) {
                                SetSprite("InvincibleLargeStand");
                            }
                            else if (CurrentState == State.Invincible) {
                                SetSprite("InvincibleStand");
                            }
                        }
                        velocity = gravity;
                        isJumping = false;
                    }
                }
#if DEBUG
                if (i.KeyPressed(OpenTK.Input.Key.P)) {
                    Console.WriteLine("Player Position, X: " + Position.X + " Y: " + Position.Y);
                    Console.WriteLine("Player Position, X: " + (int)(Position.X / Game.TILE_SIZE) + " Y: " + (int)(Position.Y / Game.TILE_SIZE));
                    Console.WriteLine("Current state: " + CurrentState);
                    Console.WriteLine("Large form: " + Large);
                }
#endif
                //shoot projectiles
                if (CurrentState == State.Fire && i.KeyPressed(OpenTK.Input.Key.Space)) {
                    //play projectile sound
                    SoundManager.Instance.PlaySound(Game.Instance.SoundBank["Projectile"]);

                    if (Projectiles == null) {
                        Projectiles = new List<Bullet>();
                    }
                    PointF velocity = new PointF(0.0f, 0.0f);
                    if (faceLeft) {
                        velocity.X = -100.0f;
                    }
                    else {
                        velocity.X = 100.0f;
                    }
                    Projectiles.Add(new Bullet(Center, velocity));
                }
                //update projectiles
                if (Projectiles != null && Projectiles.Count > 0) {
                    for (int j = Projectiles.Count - 1; j >= 0; j--) {
                        float xPos = Projectiles[j].Position.X;
                        float yPos = Projectiles[j].Position.Y;
                        if (!Projectiles[j].InBounds()) {
                            Projectiles.RemoveAt(j);
                            continue;
                        }
                        Projectiles[j].Update(dTime);

                    }
                }
                //invicibility timer
                if (CurrentState == State.Invincible) {
                    invincibilityTime += dTime;
                    if (invincibilityTime > 15.0f) {
                        invincibilityTime -= 15.0f;
                        CurrentState = State.Normal;
                    }
                }
                Animate(dTime);

            }//end !dead
        }//end update
        protected void SetJump(float height,float duration) {
            Impulse = 2 * height / duration;
            Impulse *= -1;
            gravity = -Impulse / duration;
        }
        public void Win(float dTime) {
            if (Position.Y / Game.TILE_SIZE < 10) { //manually move down flag pole
                Position.Y += speed*dTime*2;
#if WINDEBUG
                Console.WriteLine("Hero Position.Y: "+Position.Y / Game.TILE_SIZE);
#endif
                //add animations
            }
            else {
                xMovement = true;
                
                if (Large && CurrentState == State.Fire) {
                    if (CurrentSprite == "LargeFireJump") {
                        SetSprite("LargeFireStand");
                    }
                }
                else if(Large && CurrentState == State.Invincible) {
                    if (CurrentSprite == "InvincibleLargeJump") {
                        SetSprite("InvincibleLargeStand");
                    }
                }
                else if (Large) {
                    if (CurrentSprite == "LargeJump") {
                        SetSprite("LargeStand");
                    }
                }
                else if (CurrentState == State.Fire) {
                    if (CurrentSprite == "FireJump") {
                        SetSprite("FireStand");
                    }
                }
                else if (CurrentState == State.Invincible) {
                    if (CurrentSprite == "InvincibleJump") {
                        SetSprite("InvincibleStand");
                    }
                }
                else {
                    if (CurrentSprite == "Jump") {
                        SetSprite("Stand");
                    }
                }
                Animate(dTime);
            }
            if (xMovement && Game.Instance.GetTile(BottomCorners[Character.CORNER_TOP_LEFT]).TileValue != 30) {
                if (CurrentSprite != "Run" || CurrentSprite != "LargeRun" || CurrentSprite != "LargeFireRun" || CurrentSprite != "FireRun" || CurrentSprite != "InvincibleRun" || CurrentSprite != "InvincibleLargeRun") {
#if WINDEBUG
                    Console.WriteLine("CurrentSprite: " + CurrentSprite);
                    Console.WriteLine("Sprite set to Run");
#endif
                    if (Large && CurrentState == State.Fire) {
                        SetSprite("LargeFireRun");
                    }
                    else if (Large && CurrentState == State.Invincible) {
                        SetSprite("InvincibleLargeRun");
                    }
                    else if (Large && CurrentState == State.Normal) {
                        SetSprite("LargeRun");
                    }
                    else if (CurrentState == State.Fire) {
                        SetSprite("FireRun");
                    }
                    else if (CurrentState == State.Invincible) {
                        SetSprite("InvincibleRun");
                    }
                    else if (CurrentState == State.Normal) {
                        SetSprite("Run");
                    }
                }
                Animate(dTime);
                Position.X += speed * dTime*2;
            }

        }
        public void Die(float dTime) {
            deathTimer += dTime;
            if (!isJumping) {
                Jump(Impulse*1.5f);
                SetSprite("Dead");
            }
            if (deathTimer > 1.20f) {
                deathTimer = 0.0f;
                Game.Instance.CurrentState = Game.State.Start;
                if (Lifes < 0) {
                    Game.Instance.Reset();
                }
            }
        }
        public void Jump(float impulse) {
            
            isJumping = true;
            velocity = impulse;
            if (CurrentState == State.Normal && Large) {
                SetSprite("LargeJump");
            }
            else if (CurrentState == State.Normal) {
                SetSprite("Jump");
            }
            else if (CurrentState == State.Fire && Large) {
                SetSprite("LargeFireJump");
            }
            else if (CurrentState == State.Fire) {
                SetSprite("FireJump");
            }
            else if (CurrentState == State.Invincible && Large) {
                SetSprite("InvincibleLargeJump");
            }
            else if (CurrentState == State.Invincible) {
                SetSprite("InvincibleJump");
            }
        }
        public void ChangeForm(string newForm) {
            if (newForm == "Large") {
                CurrentFrame = 0;
                Large = true;
            }
            else if (newForm == "Fire") {
                CurrentFrame = 0;
                CurrentState = State.Fire;
            }
            else if (newForm == "LargeFire") {
                CurrentFrame = 0;
                CurrentState = State.Fire;
                Large = true;
            }
            else if (newForm == "Invincible") {
                CurrentFrame = 0;
                CurrentState = State.Invincible;
            }
            else if (newForm == "LargeInvincible") {
                CurrentFrame = 0;
                CurrentState = State.Invincible;
                Large = true;
            }
        }
        public override void Render(PointF offsetPosition) {
            if (breakFX != null) {
                breakFX.Render(offsetPosition);
            }
            if (CurrentState == State.Normal) {
                base.Render(offsetPosition);
            }
            else {
                Point renderPosition = new Point((int)Position.X, (int)Position.Y);
                renderPosition.X -= (int)offsetPosition.X - 1;
                renderPosition.Y -= 1;
                Rectangle renderRect = SpriteSources[CurrentSprite][CurrentFrame];
                renderRect.X -= 1;
                renderRect.Y -= 1;
                if (!faceLeft) {
                    TextureManager.Instance.Draw(Sprite, renderPosition, 1.0f, renderRect);
                }
                else {
                    TextureManager.Instance.Draw(Sprite, new Point(renderPosition.X + renderRect.Width, renderPosition.Y), new Point(-1, 1), renderRect);
                }
                //render projectiles
                if (Projectiles != null && Projectiles.Count > 0) {
                    for (int i = 0; i < Projectiles.Count; i++) {
                        Projectiles[i].Render(offsetPosition);
                    }
                }
#if DEBUG
                /*
                //render corners
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
                */
#endif
            }
        }
    }
}
