using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using System.Drawing;

namespace MarioWorld1_1 {
    class PlayerCharacter : Character{
        public float speed = 90.0f;
        protected float impulse = 0.0f;
        protected float velocity = 0.0f;
        protected float gravity = 0 / 0f;
        public PlayerCharacter(string spritePath) : base(spritePath) {
            AddSprite("Right", new Rectangle(12, 6, 12, 18));
            SetSprite("Right");
        }
        public void Render(/*offset here*/) {
            Point renderPosition = new Point((int)Position.X, (int)Position.Y);
            //offset position stuff
            //GraphicsManager.Instance.DrawRect(new Rectangle(renderPosition.X,renderPosition.Y,SpriteSources[currentSprite][currentFrame].Width, SpriteSources[currentSprite][currentFrame].Height), Color.Red);
            TextureManager.Instance.Draw(Sprite, renderPosition, 1.0f, SpriteSources[currentSprite][currentFrame]);
        }
        public void Update(float dTime) {

        }
    }
}
