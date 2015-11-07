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
            AddSprite("Dead1", new Rectangle(52, 105, 16, 16));
            AddSprite("Dead2", new Rectangle(75, 110, 16, 16));
            SetSprite("Walk");
            moveUpDown = movingUpDown;
        }
        public override void Render(PointF offsetPosition) {
            Rectangle visual = SpriteSources[CurrentSprite][CurrentFrame];
            visual.X = (int)Position.X - ((int)offsetPosition.X - 1);
            visual.Y = (int)Position.Y - 1;
            //GraphicsManager.Instance.DrawRect(visual, Color.Red);
            Point renderPosition = new Point((int)Position.X, (int)Position.Y);
            renderPosition.X -= (int)offsetPosition.X - 1;
            renderPosition.Y -= 1;

            Rectangle collision = Rect;
            //Construct collision from corners
            collision.X = (int)Corners[CORNER_TOP_LEFT].X;
            collision.Y = (int)Corners[CORNER_TOP_LEFT].Y;
            collision.Width = (int)(Corners[CORNER_BOTTOM_RIGHT].X - Corners[CORNER_BOTTOM_LEFT].X);
            collision.Height = (int)(Corners[CORNER_BOTTOM_RIGHT].Y - Corners[CORNER_TOP_LEFT].Y);
            //Move into screen space
            collision.X -= (int)offsetPosition.X;
            //GraphicsManager.Instance.DrawRect(collision, Color.Blue);
            if (!faceLeft) {
                TextureManager.Instance.Draw(Sprite, renderPosition, 1.0f, visual);
            }
            else {
                TextureManager.Instance.Draw(Sprite, new Point(renderPosition.X + visual.Width, renderPosition.Y), new Point(-1, 1), visual);
            }
        }
    }
}
