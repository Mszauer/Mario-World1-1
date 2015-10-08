using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using System.Drawing;

namespace MarioWorld1_1 {
    class PlayerCharacter : Character{
        public float speed = 90.0f;
        protected float impulse = 0.0f;
        protected float velocity = 0.0f;
        protected float gravity = 0 / 0f;
        public PlayerCharacter(string spritePath) : base(spritePath) {

        }
    }
}
