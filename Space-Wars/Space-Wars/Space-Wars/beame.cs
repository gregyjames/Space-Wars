using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace WindowsGame2
{
    class beam1
    {
        //All essential varibles
        public Texture2D Tex1;
        public Vector2 Pos1;
        public bool Enabled1;
        public int Damage1;
        Viewport viewport1;
        public Rectangle E1;
        bool colide;

        public int Width1
        {
            get { return Tex1.Width; }
        }

        public int Height1
        {
            get { return Tex1.Height; }
        }

        float beamSpeed1;

        //Initialize varibles
        public void init(Viewport view, Texture2D tex, Vector2 pos, Rectangle e1, bool c)
        {
            Tex1 = tex;
            colide = c;
            Pos1 = pos;
            E1 = e1;
            this.viewport1 = view;

            Enabled1 = true;
            Damage1 = 2;
            beamSpeed1 = 20f;
        }
        //Move the beams and disable them on impact with the player
        public void Update()
        {
            Pos1.Y += beamSpeed1;

            if(colide == true)
                    Enabled1 = false;

            if(colide == false)
                if (Pos1.Y + Tex1.Height > 1000)
                    Enabled1 = false;
            
        }

        //Draw everrything
        public void Draw(SpriteBatch sb1)
        {
            sb1.Draw(Tex1, Pos1, null, Color.White, 0f, new Vector2(Width1 / 2, Height1), 1f, SpriteEffects.None, 1);
        }
    }
}
