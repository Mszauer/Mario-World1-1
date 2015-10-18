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
        float animFPS = 1.0f / 3.0f; //one sec / number of frames
        float animTimer = 0f;
        public Rectangle Rect {
            get {
                return new Rectangle((int)Position.X, (int)Position.Y, SpriteSources[currentSprite][currentFrame].Width, SpriteSources[currentSprite][currentFrame].Height);
            }
        }
        public Item(string spriteSheet) {
            Sprite = TextureManager.Instance.LoadTexture(spriteSheet);
        }
        public void Render(PointF offsetPosition) {
            Point renderPosition = new Point((int)Position.X, (int)Position.Y);
            renderPosition.X -= (int)offsetPosition.X;
            renderPosition.Y -= (int)offsetPosition.Y;
            TextureManager.Instance.Draw(Sprite, renderPosition, 1.0f, Source);
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
    }
}
