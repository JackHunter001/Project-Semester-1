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
        SpikesSprite2 spikesSprite2;
        BrainEnemySprite brainSprite;
        BrainEnemySprite2 brainSprite2;

        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>();
        List<Vector2> UFO = new List<Vector2>();
        List<Vector2> coins = new List<Vector2>();
        List<Vector2> brainEnemy = new List<Vector2>();
        List<Vector2> brainEnemy2 = new List<Vector2>();
        List<Vector2> spikes = new List<Vector2>();
        List<Vector2> spikes2 = new List<Vector2>();

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

            playerSprite = new PlayerSprite(playerSheetTxr, whiteBox, new Vector2(50, 290), jumpSound);
            coinSprite = new CoinSprite(playerSheetTxr, whiteBox, new Vector2(630, 270));
            ufoSprite = new UFOSprite(UFOCollectTxr, whiteBox, new Vector2(725, 320));
            spikesSprite = new SpikesSprite(EnemySheetTxr, whiteBox, new Vector2(400, 342));
            spikesSprite2 = new SpikesSprite2(EnemySheetTxr, whiteBox, new Vector2(428, 342));
            brainSprite = new BrainEnemySprite(EnemySheetTxr, whiteBox, new Vector2(300, 280));
            brainSprite2 = new BrainEnemySprite2(EnemySheetTxr, whiteBox, new Vector2(530, 280));
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
                playerSprite.ResetPlayer(new Vector2(50, 290));
                playerSprite.coinsCollected--;
                coinSprite.dead = false;
                deathSound.Play();
            }

            if (playerSprite.checkCollision(ufoSprite))
            {
                levelNumber++;
                if (levelNumber >= levels.Count) levelNumber = 0;
                ufoSprite.spritePos = UFO[levelNumber];
                playerSprite.ResetPlayer(new Vector2(50, 290));
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
                playerSprite.ResetPlayer(new Vector2(50, 290));
                deathSound.Play();
            }

            if (playerSprite.checkCollision(spikesSprite2))
            {
                playerSprite.lives--;
                coinSprite.dead = false;
                playerSprite.coinsCollected--;
                spikesSprite2.spritePos = spikes2[levelNumber];
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
                playerSprite.ResetPlayer(new Vector2(50, 290));
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
                playerSprite.ResetPlayer(new Vector2(50, 290));
                deathSound.Play();
            }

            if (playerSprite.checkCollision(brainSprite2))
            {
                brainSprite2.spritePos = brainEnemy2[levelNumber];
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
            spikesSprite2.Draw(_spriteBatch, gameTime);
            if (!coinSprite.dead) coinSprite.Draw(_spriteBatch, gameTime);
            brainSprite.Draw(_spriteBatch, gameTime);
            brainSprite2.Draw(_spriteBatch, gameTime);

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
            brainEnemy.Add(new Vector2(300, 280));
            brainEnemy2.Add(new Vector2(530, 280));
            spikes.Add(new Vector2(400, 342));
            spikes2.Add(new Vector2(428, 342));

            levels.Add(new List<PlatformSprite>());
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(160, 270)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(255, 180)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(190, 420)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(375, 420)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(520, 350)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(730, 350)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(255, 160));
            brainEnemy.Add(new Vector2(215, 280));
            brainEnemy2.Add(new Vector2(215, 280));
            spikes.Add(new Vector2(310, 342));
            spikes2.Add(new Vector2(310, 342));

            levels.Add(new List<PlatformSprite>());
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(-45, 260)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(100, 190)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(170, 400)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(320, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(480, 320)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(730, 350)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(630, 270));
            brainEnemy.Add(new Vector2(180, 250));
            brainEnemy2.Add(new Vector2(600, 350));
            spikes.Add(new Vector2(133, 182));
            spikes2.Add(new Vector2(353, 340));

            levels.Add(new List<PlatformSprite>());
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(205, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(335, 275)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(205, 180)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 180)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(550, 275)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(730, 350)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(30, 150));
            brainEnemy.Add(new Vector2(180, 250));
            brainEnemy2.Add(new Vector2(445, 270));
            spikes.Add(new Vector2(171, 172));
            spikes2.Add(new Vector2(584, 267));

            levels.Add(new List<PlatformSprite>());
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(200, 400)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(390, 400)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(570, 440)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(780, 350)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(684, 370)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(540, 280)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(380, 255)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(210, 220)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(45, 190)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(215, 110)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(380, 70)));
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(550, 30)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(550, 3));
            brainEnemy.Add(new Vector2(640, 30));
            brainEnemy2.Add(new Vector2(20, 130));
            spikes.Add(new Vector2(234, 392));
            spikes2.Add(new Vector2(507, 272));

            levels.Add(new List<PlatformSprite>());
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 300)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 250)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 250)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 478)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 478)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(570, 410)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(698, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350)));
            UFO.Add(new Vector2(725, 320));
            coins.Add(new Vector2(223, 450));
            brainEnemy.Add(new Vector2(310, 260));
            brainEnemy2.Add(new Vector2(320, 460));
            spikes.Add(new Vector2(457, 242));
            spikes2.Add(new Vector2(604, 402));


        }
    }
}
