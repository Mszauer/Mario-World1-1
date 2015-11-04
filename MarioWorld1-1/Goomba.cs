using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MarioWorld1_1 {
    class Goomba : EnemyCharacter{
        public Goomba(string spritePath, bool movingUpDown) : base(spritePath, movingUpDown) {
            AddSprite("Walk", new Rectangle(1, 4, 16, 16), new Rectangle(22, 4, 16, 16));
            AddSprite("Death", new Rectangle(44, 4, 16, 16));
            SetSprite("Walk");
            moveUpDown = movingUpDown;
        }
    }
}
