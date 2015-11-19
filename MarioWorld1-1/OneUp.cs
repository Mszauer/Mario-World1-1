using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class OneUp : Item{
        public OneUp(string spriteSheet) : base(spriteSheet) {
            speed = speed / 2;
            AddSprite("Default", new Rectangle(16, 16, 16, 16));
            currentSprite = "Default";
        }
    }
}
