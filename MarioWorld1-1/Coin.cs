using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Coin : Item{
        protected int sprite = 0;
        protected Dictionary<string, Rectangle[]> spriteSources = null;
        protected string CurrentSprite = null;
        private float animTimer = 0f;
        private float animFPS = 5.0f; //number of frames

        public Coin(string spritepath) : base(spritepath){
            sprite = TextureManager.Instance.LoadTexture(spritepath);
            AddSprite("Default",new Rectangle(432, 113, 16, 16), new Rectangle(450, 113, 16, 16), new Rectangle(432, 113, 16, 16), new Rectangle(464, 113, 16, 16), new Rectangle(480, 113, 16, 16));
        }
        public Coin Spawn(string spriteSheet) {
            return new Coin(spriteSheet);
        }
        public override void Update(float dTime) { //same as animate function
            Animate(dTime);
            animTimer += dTime;
            if (animTimer > animFPS) {
                animTimer -= animFPS;
                currentFrame += 1;
                if (currentFrame > spriteSources[CurrentSprite].Length - 1) {
                    currentFrame = 0;
                }
            }
            Position.Y -= speed * dTime;
            float dY = StartPos.Y - Position.Y / Game.TILE_SIZE; //determines how many tiles it will rise
            if (dY > 2.0f) {
                //Destroy();
                //how to remove from item list?
            }
        }
    }
}
