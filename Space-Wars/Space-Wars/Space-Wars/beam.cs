using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace WindowsGame2
{
    class beam
    {
        //All esential varibles
        public Texture2D Tex;
        public Vector2 Pos;
        public bool Enabled;
        public int Damage;
        public Rectangle E;
        bool C1;

        Viewport viewport;

        public int Width
        {
            get { return Tex.Width; }
        }

        public int Height
        {
            get { return Tex.Height; }
        }

        float beamSpeed;

        public void init(Viewport view, Texture2D tex, Vector2 pos, Rectangle e, bool C)
        {
            //Initialize the varibles
            Tex = tex;
            E = e;
            Pos = pos;
            C1 = C;
            this.viewport = view;

            Enabled = true;
            Damage = 2;
            beamSpeed = 20f;
        }
        public void Update()
        {
            //Move the beam and disible it when it reaches the top of screen
            Pos.Y -= beamSpeed;

                if (C1 == true)
                    if (Pos.Y + Tex.Height < E.Bottom - 150)
                        Enabled = false;

                if (C1 == false)
                    if (Pos.Y + Tex.Height < 0)
                        Enabled = false;

        }
        //Draw everything
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Tex, Pos, null, Color.White, 0f, new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 1);
        }
    }
}
