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
        public string currentSprite { get; set; }
        public int currentFrame = 0;
        float animFPS = 1.0f / 3.0f; //one sec / number of frames
        float animTimer = 0f;
        public Rectangle Rect {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, SpriteSources[currentSprite][currentFrame].Width, SpriteSources[currentSprite][currentFrame].Height);
            }
        }
        public PointF Center {
            get {
                return new PointF(Rect.X + (Rect.Width / 2), Rect.Y + (Rect.Height / 2));
            }
        }
        public PointF[] Corners {
            get {
                float w = SpriteSources[currentSprite][currentFrame].Width;
                float h = SpriteSources[currentSprite][currentFrame].Height;
                return new PointF[] {
                    new PointF(Position.X,Position.Y),
                    new PointF(Position.X+w,Position.Y),
                    new PointF(Position.X,Position.Y+h),
                    new PointF(Position.X+w,Position.Y+h)
                };
            }
        }
        public static readonly int CORNER_TOP_LEFT = 0;
        public static readonly int CORNER_TOP_RIGHT = 1;
        public static readonly int CORNER_BOTTOM_LEFT = 2;
        public static readonly int CORNER_BOTTOM_RIGHT = 3;
        public Character(string spritePath) {
            Sprite = TextureManager.Instance.LoadTexture(spritePath);
        }
        public void Render(/*offset*/) {
            //create offset then add
            TextureManager.Instance.Draw(Sprite, new Point((int)Position.X, (int)Position.Y));
        }
        public void Destroy() {
            TextureManager.Instance.UnloadTexture(Sprite);
        }
        public void SetSprite(string name) {
            if (SpriteSources.ContainsKey(name)) {
                currentSprite = name;
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
            if (currentSprite == null) {
                currentSprite = name;
            }
            SpriteSources.Add(name, source);
        }
        protected void Animate(float dTime) {
            animTimer += dTime;
            if (animTimer > animFPS) {
                animTimer -= animFPS;
                currentFrame += 1;
                if (currentFrame > SpriteSources[currentSprite].Length - 1) {
                    currentFrame = 0;
                }
            }
        }
    }
}
