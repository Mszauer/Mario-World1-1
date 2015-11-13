using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Koopa  : EnemyCharacter{
        public override Rectangle Rect {
            get {
                Rectangle result = base.Rect;
                result.Width = result.Height = 15;
                return result;
            }
        }
        public Koopa(string spriteSheet, bool movingUpDown) : base(spriteSheet, movingUpDown) {
            AddSprite("Walk", new Rectangle(5, 100, 17, 22), new Rectangle(27, 100, 17, 22));
            //AddSprite("Dead1", new Rectangle(52, 105, 16, 16));
            AddSprite("Dead", new Rectangle(75, 110, 16, 16));
            SetSprite("Walk");
            moveUpDown = movingUpDown;
        }
        public override void Update(float dTime) {
            if (CurrentState == State.Alive) {
                SetSprite("Walk");
                base.Update(dTime);
            }
            else if (CurrentState == State.Dead1) {
                SetSprite("Dead");
            }
            else if (CurrentState == State.Dead2) {
                SetSprite("Dead");
                base.Update(dTime);
            }
        }
        public override void Render(PointF offsetPosition) {
            //base.Render(offsetPosition);
            Rectangle visual = SpriteSources[CurrentSprite][CurrentFrame];
            //x is moved into screen space
            visual.X = (int)Position.X - ((int)offsetPosition.X - 1);
            //y is already in screen space, offset it by difference in sprite height / collision height to allign with ground
            visual.Y = (int)Position.Y - (SpriteSources[CurrentSprite][CurrentFrame].Height - Rect.Height) + 2;
            GraphicsManager.Instance.DrawRect(visual, Color.Red);

            Rectangle collision = Rect;
            //Construct collision from corners
            collision.X = (int)Corners[CORNER_TOP_LEFT].X + 1;
            collision.Y = (int)Corners[CORNER_TOP_LEFT].Y + 2;
            collision.Width = (int)(Corners[CORNER_BOTTOM_RIGHT].X - Corners[CORNER_BOTTOM_LEFT].X);
            collision.Height = (int)(Corners[CORNER_BOTTOM_RIGHT].Y - Corners[CORNER_TOP_LEFT].Y);
            //Move into screen space
            collision.X -= (int)offsetPosition.X;
            //GraphicsManager.Instance.DrawRect(collision, Color.Blue);
            if (!faceLeft) {
                TextureManager.Instance.Draw(Sprite, new Point(visual.X, visual.Y), 1.0f, SpriteSources[CurrentSprite][CurrentFrame]);
            }
            else {
                TextureManager.Instance.Draw(Sprite, new Point(visual.X+visual.Width, visual.Y), new Point(-1,1), SpriteSources[CurrentSprite][CurrentFrame]);

            }
        }
    }
}
