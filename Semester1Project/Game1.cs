using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Semester1Project
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D whiteBox, menuScreenTxr, playerSheetTxr, EnemySheetTxr, Lvl1PlatformTxr, UFOCollectTxr, backGroundTxr; //setting the texture variables
        SpriteFont uiTextFont, heartFont; //setting the font variables
        SoundEffect jumpSound, fanfareSound, victorySound, deathSound, coinSound, completionSound, restartSound; //setting the sound variables
        Song backgroundSong; //setting the song variables
        
        Point screenSize = new Point(800, 480); //setting the game window size
        int levelNumber = 0; //setting a variable for the level the player is on
        float playTime = 0; //setting a variable for the timer

        PlayerSprite playerSprite; //variable for player class

        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>(); //setting a list of lists for the platforms in the levels
        List<List<CoinSprite>> coinSprite = new List<List<CoinSprite>>(); //setting a list of lists for the Coins in the levels
        List<List<UFOSprite>> ufoSprite = new List<List<UFOSprite>>(); //setting a list of lists for the UFO's in the levels
        List<List<SpikesSprite>> spikesSprite = new List<List<SpikesSprite>>(); //setting a list of lists for the Spikes in the levels
        List<List<SpikesSprite2>> spikesSprite2 = new List<List<SpikesSprite2>>(); //setting a list of lists for the secondary Spikes in the levels
        List<List<BrainEnemySprite>> brainSprite = new List<List<BrainEnemySprite>>(); //setting a list of lists for the BrainBots in the levels
        List<List<BrainEnemySprite2>> brainSprite2 = new List<List<BrainEnemySprite2>>(); //setting a list of lists for the secondary BrainBots in the levels

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

            UFOLevels(); //building the UFO's
            BrainBotsLevels(); //building the BrainBots
            BrainBots2Levels(); //building the secondary BrainBots
            SpikesLevels(); //building the spikes
            Spikes2Levels(); //building the secondary Spikes
            CoinsLevels(); //building the Coins
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
                foreach (CoinSprite coinSprite in coinSprite[levelNumber]) coinSprite.dead = false; //coinsprite is not dead
                if (playerSprite.lives <= 0) restartSound.Play(); //if lives are 0 or under then restartsound plays
                else if (playerSprite.lives > 0) deathSound.Play(); //if lives are over 0 thne deathsound plays
            }

            foreach (UFOSprite ufoSprite in ufoSprite[levelNumber])
            {
                if (playerSprite.checkCollision(ufoSprite))//checking to see if the player is colliding with the ufosprite
                {
                    levelNumber++; //levelnumber increases by 1
                    if (levelNumber >= levels.Count) levelNumber = 0; //when the player beats the last level it restarts back to the first level
                                                                      //ufoSprite.spritePos = ufoSprite[levelNumber];
                    playerSprite.ResetPlayer(new Vector2(50, 290));
                    if (levelNumber <= 5)
                    {
                        victorySound.Play();
                    } //if levelnumber is under or equal to 4 then victorysound plays
                    else if (levelNumber > 5) completionSound.Play(); //if levelnumber equals to 5 then completionsound plays
                    foreach (CoinSprite coinSprite in coinSprite[levelNumber]) coinSprite.dead = false; //coinsprite is not dead
                }
            }

            foreach (CoinSprite coinSprite in coinSprite[levelNumber])
            {
                if (!coinSprite.dead && playerSprite.checkCollision(coinSprite)) //checking to see if the coin is not dead and if the player is collidjg with the coin
                {
                    playerSprite.coinsCollected++; //coinscollected increases by 1
                    if (playerSprite.coinsCollected < 0)
                    {
                        playerSprite.coinsCollected = 0;
                    }
                    coinSprite.dead = true;
                    coinSound.Play(0.5f, 1, 0); //coinsound plays at a lower volume and higher speed
                }
            }

            foreach (SpikesSprite spikesSprite in spikesSprite[levelNumber])
            {
                if (playerSprite.checkCollision(spikesSprite)) //checking to see if the player is colliding with the spikes
                {
                    playerSprite.lives--; //player lives decrease by 1
                    foreach (CoinSprite coinSprite in coinSprite[levelNumber]) coinSprite.dead = false; //coin is not dead
                    playerSprite.coinsCollected--; //coinscollected decreases by 1
                                                   //spikesSprite.spritePos = spikes[levelNumber];
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
            }

            foreach (SpikesSprite2 spikesSprite2 in spikesSprite2[levelNumber])
            {
                if (playerSprite.checkCollision(spikesSprite2)) //checking to see if the player is colliding with the spikes
                {
                    playerSprite.lives--;
                    foreach (CoinSprite coinSprite in coinSprite[levelNumber]) coinSprite.dead = false;
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
            }

            foreach (BrainEnemySprite brainSprite in brainSprite[levelNumber])
            {
                if (playerSprite.checkCollision(brainSprite)) //checking to see if the player is colliding with the brainbot
                {
                    playerSprite.lives--;
                    foreach (CoinSprite coinSprite in coinSprite[levelNumber]) coinSprite.dead = false;
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
            }

            foreach (BrainEnemySprite2 brainSprite2 in brainSprite2[levelNumber])
            {
                if (playerSprite.checkCollision(brainSprite2)) //checking to see if the player is colliding with the brainbot
                {
                    playerSprite.lives--;
                    foreach (CoinSprite coinSprite in coinSprite[levelNumber]) coinSprite.dead = false;
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

            foreach (PlatformSprite platform in levels[levelNumber]) platform.Draw(_spriteBatch, gameTime); //drawing the platforms
            foreach (BrainEnemySprite brainSprite in brainSprite[levelNumber]) brainSprite.Draw(_spriteBatch, gameTime); //drawing the BrainBots
            foreach (BrainEnemySprite2 brainSprite2 in brainSprite2[levelNumber]) brainSprite2.Draw(_spriteBatch, gameTime); //drawing the secondary BrainBots
            foreach (SpikesSprite2 spikesSprite2 in spikesSprite2[levelNumber]) spikesSprite2.Draw(_spriteBatch, gameTime); //drawing the secondary Spikes
            foreach (SpikesSprite spikesSprite in spikesSprite[levelNumber]) spikesSprite.Draw(_spriteBatch, gameTime); //drawing the Spikes
            foreach (UFOSprite ufoSprite in ufoSprite[levelNumber]) ufoSprite.Draw(_spriteBatch, gameTime); //drawing the UFO's
            foreach (CoinSprite coinSprite in coinSprite[levelNumber]) if (!coinSprite.dead) coinSprite.Draw(_spriteBatch, gameTime); //drawing the Coins only if coinSprite is not dead

            string livesString = ""; //creating a string variable for lives
            
            if (playerSprite.lives == 3) livesString = "bim";  //displaying the letters 'b' 'i' 'm' from the selected font when the player has 3 lives
            else if (playerSprite.lives == 2) livesString = "im"; //displaying the letters 'i' 'm' from the selected font when the player has 2 lives
            else if (playerSprite.lives == 1) livesString = "b"; //displaying the letter 'b' from the selected font when the player has 1 life
            else livesString = ""; //displaying nothing when the player has 0 lives

            _spriteBatch.DrawString(heartFont, livesString, new Vector2(15, 5), Color.White); //drawing the fonts at those cooridnates

           _spriteBatch.DrawString(uiTextFont, "level " + (levelNumber + 1), new Vector2(screenSize.X - 15 - uiTextFont.MeasureString("level " + (levelNumber + 1)).X, 5), Color.White); //drawing the word level at this position and also adding 1 to levelnumber
           _spriteBatch.DrawString(uiTextFont, "Coins " + (playerSprite.coinsCollected), new Vector2(screenSize.X - 15 - uiTextFont.MeasureString("Coins " + playerSprite.coinsCollected).X, 30), Color.White); //drawing the word Coins at this position
            _spriteBatch.DrawString(uiTextFont, "Time: " + Math.Round(playTime) + "s", new Vector2(703, 55), Color.White); //drawing the word Time: at this position

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void CoinsLevels()
        {
            coinSprite.Add(new List<CoinSprite>());  //adding the list of Coins
            coinSprite[0].Add(new CoinSprite(playerSheetTxr, whiteBox, new Vector2(630, 270))); //adding the coin on the first level at this position

            coinSprite.Add(new List<CoinSprite>());  //adding the list of Coins
            coinSprite[1].Add(new CoinSprite(playerSheetTxr, whiteBox, new Vector2(255, 160))); //adding the coin on the second level at this position

            coinSprite.Add(new List<CoinSprite>());  //adding the list of Coins
            coinSprite[2].Add(new CoinSprite(playerSheetTxr, whiteBox, new Vector2(90, 150))); //adding the coin on the third level at this position

            coinSprite.Add(new List<CoinSprite>());  //adding the list of Coins
            coinSprite[3].Add(new CoinSprite(playerSheetTxr, whiteBox, new Vector2(30, 150))); //adding the coin on the fourth level at this position

            coinSprite.Add(new List<CoinSprite>());  //adding the list of Coins
            coinSprite[4].Add(new CoinSprite(playerSheetTxr, whiteBox, new Vector2(550, 3))); //adding the coin on the fifth level at this position

            coinSprite.Add(new List<CoinSprite>());  //adding the list of Coins
            coinSprite[5].Add(new CoinSprite(playerSheetTxr, whiteBox, new Vector2(223, 450))); //adding the coin on the sixth level at this position
        }

        void SpikesLevels()
        {
            spikesSprite.Add(new List<SpikesSprite>()); //adding the list of Sprites
            spikesSprite[0].Add(new SpikesSprite(EnemySheetTxr, whiteBox, new Vector2(400, 342))); //adding the Sprites on the first level at this position

            spikesSprite.Add(new List<SpikesSprite>()); //adding the list of Sprites
            spikesSprite[1].Add(new SpikesSprite(EnemySheetTxr, whiteBox, new Vector2(408, 411))); //adding the Sprites on the second level at this position

            spikesSprite.Add(new List<SpikesSprite>()); //adding the list of Sprites
            spikesSprite[2].Add(new SpikesSprite(EnemySheetTxr, whiteBox, new Vector2(133, 182))); //adding the Sprites on the third level at this position

            spikesSprite.Add(new List<SpikesSprite>()); //adding the list of Sprites
            spikesSprite[3].Add(new SpikesSprite(EnemySheetTxr, whiteBox, new Vector2(171, 172))); //adding the Sprites on the fourth level at this position

            spikesSprite.Add(new List<SpikesSprite>()); //adding the list of Sprites
            spikesSprite[4].Add(new SpikesSprite(EnemySheetTxr, whiteBox, new Vector2(234, 392))); //adding the Sprites on the fifth level at this position

            spikesSprite.Add(new List<SpikesSprite>()); //adding the list of Sprites
            spikesSprite[5].Add(new SpikesSprite(EnemySheetTxr, whiteBox, new Vector2(457, 242))); //adding the Sprites on the sixth level at this position
        }
   
        void Spikes2Levels()
        {
            spikesSprite2.Add(new List<SpikesSprite2>()); //adding the list of secondary Sprites
            spikesSprite2[0].Add(new SpikesSprite2(EnemySheetTxr, whiteBox, new Vector2(428, 342))); //adding the secondary Sprites on the first level at this position

            spikesSprite2.Add(new List<SpikesSprite2>()); //adding the list of secondary Sprites
            spikesSprite2[1].Add(new SpikesSprite2(EnemySheetTxr, whiteBox, new Vector2(194, 262))); //adding the secondary Sprites on the second level at this position

            spikesSprite2.Add(new List<SpikesSprite2>()); //adding the list of secondary Sprites
            spikesSprite2[2].Add(new SpikesSprite2(EnemySheetTxr, whiteBox, new Vector2(353, 340))); //adding the secondary Sprites on the third level at this position

            spikesSprite2.Add(new List<SpikesSprite2>()); //adding the list of secondary Sprites
            spikesSprite2[3].Add(new SpikesSprite2(EnemySheetTxr, whiteBox, new Vector2(584, 267))); //adding the secondary Sprites on the fourth level at this position

            spikesSprite2.Add(new List<SpikesSprite2>()); //adding the list of secondary Sprites
            spikesSprite2[4].Add(new SpikesSprite2(EnemySheetTxr, whiteBox, new Vector2(507, 272))); //adding the secondary Sprites on the fifth level at this position

            spikesSprite2.Add(new List<SpikesSprite2>()); //adding the list of secondary Sprites
            spikesSprite2[5].Add(new SpikesSprite2(EnemySheetTxr, whiteBox, new Vector2(604, 402))); //adding the secondary Sprites on the sixth level at this position
        }
        void BrainBotsLevels()
        {
            brainSprite.Add(new List<BrainEnemySprite>()); //adding the list of BrainBots
            brainSprite[0].Add(new BrainEnemySprite(EnemySheetTxr, whiteBox, new Vector2(300, 280))); //adding the BrainBots on the first level at this position

            brainSprite.Add(new List<BrainEnemySprite>()); //adding the list of BrainBots
            brainSprite[1].Add(new BrainEnemySprite(EnemySheetTxr, whiteBox, new Vector2(340, 220))); //adding the BrainBots on the second level at this position

            brainSprite.Add(new List<BrainEnemySprite>()); //adding the list of BrainBots
            brainSprite[2].Add(new BrainEnemySprite(EnemySheetTxr, whiteBox, new Vector2(180, 250))); //adding the BrainBots on the third level at this position

            brainSprite.Add(new List<BrainEnemySprite>()); //adding the list of BrainBots
            brainSprite[3].Add(new BrainEnemySprite(EnemySheetTxr, whiteBox, new Vector2(180, 250))); //adding the BrainBots on the fourth level at this position

            brainSprite.Add(new List<BrainEnemySprite>()); //adding the list of BrainBots
            brainSprite[4].Add(new BrainEnemySprite(EnemySheetTxr, whiteBox, new Vector2(640, 30))); //adding the BrainBots on the fifth level at this position

            brainSprite.Add(new List<BrainEnemySprite>()); //adding the list of BrainBots
            brainSprite[5].Add(new BrainEnemySprite(EnemySheetTxr, whiteBox, new Vector2(310, 260))); //adding the BrainBots on the sixth level at this position
        }
        void BrainBots2Levels()
        {
            brainSprite2.Add(new List<BrainEnemySprite2>()); //adding the list of secondary BrainBots
            brainSprite2[0].Add(new BrainEnemySprite2(EnemySheetTxr, whiteBox, new Vector2(530, 280))); //adding the secondary BrainBots on the first level at this position

            brainSprite2.Add(new List<BrainEnemySprite2>()); //adding the list of secondary BrainBots
            brainSprite2[1].Add(new BrainEnemySprite2(EnemySheetTxr, whiteBox, new Vector2(630, 340))); //adding the secondary BrainBots on the second level at this position

            brainSprite2.Add(new List<BrainEnemySprite2>()); //adding the list of secondary BrainBots
            brainSprite2[2].Add(new BrainEnemySprite2(EnemySheetTxr, whiteBox, new Vector2(600, 350))); //adding the secondary BrainBots on the third level at this position

            brainSprite2.Add(new List<BrainEnemySprite2>()); //adding the list of secondary BrainBots
            brainSprite2[3].Add(new BrainEnemySprite2(EnemySheetTxr, whiteBox, new Vector2(445, 270))); //adding the secondary BrainBots on the fourth level at this position

            brainSprite2.Add(new List<BrainEnemySprite2>()); //adding the list of secondary BrainBots
            brainSprite2[4].Add(new BrainEnemySprite2(EnemySheetTxr, whiteBox, new Vector2(20, 130))); //adding the secondary BrainBots on the fifth level at this position

            brainSprite2.Add(new List<BrainEnemySprite2>()); //adding the list of secondary BrainBots
            brainSprite2[5].Add(new BrainEnemySprite2(EnemySheetTxr, whiteBox, new Vector2(320, 460))); //adding the secondary BrainBots on the sixth level at this position
        }
        void UFOLevels()
        {
            ufoSprite.Add(new List<UFOSprite>()); //adding the list of UFOs
            ufoSprite[0].Add(new UFOSprite(UFOCollectTxr, whiteBox, new Vector2(725, 320))); //adding the UFO on the first level at this position

            ufoSprite.Add(new List<UFOSprite>()); //adding the list of platforms
            ufoSprite[1].Add(new UFOSprite(UFOCollectTxr, whiteBox, new Vector2(725, 320))); //adding the UFOs on the second level at this position

            ufoSprite.Add(new List<UFOSprite>()); //adding the list of platforms
            ufoSprite[2].Add(new UFOSprite(UFOCollectTxr, whiteBox, new Vector2(725, 320))); //adding the UFOs on the third level at this position

            ufoSprite.Add(new List<UFOSprite>()); //adding the list of platforms
            ufoSprite[3].Add(new UFOSprite(UFOCollectTxr, whiteBox, new Vector2(725, 320))); //adding the UFOs on the fourth level at this position

            ufoSprite.Add(new List<UFOSprite>()); //adding the list of platforms
            ufoSprite[4].Add(new UFOSprite(UFOCollectTxr, whiteBox, new Vector2(725, 320))); //adding the UFOs on the fifth level at this position

            ufoSprite.Add(new List<UFOSprite>()); //adding the list of platforms
            ufoSprite[5].Add(new UFOSprite(UFOCollectTxr, whiteBox, new Vector2(725, 320))); //adding the UFOs on the sixth level at this position
        }

        void BuildLevels()
            {
                levels.Add(new List<PlatformSprite>()); //adding the list of platforms
                levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
                levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(122, 350)));
                levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 350)));
                levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(314, 350)));
                levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 350)));
                levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 350)));
                levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(602, 350)));
                levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(698, 350)));
                levels[0].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350))); //adding platforms to the first level (level 0)

                levels.Add(new List<PlatformSprite>());  //adding the list of platforms
                levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
                levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(160, 270)));
                levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(255, 180)));
                levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(190, 420)));
                levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(375, 420)));
                levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(520, 350)));
                levels[1].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(730, 350))); //adding platforms to the second level (level 1)

                levels.Add(new List<PlatformSprite>());  //adding the list of platforms
                levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
                levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(-45, 260)));
                levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(100, 190)));
                levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(170, 400)));
                levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(320, 350)));
                levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(480, 320)));
                levels[2].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(730, 350))); //adding platforms to the third level (level 2)

                levels.Add(new List<PlatformSprite>());  //adding the list of platforms
                levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
                levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(205, 350)));
                levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(335, 275)));
                levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(205, 180)));
                levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 180)));
                levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(550, 275)));
                levels[3].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(730, 350))); //adding platforms to the fourth level (level 3)

                levels.Add(new List<PlatformSprite>());  //adding the list of platforms
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
                levels[4].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(550, 30))); //adding platforms to the fifth level (level 4)

                levels.Add(new List<PlatformSprite>());  //adding the list of platforms
                levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(26, 350)));
                levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 300)));
                levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 250)));
                levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(506, 250)));
                levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(218, 478)));
                levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(410, 478)));
                levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(570, 410)));
                levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(698, 350)));
                levels[5].Add(new PlatformSprite(Lvl1PlatformTxr, whiteBox, new Vector2(794, 350))); //adding platforms to the sixth level (level 5)

        }
    }
}
