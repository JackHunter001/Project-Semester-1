using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Semester1Project
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D whiteBox, menuScreenTxr, playerSheetTxr, MenuTextTxr, EnemySheetTxr, Lvl1PlatformTxr, UFOCollectTxr, backGroundTxr; //setting the texture variables
        SpriteFont uiTextFont, heartFont; //setting the font variables
        SoundEffect jumpSound, fanfareSound, victorySound, deathSound, coinSound, completionSound, restartSound; //setting the sound variables
        Song backgroundSong; //setting the song variables
        
        Point screenSize = new Point(800, 480); //setting the game window size
        int levelNumber = 0; //setting a variable for the level the player is on
        float playTime = 0; //setting a variable for the timer

        PlayerSprite playerSprite; //variable for player class
        CoinSprite coinSprite; //variable for coin class
        UFOSprite ufoSprite; //variable for ufo class
        SpikesSprite spikesSprite; //variable for spikes class
        SpikesSprite2 spikesSprite2; //variable for secondary spikes class
        BrainEnemySprite brainSprite; //variable for brainbot class
        BrainEnemySprite2 brainSprite2; //variable for secondary brainbot class

        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>(); //setting a list of lists for the platforms in the levels
        List<Vector2> UFO = new List<Vector2>(); //setting a list for the ufo's
        List<Vector2> coins = new List<Vector2>(); //setting a list for the coins
        List<Vector2> brainEnemy = new List<Vector2>(); //setting a list for the brainbots
        List<Vector2> brainEnemy2 = new List<Vector2>(); //setting a list for the secondary brainbots
        List<Vector2> spikes = new List<Vector2>(); //setting a list for the spikes
        List<Vector2> spikes2 = new List<Vector2>(); //setting a list for the secondary spikes

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = screenSize.X; //setting the width of the screen to the screensize variable X 
            _graphics.PreferredBackBufferHeight = screenSize.Y; //setting the height of the screen  to the screensize variable Y 

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            menuScreenTxr = Content.Load<Texture2D>("MenuScreen"); //loading in the menuscreen image
            playerSheetTxr = Content.Load<Texture2D>("PlayerSpriteSheet"); //loading in the player-sprite-sheet image
            backGroundTxr = Content.Load<Texture2D>("CanyonBackground"); //loading in the background image
            EnemySheetTxr = Content.Load<Texture2D>("EnemySpriteSheet"); //loading in the enemy-sprite-sheet image
            Lvl1PlatformTxr = Content.Load<Texture2D>("Platform"); //loading in the platform image 
            UFOCollectTxr = Content.Load<Texture2D>("UFO"); //loading in the ufo image
            uiTextFont = Content.Load<SpriteFont>("UIText"); //loading in the UItext font
            heartFont = Content.Load<SpriteFont>("HeartText"); //loading in the hearttext font
            fanfareSound = Content.Load<SoundEffect>("fanfare"); //loading in the fanfare sound
            jumpSound = Content.Load<SoundEffect>("JumpSound"); //loading in the jump sound
            deathSound = Content.Load<SoundEffect>("Death"); //loading in the death sound
            victorySound = Content.Load<SoundEffect>("Victory"); //loading in the victory sound
            coinSound = Content.Load<SoundEffect>("Coin"); //loading in the coin sound
            completionSound = Content.Load<SoundEffect>("Ending"); //loading in the completion sound
            restartSound = Content.Load <SoundEffect>("GameRestart"); //loading in the restart sound

            this.backgroundSong = Content.Load<Song>("BackgroundSong"); //loading in the background song
            MediaPlayer.Play(backgroundSong); //telling the song to play
            MediaPlayer.IsRepeating = true; //telling the sound to loop once finished
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged; //loading in the mediaplayer_mediastatechanged

            whiteBox = new Texture2D(GraphicsDevice, 1, 1); //creating a (1,1) texture
            whiteBox.SetData(new[] { Color.White }); //setting the new texture to white

            playerSprite = new PlayerSprite(playerSheetTxr, whiteBox, new Vector2(50, 290), jumpSound); //the constructor for the PlayerSprite class
            coinSprite = new CoinSprite(playerSheetTxr, whiteBox, new Vector2(630, 270)); //the constructor for the CoinSprite class
            ufoSprite = new UFOSprite(UFOCollectTxr, whiteBox, new Vector2(725, 320)); //the constructor for the UFOSprite class
            spikesSprite = new SpikesSprite(EnemySheetTxr, whiteBox, new Vector2(400, 342)); //the constructor for the SpikesSprite class
            spikesSprite2 = new SpikesSprite2(EnemySheetTxr, whiteBox, new Vector2(428, 342)); //the constructor for the SpikesSprite2 class
            brainSprite = new BrainEnemySprite(EnemySheetTxr, whiteBox, new Vector2(300, 280)); //the constructor for the brainSprite class
            brainSprite2 = new BrainEnemySprite2(EnemySheetTxr, whiteBox, new Vector2(530, 280)); //the constructor for the brainSprite2 class
            BuildLevels(); //building the levels
        }

        void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e)
        {
            MediaPlayer.Volume -= 0.1f; //changing the volume of the song
            MediaPlayer.Play(backgroundSong); //playing the song
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            playerSprite.Update(gameTime, levels[levelNumber]);

            if (playerSprite.coinsCollected < 0)
            {
                playerSprite.coinsCollected = 0;
            } //telling the coinscollected variable if it is below 0 to go back to 0 (not allowing it to go below 0)

            if (playerSprite.spritePos.Y > screenSize.Y + 50) //checking to see if the player is in this position range
            {
                playerSprite.lives--; //players lives decreases by 1
                if (playerSprite.lives <= 0) // checking to see if the players lives is below or equal to 0
                {
                    playerSprite.lives = 3; //player lives equal 3
                    playerSprite.coinsCollected = 0; //coinscollected equal 0
                    playTime = 0; //timer equals 0
                    levelNumber = 0; //level changed to the starting level
                }
                if (playerSprite.coinsCollected < 0)
                {
                    playerSprite.coinsCollected = 0;
                }
                playerSprite.ResetPlayer(new Vector2(50, 290)); //player resets at starting position
                playerSprite.coinsCollected--; //coinscollected decreases by 1
                coinSprite.dead = false; //coinsprite is not dead
                if (playerSprite.lives <= 0) restartSound.Play(); //if lives are 0 or under then restartsound plays
                else if (playerSprite.lives > 0) deathSound.Play(); //if lives are over 0 thne deathsound plays
            }

            if (playerSprite.checkCollision(ufoSprite)) //checking to see if the player is colliding with the ufosprite
            {
                levelNumber++; //levelnumber increases by 1
                if (levelNumber >= levels.Count) levelNumber = 0; //when the player beats the last level it restarts back to the first level
                ufoSprite.spritePos = UFO[levelNumber];
                playerSprite.ResetPlayer(new Vector2(50, 290));
                if (levelNumber <= 4)
                {
                    victorySound.Play();
                } //if levelnumber is under or equal to 4 then victorysound plays
                else if (levelNumber == 5) completionSound.Play(); //if levelnumber equals to 5 then completionsound plays
                coinSprite.dead = false; //coinsprite is not dead
            }

            if (!coinSprite.dead && playerSprite.checkCollision(coinSprite)) //checking to see if the coin is not dead and if the player is collidjg with the coin
            {
                playerSprite.coinsCollected++; //coinscollected increases by 1
                coinSprite.spritePos = coins[levelNumber];
                if (playerSprite.coinsCollected < 0)
                {
                    playerSprite.coinsCollected = 0;
                }
                coinSprite.dead = true;
                coinSound.Play(0.5f, 1, 0); //coinsound plays at a lower volume and higher speed
            }

            if (playerSprite.checkCollision(spikesSprite)) //checking to see if the player is colliding with the spikes
            {
                playerSprite.lives--; //player lives decrease by 1
                coinSprite.dead = false; //coin is not dead
                playerSprite.coinsCollected--; //coinscollected decreases by 1
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
                playerSprite.ResetPlayer(new Vector2(50, 290)); //resets the player at those coordinates
                if (playerSprite.lives <= 0) restartSound.Play();
                else if (playerSprite.lives > 0) deathSound.Play();
            }

            if (playerSprite.checkCollision(spikesSprite2)) //checking to see if the player is colliding with the spikes
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
                if (playerSprite.lives <= 0) restartSound.Play();
                else if (playerSprite.lives > 0) deathSound.Play();
            }

            if (playerSprite.checkCollision(brainSprite)) //checking to see if the player is colliding with the brainbot
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
                if (playerSprite.lives <= 0) restartSound.Play();
                else if (playerSprite.lives > 0) deathSound.Play();
            }

            if (playerSprite.checkCollision(brainSprite2)) //checking to see if the player is colliding with the brainbot
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
                if (playerSprite.lives <= 0) restartSound.Play();
                else if (playerSprite.lives > 0) deathSound.Play();
            }

            if (playerSprite.lives > 0) 
            {
                playerSprite.Update(gameTime); 
                playTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            } //if the players lives are above 0 then the players gametime will update and playtime will add abd then equal the total game seconds played

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            if (levelNumber == 0) _spriteBatch.Draw(menuScreenTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); 
            else _spriteBatch.Draw(backGroundTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); //if the levelnumber equals 0 then menuscreentxr will be drawn on the background but if the level number aint equal to 0 then backGroundtxr will be the background drawn

            playerSprite.Draw(_spriteBatch, gameTime); //drawing the playersprite
            ufoSprite.Draw(_spriteBatch, gameTime); //drawing the ufosprite
            spikesSprite.Draw(_spriteBatch, gameTime); //drawing the spikessprite
            spikesSprite2.Draw(_spriteBatch, gameTime); //drawing the secondary spikessprite
            if (!coinSprite.dead) coinSprite.Draw(_spriteBatch, gameTime); //if the coin sprite is not dead thne it will draw the coinsprite
            brainSprite.Draw(_spriteBatch, gameTime); //drawing the brainbotsprite
            brainSprite2.Draw(_spriteBatch, gameTime); //drawing the secondary brainbotsprite

            foreach (PlatformSprite platform in levels[levelNumber]) platform.Draw(_spriteBatch, gameTime); //drawing the platforms

            string livesString = ""; //creating a string variable for lives
            
            if (playerSprite.lives == 3) livesString = "bim";  //displaying the letters 'b' 'i' 'm' from the selected font when the player has 3 lives
            else if (playerSprite.lives == 2) livesString = "im"; //displaying the letters 'i' 'm' from the selected font when the player has 2 lives
            else if (playerSprite.lives == 1) livesString = "b"; //displaying the letter 'b' from the selected font when the player has 1 life
            else livesString = ""; //displaying nothing when the player has 0 lives

            _spriteBatch.DrawString(heartFont, livesString, new Vector2(15, 5), Color.White); //drawing the fonts at those cooridnates

           _spriteBatch.DrawString(uiTextFont, "level " + (levelNumber + 1), new Vector2(screenSize.X - 15 - uiTextFont.MeasureString("level " + (levelNumber + 1)).X, 5), Color.White); //drawing the word level at this point
           _spriteBatch.DrawString(uiTextFont, "Coins " + (playerSprite.coinsCollected), new Vector2(screenSize.X - 15 - uiTextFont.MeasureString("Coins " + playerSprite.coinsCollected).X, 30), Color.White); //drawing the word Coins at this point
            _spriteBatch.DrawString(uiTextFont, "Time: " + Math.Round(playTime) + "s", new Vector2(703, 55), Color.White); //drawing the word Time: at this point

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
            levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350))); //adding platforms to the first level
            UFO.Add(new Vector2(725, 320)); //adding a UFO to the first level
            coins.Add(new Vector2(630, 270)); //adding a coin to the first level
            brainEnemy.Add(new Vector2(300, 280));  //adding a brainbot to the first level
            brainEnemy2.Add(new Vector2(530, 280));  //adding a secondary brainbot to the first level
            spikes.Add(new Vector2(400, 342));  //adding spikes to the first level
            spikes2.Add(new Vector2(428, 342));  //adding secondary spikes to the first level

            levels.Add(new List<PlatformSprite>());
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(160, 270)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(255, 180)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(190, 420)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(375, 420)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(520, 350)));
            levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(730, 350))); //adding platforms to the second level
            UFO.Add(new Vector2(725, 320)); //adding a UFO to the second level
            coins.Add(new Vector2(255, 160)); //adding a coin to the second level
            brainEnemy.Add(new Vector2(215, 280)); //adding a brainbot to the second level
            brainEnemy2.Add(new Vector2(215, 280)); //adding a secondary brainbot to the second level
            spikes.Add(new Vector2(310, 342)); //adding spikes to the second level
            spikes2.Add(new Vector2(310, 342)); //adding secondary spikes to the second level

            levels.Add(new List<PlatformSprite>());
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(-45, 260)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(100, 190)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(170, 400)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(320, 350)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(480, 320)));
            levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(730, 350))); //adding platforms to the third level
            UFO.Add(new Vector2(725, 320)); //adding a UFO to the third level
            coins.Add(new Vector2(630, 270)); //adding a coin to the third level 
            brainEnemy.Add(new Vector2(180, 250)); //adding a brainbot to the third level
            brainEnemy2.Add(new Vector2(600, 350)); //adding a secondary brainbot to the third level
            spikes.Add(new Vector2(133, 182)); //adding spikes to the third level
            spikes2.Add(new Vector2(353, 340)); //adding secondary spikes to the third level

            levels.Add(new List<PlatformSprite>());
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(205, 350)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(335, 275)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(205, 180)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 180)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(550, 275)));
            levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(730, 350))); //adding platforms to the fourth level
            UFO.Add(new Vector2(725, 320)); //adding a UFO to the fourth level
            coins.Add(new Vector2(30, 150)); //adding a coin to the fourth level
            brainEnemy.Add(new Vector2(180, 250)); //adding a brainbot to the fourth level
            brainEnemy2.Add(new Vector2(445, 270)); //adding a secondary brainbot to the fourth level
            spikes.Add(new Vector2(171, 172)); //adding spikes to the fourth level
            spikes2.Add(new Vector2(584, 267)); //adding secondary spikes to the fourth level

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
            levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(550, 30))); //adding platforms to the fifth level
            UFO.Add(new Vector2(725, 320)); //adding a UFO to the fifth level
            coins.Add(new Vector2(550, 3)); //adding a coin to the fifth level
            brainEnemy.Add(new Vector2(640, 30)); //adding a brainbot to the fifth level
            brainEnemy2.Add(new Vector2(20, 130)); //adding a secondary brainbot to the fifth level
            spikes.Add(new Vector2(234, 392)); //adding spikes to the fifth level
            spikes2.Add(new Vector2(507, 272)); //adding secondary spikes to the fifth level

            levels.Add(new List<PlatformSprite>());
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 300)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 250)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 250)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 478)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 478)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(570, 410)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(698, 350)));
            levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350))); //adding platforms to the sixth level
            UFO.Add(new Vector2(725, 320)); //adding a UFO to the sixth level
            coins.Add(new Vector2(223, 450)); //adding a coin to the sixth level
            brainEnemy.Add(new Vector2(310, 260)); //adding a brainbot to the sixth level
            brainEnemy2.Add(new Vector2(320, 460)); //adding a secondary brainbot to the sixth level
            spikes.Add(new Vector2(457, 242)); //adding spikes to the sixth level
            spikes2.Add(new Vector2(604, 402)); //adding secondary spikes to the sixth level


        }
    }
}
