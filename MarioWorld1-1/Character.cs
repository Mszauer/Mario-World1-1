using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using System.Drawing;

namespace MarioWorld1_1 {
    class Character {
        public int Sprite { get; set; }
        public PointF Position = new PointF(0.0f, 0.0f);
        public Dictionary<string, Rectangle[]> SpriteSources { get; private set; }
        public string CurrentSprite { get; set; }
        public int CurrentFrame = 0;
        protected float animFPS = 1.0f / 3.0f; //one sec / number of frames
        protected float animTimer = 0f;
        protected bool faceLeft = false;
        public List<Bullet> Projectiles = null;
        public Rectangle Rect {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, SpriteSources[CurrentSprite][CurrentFrame].Width-1, SpriteSources[CurrentSprite][CurrentFrame].Height-1);
            }
        }
        public PointF Center {
            get {
                return new PointF(Rect.X + (Rect.Width / 2), Rect.Y + (Rect.Height / 2));
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
        protected Character(string spritePath) {
            Sprite = TextureManager.Instance.LoadTexture(spritePath);
        }
        public virtual void Render(PointF offsetPosition) {
            Point renderPosition = new Point((int)Position.X, (int)Position.Y);
            renderPosition.X -= (int)offsetPosition.X-1;
            renderPosition.Y -= 1;
            Rectangle renderRect = SpriteSources[CurrentSprite][CurrentFrame];
            renderRect.X -= 1;
            renderRect.Y -= 1;
            if (!faceLeft) {
                TextureManager.Instance.Draw(Sprite, renderPosition, 1.0f, renderRect);
            }
            else {
                TextureManager.Instance.Draw(Sprite, new Point(renderPosition.X + renderRect.Width,renderPosition.Y), new Point(-1,1), renderRect);
            }
        }
        public void Destroy() {
            //unload texture
            TextureManager.Instance.UnloadTexture(Sprite);
            //remove bullets
            if (Projectiles != null) {
                for (int i = Projectiles.Count - 1; i >= 0; i--) {
                    Projectiles.RemoveAt(i);
                }
            }
        }
        public void SetSprite(string name) {
            if (SpriteSources.ContainsKey(name)) {
                if (CurrentSprite != name) {
                    CurrentSprite = name;
                    CurrentFrame = 0;
                }
            }
            else {
#if DEBUG
                Console.WriteLine("Texture not found: " + name);
#endif
            }
        }
        public void AddSprite(string name, params Rectangle[] source) {
            if (SpriteSources == null) {
                SpriteSources = new Dictionary<string, Rectangle[]>();
            }
            if (CurrentSprite == null) {
                CurrentSprite = name;
            }
            SpriteSources.Add(name, source);
        }
        protected void Animate(float dTime) {
            animTimer += dTime;
            if (animTimer > animFPS) {
                animTimer -= animFPS;
                CurrentFrame += 1;
                if (CurrentFrame > SpriteSources[CurrentSprite].Length - 1) {
                    CurrentFrame = 0;
                }
            }
        }
    }
}
