using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Koopa  : EnemyCharacter{
        
        public Koopa(string spriteSheet, bool movingUpDown) : base(spriteSheet, movingUpDown) {
            AddSprite("Walk", new Rectangle(5, 100, 17, 22), new Rectangle(27, 100, 17, 22));
            AddSprite("Dead1", new Rectangle(52, 105, 16, 16));
            AddSprite("Dead2", new Rectangle(75, 110, 16, 16));
            SetSprite("Walk");
            moveUpDown = movingUpDown;
            //Rect.Height -= 6;
        }
        public void Update(float dTime) {
            /*Map.cs sets states*/

            //do regular stuff
            if (CurrentState == State.Alive) {
                base.Update(dTime);
            }
            //not moving at all.
            else if (CurrentState == State.Dead1) {

            }
            //moving death shell
            else if (CurrentState == State.Dead2) {

            }
        }
        public void Render(Point offsetPosition) {
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
        }
    }
}
