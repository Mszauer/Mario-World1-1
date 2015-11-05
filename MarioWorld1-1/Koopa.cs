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
    }
}
