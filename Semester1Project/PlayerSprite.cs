using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Semester1Project
{
    class PlayerSprite : Sprite
    {
        bool jumping, walking, falling, jumpIsPressed; //creating boolean variables
        const float jumpSpeed = 4f; //setting a constant float for jumpspeed
        const float walkSpeed = 100f; //setting a constant float for walkspeed
        public int lives = 3; //setting a public integer for lives
        public int coinsCollected = 0; //setting a public integer for coinscollected
        SoundEffect jumpSound; //passing in the jump sound effect

        public PlayerSprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation, SoundEffect newJumpSound) : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            jumpSound = newJumpSound; //assigning the jump sound to a new variable

            spriteOrigin = new Vector2(0.5f, 1f); //setting the origin of the sprite
            isColliding = true; //setting iscolliding variable to true

            //drawCollision = true; //to check the collison hitbox of the player

            collisionInsetMin = new Vector2(0.25f, 0.3f); //setting the collsion box vectors minimum values
            collisionInsetMax = new Vector2(0.25f, 0.03f); //setting the collsion box vectors maximum values

            frameTime = 0.1f; //setting the framerate
            animations = new List<List<Rectangle>>(); //creating a list of lists of rectangles for the animations variable

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 48)); 
            animations[0].Add(new Rectangle(0, 0, 48, 48)); 
            animations[0].Add(new Rectangle(0, 0, 48, 48)); 
            animations[0].Add(new Rectangle(48, 0, 48, 48));
            animations[0].Add(new Rectangle(48, 0, 48, 48));
            animations[0].Add(new Rectangle(48, 0, 48, 48)); //adding these rectangles from the spritesheet to the animations list

            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 48, 48, 48));
            animations[1].Add(new Rectangle(0, 48, 48, 48));
            animations[1].Add(new Rectangle(144, 0, 48, 48));
            animations[1].Add(new Rectangle(96, 0, 48, 48));
            animations[1].Add(new Rectangle(96, 0, 48, 48));
            animations[1].Add(new Rectangle(144, 0, 48, 48)); //adding these rectangles from the spritesheet to the animations list

            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(96, 0, 48, 48)); //adding this rectangle from the spritesheet to the animations list

            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(48, 48, 48, 48)); //adding this rectangle from the spritesheet to the animations list

            jumping = false; //jumping varibale equals false
            walking = false; //walking varibale equals false
            falling = true; //falling varibale equals true
            jumpIsPressed = false; //jumpispressed variable equals false
        }

        public void Update(GameTime gameTime, List<PlatformSprite> platforms)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (!jumpIsPressed && !jumping && !falling && (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.A))) //checking to see if any of these buttons are being pressed
            {
                jumpIsPressed = true;
                jumping = true;
                walking = false;
                falling = false;
                spriteVelocity.Y -= jumpSpeed; //allowing the character to jump
                jumpSound.Play(); //plays the jump sound effect
            }

            else if (jumpIsPressed && !jumping && !falling && !(keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.A)))
            {
                jumpIsPressed = false;
            } //if one of the jump keys/buttons is pressed and the character is already not jumping or falling then jumpispressed variable is false

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left) || gamePadState.IsButtonDown(Buttons.DPadLeft))
            {
                walking = true;
                spriteVelocity.X = -walkSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                flipped = true;
            } //if one of the keys/buttons for left is pressed then walking equals true and the character sprite is flipped along with the calculation for moving left
            else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right) || gamePadState.IsButtonDown(Buttons.DPadRight))
            {
                walking = true;
                spriteVelocity.X = walkSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                flipped = false;
            } //if one of the keys/buttons for left is pressed then walking equals true and the character sprite is flipped along with the calculation for moving left
            else
            {
                walking = false;
                spriteVelocity.X = 0;
            } //if none of the above conditions are met then walking is false and isnt moving (idle)

            if ((falling || jumping) && spriteVelocity.Y < 500f) spriteVelocity.Y += 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            spritePos += spriteVelocity;

            bool hasCollided = false;

            foreach (PlatformSprite platform in platforms)
            {
                if (checkCollisionBelow(platform))
                {
                    hasCollided = true;
                    while (checkCollision(platform)) spritePos.Y--;
                    spriteVelocity.Y = 0;
                    jumping = false;
                    falling = false;
                } //if the platform is below the player then hascollided equals true along with a collision detected at minus one of the player(Vertically) and the player aint falling or jumping
                else if (checkCollisionAbove(platform))
                {
                    hasCollided = true;
                    while (checkCollision(platform)) spritePos.Y++;
                    spriteVelocity.Y = 0;
                    jumping = false;
                    falling = true;
                } //if the platform is above the player then hascollided equals true along with a collision detected at plus one of the player(Vertically) and the player aint jumping but is falling

                if (checkCollisionLeft(platform))
                {
                    hasCollided = true;
                    while (checkCollision(platform)) spritePos.X--;
                    spriteVelocity.X = 0;
                } //if the platform is to the left of the player then hascollided equals true along with a collision detected at minus one of the player(Horizontally) and the player aint moving in that direction
                else if (checkCollisionRight(platform))
                {
                    hasCollided = true;
                    while (checkCollision(platform)) spritePos.X++;
                    spriteVelocity.X = 0;
                } //if the platform is to the right of the player then hascollided equals true along with a collision detected at plus one of the player(Horizontally) and the player aint moving in that direction

                if (!hasCollided && walking) falling = true; //if the player is not colliding with anything and is walking then falling equals true
                if (jumping && spriteVelocity.Y > 0)
                {
                    jumping = false;
                    falling = true;
                } //if the player is jumping and is in the air then falling equals true while jumping equals false

                if (walking) setAnim(1); 
                else if (falling) setAnim(3); 
                else if (jumping) setAnim(2); 
                else setAnim(0); //setting the different animations depending on the player state
            }
        }

        public void ResetPlayer(Vector2 newPos)
        {
            spritePos = newPos; //setting a new player position
            spriteVelocity = new Vector2(); //creating an empty vector
            jumping = false; //setting jumping to false
            walking = false; //setting walking to false
            falling = true; //setting falling to true
        }
    }
}
