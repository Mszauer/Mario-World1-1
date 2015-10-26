using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MarioWorld1_1 {
    class FireFlower : Item{
        public FireFlower(string spriteSheet) : base(spriteSheet) {
            AddSprite("Default", new Rectangle(0, 33, 16, 16), new Rectangle(16, 33, 16, 16), new Rectangle(32, 33, 16, 16), new Rectangle(48, 33, 16, 16));
            currentSprite = "Default";
        }
        public override void Update(float dTime) {

        }
    }
}
