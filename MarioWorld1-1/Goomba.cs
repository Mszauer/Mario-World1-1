using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MarioWorld1_1 {
    class Goomba : EnemyCharacter{
        public Goomba(string spritePath, bool movingUpDown) : base(spritePath, movingUpDown) {
            AddSprite("Walk", new Rectangle(2, 4, 16, 16), new Rectangle(25, 4, 16, 16));
            AddSprite("Dead", new Rectangle(44, 4, 16, 16));
            SetSprite("Walk");
            moveUpDown = movingUpDown;
        }
    }
}
