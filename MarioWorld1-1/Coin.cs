using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Coin {
        protected int sprite = 0;
        public PointF Position = new PointF(0, 0);
        protected Dictionary<string, Rectangle[]> spriteSources = null;
        protected string CurrentSprite = null;
        protected int currentFrame = 0;
        private float animTimer = 0f;
        private float animFPS = 5.0f; //number of frames
        public Coin(string spritepath){
            sprite = TextureManager.Instance.LoadTexture(spritepath);
            AddSprite("Default",new Rectangle(432, 113, 16, 16), new Rectangle(450, 113, 16, 16), new Rectangle(432, 113, 16, 16), new Rectangle(464, 113, 16, 16), new Rectangle(480, 113, 16, 16));
        }
        public Coin Spawn(string spriteSheet) {
            return new Coin(spriteSheet);
        }
        public void Render() {
            TextureManager.Instance.Draw(sprite, new Point((int)Position.X,(int)Position.Y), 1.0f);
        }
        public void AddSprite(string name, params Rectangle[] source) {
            if (spriteSources == null) {
                spriteSources = new Dictionary<string, Rectangle[]>();
            }
            if (CurrentSprite == null) {
                CurrentSprite = name;
            }
            spriteSources.Add(name, source);
        }
        public void Destroy() {
            TextureManager.Instance.UnloadTexture(sprite);
        }
        public void Update(float dTime) { //same as animate function
            animTimer += dTime;
            if (animTimer > animFPS) {
                animTimer -= animFPS;
                currentFrame += 1;
                if (currentFrame > spriteSources[CurrentSprite].Length - 1) {
                    currentFrame = 0;
                }
            }

        }
    }
}
