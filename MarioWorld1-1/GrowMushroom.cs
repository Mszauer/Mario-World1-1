using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class GrowMushroom : Item{
        public int Value = 0;
        public GrowMushroom(string spriteSheet) : base(spriteSheet) {
            AddSprite("Default", new Rectangle(0, 0, 16, 16));
            currentSprite = "Default";
        }
    }
}
