using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Item {
        protected int Sprite;
        protected Rectangle Source;
        public PointF Position;
        public Dictionary<string, Rectangle[]> SpriteSources = null;
        public string currentSprite { get; set; }
        protected int currentFrame = 0;
        protected float gravity = 150.0f; //same formula as in player character
        protected float speed = 75.0f;
        protected int direction = 1;

        float animFPS = 1.0f / 10.0f; //one sec / number of frames
        float animTimer = 0f;
        public static string ItemSheet = null;
        public bool IsSpawned = false;
        public Rectangle Rect {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, SpriteSources[currentSprite][currentFrame].Width-1, SpriteSources[currentSprite][currentFrame].Height-1);
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
        public Item(string spriteSheet) {
            Sprite = TextureManager.Instance.LoadTexture(ItemSheet);
        }
        public void Render(PointF offsetPosition) {
            Point renderPosition = new Point((int)Position.X, (int)Position.Y);
            renderPosition.X -= (int)offsetPosition.X;
            renderPosition.Y -= (int)offsetPosition.Y;
            Rectangle renderRect = SpriteSources[currentSprite][currentFrame];
            renderRect.X -= 1;
            renderRect.Y -= 1;
            TextureManager.Instance.Draw(Sprite, renderPosition, 1.0f, renderRect);
        }
        public static Item SpawnItem(string itemType) {
            if (itemType == "GrowMushroom") {
                return new GrowMushroom(ItemSheet);
            }
            if (itemType == "FireFlower") {
                return new FireFlower(ItemSheet);
            }
            if (itemType == "Star") {
                return new Star(ItemSheet);
            }
            Console.WriteLine("Item not assigned to SpawnItem Function in Item.cs");
            return null;
        }
        public void Destroy() {
            TextureManager.Instance.UnloadTexture(Sprite);
        }
        public void AddSprite(string name, params Rectangle[] source) {
            if (SpriteSources == null) {
                SpriteSources = new Dictionary<string, Rectangle[]>();
            }
            if (currentSprite == null) {
                currentSprite = name;
            }
            SpriteSources.Add(name, source);
        }
        public virtual void Update(float dTime) {
            ApplyGravity(dTime);
            HorizontalMovement(dTime);
            
            //Floor collision
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_LEFT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_LEFT]));
                if (intersection.Width*intersection.Height > 0) {
                    Position.Y = intersection.Top - Rect.Height;
                }
            }
            if (!Game.Instance.GetTile(Corners[CORNER_BOTTOM_RIGHT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_BOTTOM_RIGHT]));
                if (intersection.Width * intersection.Height > 0) {
                    Position.Y = intersection.Top - Rect.Height;
                }
            }
        }
        protected void Animate(float dTime) {
            animTimer += dTime;
            if (animTimer > animFPS) {
                animTimer -= animFPS;
                currentFrame++;
                if (currentFrame > SpriteSources[currentSprite].Length - 1) {
                    currentFrame = 0;
                }
            }
        }
        protected virtual void ApplyGravity(float dTime) {
            Position.Y += gravity * dTime;
        }
        protected virtual void HorizontalMovement(float dTime) {
            Position.X += speed * dTime*direction;
            //right side collision
            if (!Game.Instance.GetTile(Corners[CORNER_TOP_RIGHT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_RIGHT]));
                if (intersection.Width * intersection.Height > 0) {
                    direction *= -1;
                }
            }
            
            //left side collision
            if (!Game.Instance.GetTile(Corners[CORNER_TOP_LEFT]).Walkable) {
                Rectangle intersection = Intersections.Rect(Rect, Game.Instance.GetTileRect(Corners[CORNER_TOP_LEFT]));
                if (intersection.Width * intersection.Height > 0) {
                    direction *= -1;
                }
            }
            
        }

    }
}
