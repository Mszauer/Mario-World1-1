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
        protected List<Rectangle[]> spriteSources { get; private set; }
        protected int currentFrame = 0;
        public Point Position = new Point(0, 0);
        public BreakEffect(string spritePath,Point position) {
            sprite = TextureManager.Instance.LoadTexture(spritePath);
            AddSprite(new Rectangle(0, 0, 48, 48), new Rectangle(48, 0, 48, 48), new Rectangle(144, 0, 48, 48), new Rectangle(0, 48, 48, 48),new Rectangle(48,48,48,48));
            Position = position;
        }
        protected void AddSprite(params Rectangle[] source) {
            if (spriteSources == null) {
                spriteSources = new List<Rectangle[]>();
            }
            spriteSources.Add(source);
        }
        public bool Animate(float dTime) {
            currentFrame += 1;
            if (currentFrame > spriteSources.Count - 1) {
                Destroy();
                currentFrame = 0;
                return true;
            }
            return false;
        }
        public void Render() {
            Point renderPos = new Point(Position.X, Position.Y);
            renderPos.X -= Game.TILE_SIZE;
            TextureManager.Instance.Draw(sprite, renderPos);
        }
        protected void Destroy() {
            TextureManager.Instance.UnloadTexture(sprite);
        }
    }
}
