using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Semester1Project
{
    //Oragnise each level

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D whiteBox, menuScreenTxr, playerSheetTxr, MenuTextTxr, EnemySheetTxr, Lvl1PlatformTxr, UFOCollectTxr, backGroundTxr;
        SpriteFont uiTextFont, heartFont;
        SoundEffect jumpSound, fanfareSound, victorySound, deathSound, coinSound, completionSound;
        Song backgroundSong;
        
        Point screenSize = new Point(800, 480);
        int levelNumber = 0;
        float playTime = 0;

        PlayerSprite playerSprite;
        CoinSprite coinSprite;
        UFOSprite ufoSprite;
        SpikesSprite spikesSprite;
        BrainEnemySprite brainSprite;

        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>();
        List<Vector2> UFO = new List<Vector2>();
        List<Vector2> coins = new List<Vector2>();
        List<Vector2> brainEnemy = new List<Vector2>();
        List<Vector2> spikes = new List<Vector2>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            menuScreenTxr = Content.Load<Texture2D>("MenuScreen");
            playerSheetTxr = Content.Load<Texture2D>("PlayerSpriteSheet");
            MenuTextTxr = Content.Load<Texture2D>("MenuScreenText");
            backGroundTxr = Content.Load<Texture2D>("CanyonBackground");
            EnemySheetTxr = Content.Load<Texture2D>("EnemySpriteSheet");
            Lvl1PlatformTxr = Content.Load<Texture2D>("Platform");
            UFOCollectTxr = Content.Load<Texture2D>("UFO");
            uiTextFont = Content.Load<SpriteFont>("UIText");
            heartFont = Content.Load<SpriteFont>("HeartText");
            fanfareSound = Content.Load<SoundEffect>("fanfare");
            jumpSound = Content.Load<SoundEffect>("JumpSound");
            deathSound = Content.Load<SoundEffect>("Death");
            victorySound = Content.Load<SoundEffect>("Victory");
            coinSound = Content.Load<SoundEffect>("Coin");
            completionSound = Content.Load<SoundEffect>("Ending");

            this.backgroundSong = Content.Load<Song>("BackgroundSong");
            MediaPlayer.Play(backgroundSong);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;

            whiteBox = new Texture2D(GraphicsDevice, 1, 1);
            whiteBox.SetData(new[] { Color.White });

            playerSprite = new PlayerSprite(playerSheetTxr, whiteBox, new Vector2(50, 250), jumpSound);
            coinSprite = new CoinSprite(playerSheetTxr, whiteBox, new Vector2(630, 270));
            ufoSprite = new UFOSprite(UFOCollectTxr, whiteBox, new Vector2(725, 320));
            spikesSprite = new SpikesSprite(EnemySheetTxr, whiteBox, new Vector2(310, 342));
            brainSprite = new BrainEnemySprite(EnemySheetTxr, whiteBox, new Vector2(215, 280));
            BuildLevels();

        }

        void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e)
        {
            MediaPlayer.Volume -= 0.1f;
            MediaPlayer.Play(backgroundSong);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            playerSprite.Update(gameTime, levels[levelNumber]);

            if (playerSprite.coinsCollected < 0)
            {
                playerSprite.coinsCollected = 0;
            }

            if (playerSprite.spritePos.Y > screenSize.Y + 50)
            {
                playerSprite.lives--;
                if (playerSprite.lives <= 0)
                {
                    playerSprite.lives = 3;
                    playerSprite.coinsCollected = 0;
                    playTime = 0;
                    levelNumber = 0;
                }
                if (playerSprite.coinsCollected < 0)
                {
                    playerSprite.coinsCollected = 0;
                }
                playerSprite.ResetPlayer(new Vector2(50, 250));
                playerSprite.coinsCollected--;
                coinSprite.dead = false;
                deathSound.Play();
            }

            if (playerSprite.checkCollision(ufoSprite))
            {
                levelNumber++;
                if (levelNumber >= levels.Count) levelNumber = 0;
                ufoSprite.spritePos = UFO[levelNumber];
                playerSprite.ResetPlayer(new Vector2(50, 250));
                if (levelNumber <= 4)
                {
                    victorySound.Play();
                }
                else if (levelNumber == 5) completionSound.Play();
                coinSprite.dead = false;
            }

            if (!coinSprite.dead && playerSprite.checkCollision(coinSprite))
            {
                playerSprite.coinsCollected++;
                coinSprite.spritePos = coins[levelNumber];
                if (playerSprite.coinsCollected < 0)
                {
                    playerSprite.coinsCollected = 0;
                }
                coinSprite.dead = true;
                coinSound.Play(0.5f, 1, 0); 
            }

            if (playerSprite.checkCollision(spikesSprite))
            {
                playerSprite.lives--;
                coinSprite.dead = false;
                playerSprite.coinsCollected--;
                spikesSprite.spritePos = spikes[levelNumber];
                if (playerSprite.lives <= 0)
                {
                    playerSprite.lives = 3;
                    playerSprite.coinsCollected = 0;
                    playTime = 0;
                    levelNumber = 0;
                }
                if (playerSprite.coinsCollected < 0)
                {
                    playerSprite.coinsCollected = 0;
                }
                playerSprite.ResetPlayer(new Vector2(50, 250));
                deathSound.Play();
            }

            if (playerSprite.checkCollision(brainSprite))
            {
                brainSprite.spritePos = brainEnemy[levelNumber];
                playerSprite.lives--;
                coinSprite.dead = false;
                playerSprite.coinsCollected--;
                if (playerSprite.lives <= 0)
                {
                    playerSprite.lives = 3;
                    playerSprite.coinsCollected = 0;
                    playTime = 0;
                    levelNumber = 0;
                }
                if (playerSprite.coinsCollected < 0)
                {
                    playerSprite.coinsCollected = 0;
                }
                playerSprite.ResetPlayer(new Vector2(50, 250));
                deathSound.Play();
            }

            if (playerSprite.lives > 0)
            {
                playerSprite.Update(gameTime);
                playTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (levelNumber == 0) _spriteBatch.Draw(menuScreenTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);
            else _spriteBatch.Draw(backGroundTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);


            playerSprite.Draw(_spriteBatch, gameTime);
            ufoSprite.Draw(_spriteBatch, gameTime);
            spikesSprite.Draw(_spriteBatch, gameTime);
            if (!coinSprite.dead) coinSprite.Draw(_spriteBatch, gameTime);
            brainSprite.Draw(_spriteBatch, gameTime);

            foreach (PlatformSprite platform in levels[levelNumber]) platform.Draw(_spriteBatch, gameTime);

            string livesString = "";
            
            if (playerSprite.lives == 3) livesString = "bim"; 
            else if (playerSprite.lives == 2) livesString = "im"; 
            else if (playerSprite.lives == 1) livesString = "b";
            else livesString = "";

            _spriteBatch.DrawString(heartFont, livesString, new Vector2(15, 5), Color.White);

           _spriteBatch.DrawString(uiTextFont, "level " + (levelNumber + 1), new Vector2(screenSize.X - 15 - uiTextFont.MeasureString("level " + (levelNumber + 1)).X, 5), Color.White);
           _spriteBatch.DrawString(uiTextFont, "Coins " + (playerSprite.coinsCollected), new Vector2(screenSize.X - 15 - uiTextFont.MeasureString("Coins " + playerSprite.coinsCollected).X, 30), Color.White);
           _spriteBatch.DrawString(uiTextFont, "Time: " + Math.Round(playTime) + "s", new Vector2(703, 55), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void BuildLevels()
        {
            levels.Add(new List<PlatformSprite>());
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(122, 350)));
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 350)));
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(314, 350)));
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 350)));
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 350)));
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(602, 350)));
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(698, 350)));
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(630, 270));
            brainEnemy.Add(new Vector2(215, 280));
            spikes.Add(new Vector2(310, 342));

            levels.Add(new List<PlatformSprite>());
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(150, 300)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 350)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(314, 350)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 350)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 350)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(602, 350)));

            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(630, 270));
            brainEnemy.Add(new Vector2(215, 280));
            spikes.Add(new Vector2(310, 342));

            levels.Add(new List<PlatformSprite>());
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(122, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(314, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(602, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(698, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(630, 270));
            brainEnemy.Add(new Vector2(215, 280));
            spikes.Add(new Vector2(310, 342));

            levels.Add(new List<PlatformSprite>());
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(122, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(314, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(602, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(698, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(630, 270));
            brainEnemy.Add(new Vector2(215, 280));
            spikes.Add(new Vector2(310, 342));

            levels.Add(new List<PlatformSprite>());
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(122, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(314, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(602, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(698, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(630, 270));
            brainEnemy.Add(new Vector2(215, 280));
            spikes.Add(new Vector2(310, 342));

            levels.Add(new List<PlatformSprite>());
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(122, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(314, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(602, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(698, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(630, 270));
            brainEnemy.Add(new Vector2(215, 280));
            spikes.Add(new Vector2(310, 342));


        }
    }
}
