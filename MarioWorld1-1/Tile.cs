﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GameFramework;

namespace MarioWorld1_1 {
    class Tile {
        public string DoorPath = null;
        public bool IsDoor { get; set; }
        public int Sprite { get; private set; }
        public Rectangle Source { get; private set; }
        public bool Walkable { get; set; }
        public Point WorldPosition { get; set; }
        public float Scale { get; set; }
        public bool Breakable { get; set; }
        public int TileValue { get; set; }
        public float YOffset = 0.0f;
        public string Item { get; set; }

        public Tile(string spritePath, Rectangle source) {
            Sprite = TextureManager.Instance.LoadTexture(spritePath);
            Source = source;
            Scale = 1.0f;
            Walkable = false;
            WorldPosition = new Point(0, 0);
        }
        public void Update(float dTime) {
            if (YOffset > 0) {
                YOffset -= dTime * 9.0f; //9 is arbritrary that looks good
                if (YOffset < 0.0f) {
                    YOffset = 0.0f;
                }
            }
        }
        public void Render(PointF offsetPosition) {
            Point renderPos = new Point(WorldPosition.X, WorldPosition.Y-(int)YOffset);
            renderPos.X = (int)(Scale * renderPos.X);
            renderPos.Y = (int)(Scale * renderPos.Y);
            renderPos.X -= (int)offsetPosition.X;
            TextureManager.Instance.Draw(Sprite, renderPos, Scale, Source);
        }
        public void Destroy() {
            TextureManager.Instance.UnloadTexture(Sprite);
        }
    }
}
