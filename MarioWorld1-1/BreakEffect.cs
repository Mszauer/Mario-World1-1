using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class BreakEffect {
        protected int sprite = 0;
        protected List<Rectangle> frames = null;
        protected int currentFrame = 0;
        public Point Position = new Point(0, 0);
        public BreakEffect(string spritePath,Point position) {
            sprite = TextureManager.Instance.LoadTexture(spritePath);
            AddSprite(new Rectangle(0, 0, 48, 48), new Rectangle(48, 0, 48, 48), new Rectangle(144, 0, 48, 48), new Rectangle(0, 48, 48, 48),new Rectangle(48,48,48,48));
            Position = position;
        }
        protected void AddSprite(params Rectangle[] source) {
            if (frames == null) {
                frames = new List<Rectangle>();
            }
            frames.AddRange(source);
        }
        public void Animate(float dTime) {
            currentFrame += 1;
            if (currentFrame > frames.Count - 1) {
                Destroy();
                currentFrame = 0;
                //return true;
            }
            //return false;
        }
        public void Render(PointF offsetPosition) {
            Point renderPos = new Point(Position.X, Position.Y);
            renderPos.X -= Game.TILE_SIZE;
            renderPos.X -= (int)offsetPosition.X;
            TextureManager.Instance.Draw(sprite, renderPos,1.0f,frames[currentFrame]);
        }
        protected void Destroy() {
            TextureManager.Instance.UnloadTexture(sprite);
        }
    }
}
