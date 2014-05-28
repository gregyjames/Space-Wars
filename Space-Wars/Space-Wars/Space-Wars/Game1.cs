using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame2
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //Shoutout to TOMAZ MAJ for the powerups. Your the real MVP. 

        #region varibles
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background;

        //Code for the Beam and The Ship/enemies
        Rectangle bgrect;
        bool sideBounce = true, colide = false, colide1 = false;
        float dwidth, dheight;
        StructGame Ship, enemies;
        List<beam> beams;
        List<beam1> beams1;
	    KeyboardState oldState;
        Texture2D intro, beamTex;
        SpriteFont Font;
        int damage = 5;
        string screen = "title";
        
        //Levels, score, lives, enemy health
        int score, levels, lives = 3;
        int ehealth;

        //Soundeffects
        SoundEffect Pfire, expsound;
        Song bgMusic;

        

        //Rates of fire for player/Enemy beams
        TimeSpan FT;
        TimeSpan PFT;
        TimeSpan FT1;
        TimeSpan PFT1;

        //Player life textures
        Texture2D HeartFull, HeartEmpty;

        //Code for explosion
        Rectangle ExplosionOrigin, ExplosionSprite;
        int spriteWidth, spriteHeight;
        Texture2D Explosion;
        int SpriteRow;
        int SpriteColumn;
        float timer;
        float interval = 50f;
        Boolean Explode = false;

        //Level code
        bool move = true;
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(850);
        bool NextScene = false;

        //Powerups
        PowerStrux breakWepon;
        PowerStrux[] breakDrop;

        LifeStrux lifeSprite;
        LifeStrux[] lifeDrop;

        int breakdroptimer = 0;
        int lifedroptimer = 0;
        int breaktime = 0;

        bool WeponDisable = false;

        struct PowerStrux
        {
            public Texture2D PowerPic;
            public Rectangle PowerRect;
            public float X;
            public float Y;
            public bool Visible;
        }

        struct LifeStrux

        {
            public Texture2D PowerPic;
            public Rectangle PowerRect;
            public float X;
            public float Y;
            public bool Visible;
        }

        struct StructGame
        {
            public Texture2D SpriteTexture;
            public Rectangle SpriteRectangle;
            public float X;
            public float Y;
            public float XSpeed;
            public float YSpeed;
            public float WF;
            public float TTCS;
            public bool Visibility;
        }

#endregion

        //Code to setup Player and Enemies (stolen from bread and cheese)
        #region Player and Enemies
        void setUpSpritesX(ref StructGame sprite, float wF, float tTCS, float initX, float initY, bool initV)
        {
            sprite.WF = wF;

            sprite.TTCS = tTCS;

            sprite.SpriteRectangle.Width = (int)((dwidth * wF) + 0.5f);
            float aspectRatio = (float)sprite.SpriteRectangle.Width / sprite.SpriteTexture.Height;

            sprite.SpriteRectangle.Height = (int)((sprite.SpriteRectangle.Width / aspectRatio) + 0.5f);
            sprite.X = initX;
            sprite.SpriteRectangle.X = Convert.ToInt32(initX);
            
            sprite.Y = initY;
            sprite.SpriteRectangle.Y = Convert.ToInt32(initY);
            sprite.XSpeed = dwidth / tTCS;

            sprite.YSpeed = sprite.XSpeed;
            sprite.Visibility = initV;
        }

        void SetUpSprites()
        {
            setUpSpritesX(ref Ship, 0.15f, 200.0f, 275, 350, true);

            setUpSpritesX(ref enemies, 0.25f, 200.0f, 245, 20, true);

            //float eS = (dwidth / numberOfEnemies);

            //for (int i = 0; i < numberOfEnemies; i++)
            //{
            //    enemies[i].SpriteTexture = EnemiesTexture;
            //    setUpSpritesX(ref enemies[i], 0.5f, 1000, 10 + (i * eS), 10, true);
            //}
        }

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        
        protected override void Initialize()
        {
            //Set window size
            graphics.PreferredBackBufferWidth = 650;
            graphics.PreferredBackBufferHeight = 550;
            graphics.ApplyChanges();

            //Set default values
            score = 0;
            levels = 1;
            lives = 3;
            ehealth = 100;

            dwidth = GraphicsDevice.Viewport.Width;
            dheight = GraphicsDevice.Viewport.Height;

            //Make the list for player and enemie beams
            beams = new List<beam>();

            FT = TimeSpan.FromSeconds(.2f);

            beams1 = new List<beam1>();

            FT1 = TimeSpan.FromSeconds(.55f);

            bgrect = new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            SpriteColumn = 1;

            SoundEffect.MasterVolume = 0.25f;
            MediaPlayer.Volume = 1.0f;

            //Number of powerups
            breakDrop = new PowerStrux[10];
            lifeDrop = new LifeStrux[10];

            base.Initialize();
        }

        
        protected override void LoadContent()
        {
            //Load all necessary content
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("bg");
            Ship.SpriteTexture = Content.Load<Texture2D>("spaceship");
            enemies.SpriteTexture = Content.Load<Texture2D>("Untitled-2");
            beamTex = Content.Load<Texture2D>("beam");
            Font = Content.Load<SpriteFont>("SpriteFont1");
            HeartFull = Content.Load<Texture2D>("heartfull");
            HeartEmpty = Content.Load<Texture2D>("heartempty");
            intro = Content.Load<Texture2D>("intro");
            Pfire = Content.Load<SoundEffect>("photon-gun-burst");
            expsound = Content.Load<SoundEffect>("Explosion_Ultra");
            Explosion = Content.Load<Texture2D>("spritesheet11");
            spriteWidth = Explosion.Width / 8;
            spriteHeight = Explosion.Height / 9;

            breakWepon.PowerPic = Content.Load<Texture2D>("1w5nb");
            breakSetup(ref breakWepon, 50, 50, false);

            lifeSprite.PowerPic = Content.Load<Texture2D>("lifeup");
            lifeSetup(ref lifeSprite, 0, 0, false);

            bgMusic = Content.Load<Song>("Space Trip");
            SetUpSprites();
        }

        
        protected override void UnloadContent()
        {
            
            
            
        }

        //Break powerup setup
        void breakSetup(ref PowerStrux greg, float initialX, float initialY, bool initVisibiality)
        {
            for (int i = 0; i < 5; i++)
            {
                //Set Picture, inital x/y, width, height, visibility
                breakDrop[i].PowerPic = greg.PowerPic;
                breakDrop[i].X = initialY;
                breakDrop[i].Y = initialY;
                breakDrop[i].PowerRect.Width = greg.PowerPic.Width;
                breakDrop[i].PowerRect.Height = greg.PowerPic.Height;
                breakDrop[i].Visible = initVisibiality;
            }
        }

        //Life drop code setup
        void lifeSetup(ref LifeStrux life, float initialX, float initialY, bool initVisibiality)
        {
            for (int i = 0; i < 5; i++)
            {
                lifeDrop[i].PowerPic = life.PowerPic;
                lifeDrop[i].X = initialY;
                lifeDrop[i].Y = initialY;
                lifeDrop[i].PowerRect.Width = life.PowerPic.Width;
                lifeDrop[i].PowerRect.Height = life.PowerPic.Height;
                lifeDrop[i].Visible = initVisibiality;
            }
        }

        //Drop the break 
        void dropbreak()
        {
            int Breakdroplocation;
            //Random location between the width
            Random r = new Random(678778979);
            Breakdroplocation = r.Next(0, (int)dwidth);
            for (int i = 0; i < 10; i++)
            {
                if (breakDrop[i].Visible == false)
                {
                    breakDrop[i].X = Breakdroplocation;
                    breakDrop[i].Y = 0;
                    breakDrop[i].Visible = true;

                    break;
                }
            }
        }

        //Drop the life
        void droplife()
        {
            int lifedroplocation;
            Random r1 = new Random(567578545);
            lifedroplocation = r1.Next(0, (int)dwidth);
            for (int i = 0; i < 10; i++)
            {
                if (lifeDrop[i].Visible == false)
                {
                    lifeDrop[i].X = lifedroplocation;
                    lifeDrop[i].Y = 0;
                    lifeDrop[i].Visible = true;

                    break;
                }
            }
        }

        //Add beams to the beam lists (player)
        private void AddBeam(Vector2 pos)
        {
            beam Beam = new beam();
            Beam.init(GraphicsDevice.Viewport, beamTex, pos, enemies.SpriteRectangle, colide1);
            colide1 = false;
            beams.Add(Beam);
            Pfire.Play();
        }

        //Add beams to the beam lists (enemies)
        private void AddBeam1(Vector2 pos1)
        {
            beam1 Beam1 = new beam1();
            Beam1.init(GraphicsDevice.Viewport, beamTex, pos1, Ship.SpriteRectangle, colide);
            colide = false;
            beams1.Add(Beam1);
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (move == true)
            {
                //Key pad and Game pad
                KeyboardState keys = Keyboard.GetState();
                GamePadState pad = GamePad.GetState(PlayerIndex.One);

                //Move Right
                if (keys.IsKeyDown(Keys.Right) || pad.DPad.Right == ButtonState.Pressed) 
                {
                    Ship.XSpeed = 6;
                    Ship.SpriteRectangle.X += Convert.ToInt32(Ship.XSpeed);
                }

                //Move Left
                if (keys.IsKeyDown(Keys.Left) || pad.DPad.Left == ButtonState.Pressed)
                {
                    Ship.XSpeed = -6;
                    Ship.SpriteRectangle.X += Convert.ToInt32(Ship.XSpeed);
                }

                //Limit Movement
                if (Ship.SpriteRectangle.X >= GraphicsDevice.Viewport.Width - Ship.SpriteRectangle.Width)
                {
                    Ship.SpriteRectangle.X = GraphicsDevice.Viewport.Width - Ship.SpriteRectangle.Width;
                }
                if (Ship.SpriteRectangle.X <= 0)
                {
                    Ship.SpriteRectangle.X = 0;
                }

                
                int xpeeeed = 5;

                //Enemy movement
                if (sideBounce == true)
                {
                    if (enemies.SpriteRectangle.X >= GraphicsDevice.Viewport.Width - enemies.SpriteRectangle.Width)
                    {
                        sideBounce = false;
                    }
                    else
                    {

                        enemies.SpriteRectangle.X += xpeeeed;
                    }
                }

                else if (sideBounce == false)
                {
                    if (enemies.SpriteRectangle.X <= 0)
                    {
                        sideBounce = true;
                    }
                    else
                    {
                        enemies.SpriteRectangle.X -= xpeeeed;
                    }
                }

                int shipX, shipY;

                shipX = Ship.SpriteRectangle.X;
                shipY = Ship.SpriteRectangle.Y;

                oldState = Keyboard.GetState();

                //Add beams to screen when space bar is pressed
                if (oldState.IsKeyDown(Keys.Space) || pad.Buttons.A == ButtonState.Pressed)
                {
                    if (gameTime.TotalGameTime - PFT > FT)
                    {
                        PFT = gameTime.TotalGameTime;
                        AddBeam(new Vector2(shipX + 50, shipY));
                    }
                }

                //Auto add beams to screen (for enemies)
                int enemyX, enemyY;

                enemyX = enemies.SpriteRectangle.X;
                enemyY = enemies.SpriteRectangle.Y;

                if (gameTime.TotalGameTime - PFT1 > FT1)
                {
                    PFT1 = gameTime.TotalGameTime;
                    AddBeam1(new Vector2(enemyX + 50, enemyY + 50));
                }

                UpdateBeams();
                UpdateBeams1();


            }

            else
            {
                MediaPlayer.Stop();
            }

            //Code for explode animation
            if (Explode == true)
            {
                //Timer
                timer += gameTime.ElapsedGameTime.Milliseconds;
                if (timer >= interval)
                {
                    //End animation
                    timer = 0;
                    SpriteColumn++;
                    SpriteRow++;
                    if (SpriteColumn == 8 && SpriteRow == 9)
                    {
                        Explode = false;
                        SpriteRow = 0;
                        SpriteColumn = 0;
                        expsound.Play();

                    }

                    
                }
            }



            else
            {

            }

            //Drop the break every 520 mili
            if (breakdroptimer >= 520)
            {
                dropbreak();
                breakdroptimer = 0;
            }
            else
            {

                breakdroptimer++;
            }

            //Drop the break every 220 mili
            if (lifedroptimer >= 220)
            {
                droplife();
                lifedroptimer = 0;
            }
            else
            {

                lifedroptimer++;
            }
                base.Update(gameTime);

                //Break Enemy Wepons
                #region Drop Break
                for (int i = 0; i < 10; i++)
                {
                    breakDrop[i].PowerRect.X = (int)breakDrop[i].X;
                    breakDrop[i].PowerRect.Y = (int)breakDrop[i].Y;
                    if (breakDrop[i].Visible == true)
                    {
                        if (Ship.SpriteRectangle.Intersects(breakDrop[i].PowerRect))
                        {
                                breakDrop[i].Visible = false;
                                WeponDisable = true;
                                if (WeponDisable == true)
                                {
                                    if (breaktime >= 220)
                                    {
                                        WeponDisable = false;
                                        breaktime = 0;
                                    }
                                    else
                                    {

                                        breaktime++;
                                    }
                                }
                                break;
                         }
                        if (breakDrop[i].Y > dheight)
                        {
                            breakDrop[i].Visible = false;

                            break;
                        }



                        breakDrop[i].Y = breakDrop[i].Y + 5;
                    }

                }
                #endregion

                //Life Drop
                #region Life Drop
                for (int i = 0; i < 10; i++)
                {
                    lifeDrop[i].PowerRect.X = (int)lifeDrop[i].X;
                    lifeDrop[i].PowerRect.Y = (int)lifeDrop[i].Y;
                    if (lifeDrop[i].Visible == true)
                    {
                        if (Ship.SpriteRectangle.Intersects(lifeDrop[i].PowerRect))
                        {
                            if (lifeDrop[i].Visible == true)
                            {
                                lifeDrop[i].Visible = false;
                                lives = lives + 2;
                                break;
                            }

                        }
                        if (lifeDrop[i].Y > dheight)
                        {
                            lifeDrop[i].Visible = false;

                            break;
                        }



                        lifeDrop[i].Y = lifeDrop[i].Y + 5;
                    }


                }
                #endregion
        }

        
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.DarkSlateGray);

            spriteBatch.Begin();

            if (screen == "title") { spriteBatch.Draw(intro, new Rectangle(0, 0, (int)dwidth, (int)dheight), Color.White); spriteBatch.DrawString(Font, "Press X to play!", new Vector2(250, 500), Color.White); KeyboardState keys = Keyboard.GetState(); GamePadState pad = GamePad.GetState(PlayerIndex.One);if (keys.IsKeyDown(Keys.X) || pad.Buttons.X == ButtonState.Pressed) { screen = "inst"; } }

            #region instructions
            if (screen == "inst")
            {
                spriteBatch.DrawString(Font, "Welcome to space Wars!\n\nControls: \nShoot: Space\nMove: Left and Right arrow\n\nObjective: \nDestroy your enemy before he destroys you! But be \nCareful! Each one of his hits takes out a life and you only \nhave 3 to spare! Good Luck!\n\n\nPress Y to Play!", new Vector2(0, 50), Color.White);
                GamePadState pad = GamePad.GetState(PlayerIndex.One);
                KeyboardState keys = Keyboard.GetState(); if (keys.IsKeyDown(Keys.Y) || pad.Buttons.Y == ButtonState.Pressed) { screen = "play"; }
            }
            #endregion
            #region Play
            if (screen == "play")
            {
                if (MediaPlayer.State == MediaState.Stopped)
                {
                    MediaPlayer.Play(bgMusic);
                }

                else
                {

                }
                spriteBatch.Draw(background, bgrect, Color.White);
                spriteBatch.Draw(Ship.SpriteTexture, Ship.SpriteRectangle, Color.White);
                spriteBatch.Draw(enemies.SpriteTexture, enemies.SpriteRectangle, Color.White);

                Rectangle beam;

                for (int i = 0; i < beams.Count; i++)
                {
                    beams[i].Draw(spriteBatch);
                    beam = new Rectangle(Convert.ToInt32(beams[i].Pos.X), Convert.ToInt32(beams[i].Pos.Y), beams[i].Width, beams[i].Height);

                    if (beam.Intersects(enemies.SpriteRectangle) && colide1 == false)
                    {
                        colide1 = true;
                        ehealth = ehealth - damage;
                        score++;
                    }
                }




                Rectangle beam1;

                for (int i = 0; i < beams1.Count; i++)
                {
                    beams1[i].Draw(spriteBatch);
                    beam1 = new Rectangle(Convert.ToInt32(beams1[i].Pos1.X), Convert.ToInt32(beams1[i].Pos1.Y), beams1[i].Width1, beams1[i].Height1);


                    //if (beam1.Y <= Ship.SpriteRectangle.Y)
                    //{
                    //    if (beam1.X > Ship.SpriteRectangle.X)
                    //    {
                    //        if (beam1.X < Ship.SpriteRectangle.X + Ship.SpriteRectangle.Width)
                    //        {
                    //                //this.Exit();
                    //        }
                    //    }
                    //};

                    if (beam1.Intersects(Ship.SpriteRectangle) && colide == false)
                    {
                        colide = true;
                        lives--;
                    }
                }

                spriteBatch.DrawString(Font, "Enemy Health: " + ehealth.ToString(), new Vector2(50, 50), Color.White);
                spriteBatch.DrawString(Font, "Score: " + score.ToString(), new Vector2(350, 50), Color.White);
                spriteBatch.DrawString(Font, "Level: " + levels.ToString(), new Vector2(475, 50), Color.White);

                int y = Convert.ToInt32(dheight - 50);

                if (lives == 5 || lives == 4)
                {
                    lives = 3;
                }

                switch (lives)
                {
                    case 3:
                        spriteBatch.Draw(HeartFull, new Rectangle(30, y, 50, 50), Color.White);
                        spriteBatch.Draw(HeartFull, new Rectangle(80, y, 50, 50), Color.White);
                        spriteBatch.Draw(HeartFull, new Rectangle(130, y, 50, 50), Color.White);
                        break;
                    case 2:
                        spriteBatch.Draw(HeartEmpty, new Rectangle(30, y, 50, 50), Color.White);
                        spriteBatch.Draw(HeartFull, new Rectangle(80, y, 50, 50), Color.White);
                        spriteBatch.Draw(HeartFull, new Rectangle(130, y, 50, 50), Color.White);
                        break;
                    case 1:
                        spriteBatch.Draw(HeartEmpty, new Rectangle(30, y, 50, 50), Color.White);
                        spriteBatch.Draw(HeartEmpty, new Rectangle(80, y, 50, 50), Color.White);
                        spriteBatch.Draw(HeartFull, new Rectangle(130, y, 50, 50), Color.White);
                        break;
                }


                
                if (lives <= 0)
                {
                    move = false;
                    Explode = true;
                    ExplosionOrigin = new Rectangle((int)Ship.SpriteRectangle.X - 100, (int)Ship.SpriteRectangle.Y -35, 300, 300);
                    ExplosionSprite = new Rectangle(SpriteColumn * spriteWidth, SpriteRow * spriteHeight, spriteWidth, spriteHeight);
                    spriteBatch.Draw(Explosion, ExplosionOrigin, ExplosionSprite, Color.White);
                    screen = "over";
                }

                else if (ehealth <= 0)
                {
                    move = false;
                    Explode = true;
                    NextScene = true;
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(850);
                    ExplosionOrigin = new Rectangle((int)enemies.SpriteRectangle.X -55, (int)enemies.SpriteRectangle.Y + 25, 300, 300);
                    ExplosionSprite = new Rectangle(SpriteColumn * spriteWidth, SpriteRow * spriteHeight, spriteWidth, spriteHeight);
                    spriteBatch.Draw(Explosion, ExplosionOrigin, ExplosionSprite, Color.White);
                }


                for (int i = 0; i < 10; i++)
                {
                    if (breakDrop[i].Visible == true)
                    {
                        spriteBatch.Draw(breakDrop[i].PowerPic, breakDrop[i].PowerRect, Color.White);

                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    if (lifeDrop[i].Visible == true)
                    {
                        spriteBatch.Draw(lifeDrop[i].PowerPic, lifeDrop[i].PowerRect, Color.White);

                    }
                } 

                if (Explode == true)
                {
                    timeSpan -= gameTime.ElapsedGameTime;
                    if (timeSpan < TimeSpan.Zero)
                    {
                        timeSpan = TimeSpan.FromMilliseconds(850);
                        levels = levels + 1;
                        Explode = false;
                        switch (levels)
                        {
                            case 1:
                                move = true;
                                FT1 = TimeSpan.FromSeconds(.55f);
                                lives = 3;
                                damage = 10;
                                ehealth = 200;
                                break;
                            case 2:
                                move = true;
                                FT1 = TimeSpan.FromSeconds(.45f);
                                lives = 3;
                                damage = 10;
                                ehealth = 200;
                                break;
                            case 3:
                                FT1 = TimeSpan.FromSeconds(.35f);
                                move = true;
                                lives = 3;
                                damage = 15;
                                ehealth = 400;
                                break;
                            case 4:
                                FT1 = TimeSpan.FromSeconds(.30f);
                                lives = 3;
                                move = true;
                                damage = 20;
                                ehealth = 600;
                                break;
                            case 5:
                                screen = "win";
                                break;
                        }
                    }
                        
                }



            }

            #endregion
            #region win
            if (screen == "win")
            {
                spriteBatch.DrawString(Font, "YOU WIN", new Vector2(0, 50), Color.White);
                spriteBatch.DrawString(Font, "Credits:", new Vector2(0, 100), Color.White);
                spriteBatch.DrawString(Font, "logo: http://crypticgfx.com/showthread.php?tid=92", new Vector2(0, 125), Color.White);
                spriteBatch.DrawString(Font, "song: https://soundcloud.com/gregy_james (me)", new Vector2(0, 150), Color.White);
                spriteBatch.DrawString(Font, "Press A to play again!", new Vector2(0, 450), Color.White);
                spriteBatch.DrawString(Font, "Press B to quit, champion!", new Vector2(0, 500), Color.White);
                GamePadState pad = GamePad.GetState(PlayerIndex.One);
                KeyboardState keys = Keyboard.GetState(); if (keys.IsKeyDown(Keys.A) || pad.Buttons.A == ButtonState.Pressed) {
                    reset(0, 3, 0, "play", 100, true, FT1);
                }
                if (keys.IsKeyDown(Keys.B) || pad.Buttons.B == ButtonState.Pressed) { this.Exit(); }
            }
#endregion
            #region Over
            if (screen == "over")
            {
                spriteBatch.DrawString(Font, "YOU LOOSE", new Vector2(0, 50), Color.White);
                spriteBatch.DrawString(Font, "*sarcasticly* Aliens have taken over earth. 'Way to go!'", new Vector2(0, 100), Color.White);

                spriteBatch.DrawString(Font, "Press A to play again!", new Vector2(0, 200), Color.White);

                spriteBatch.DrawString(Font, "Press B to quit, sore loser...", new Vector2(0, 250), Color.White);

                GamePadState pad = GamePad.GetState(PlayerIndex.One);
                KeyboardState keys = Keyboard.GetState(); 
                
                if (keys.IsKeyDown(Keys.A) || pad.Buttons.A == ButtonState.Pressed) {
                    reset(0, 3, 0, "play", 100, true, FT1);
                    }
                if (keys.IsKeyDown(Keys.B) || pad.Buttons.B == ButtonState.Pressed) { this.Exit(); }
            }
            #endregion
            spriteBatch.End();
            
            base.Draw(gameTime);
        }


        //Code for Updating Player Beams
        #region Update beams (player and enemies)
        public void UpdateBeams()
        {
            

            for (int i = beams.Count - 1; i >= 0; i--)
            {
                beams[i].Update();

                if (beams[i].Enabled == false)
                {
                    //Remove the beams if they are disbled.
                    beams.RemoveAt(i);
                }
            }
        }

        //Code for Updating Enemy Beams
        public void UpdateBeams1()
        {
            for (int i = beams1.Count - 1; i >= 0; i--)
            {
                beams1[i].Update();

                if (beams1[i].Enabled1 == false)
                {
                    beams1.RemoveAt(i);
                }
            }
        }

        #endregion
        #region Reset the levels
        public void reset(int Score, int Lives, int Level, string Screen, int Ehealth, bool Move, TimeSpan time)
        {
            move = Move;
            score = Score;
            lives = Lives;
            levels = Level;
            screen = "play";
            ehealth = Ehealth;
            time = TimeSpan.FromSeconds(.55f);
            Ship.SpriteRectangle.X = 0;
            damage = 5;
        }
        #endregion

    }
}
