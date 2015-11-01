using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Star : Item {
        public int ItemValue = 0;
        public Star(string spriteSheet) : base(spriteSheet) {
            AddSprite("Default", new Rectangle(0, 50, 16, 16), new Rectangle(16, 50, 16, 16), new Rectangle(32, 50, 16, 16), new Rectangle(48, 50, 16, 16));
            currentSprite = "Default";
        }
        public override void Update(float dTime) {
            Animate(dTime);
        }
    }
}
