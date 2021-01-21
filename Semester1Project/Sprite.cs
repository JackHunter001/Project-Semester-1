using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Semester1Project
{
    class Sprite
    {
        Texture2D spriteSheet, collisionTexture;

        public Vector2 spritePos, spriteVelocity, spriteOrigin, spriteScale, collisionInsetMin, collisionInsetMax;

        public bool flipped, isDead, isColliding, drawCollision;

        int collPadding = 5;

        public List<List<Rectangle>> animations; 
        public float frameTime, frameCounter;
        public int currentFrame, currentAnim;

        public Sprite(Texture2D newSpriteSheet, Texture2D newCollisionTexture, Vector2 newLocation)
        {
            spriteSheet = newSpriteSheet;
            collisionTexture = newCollisionTexture;
            spritePos = newLocation;

            isColliding = false;
            drawCollision = false;
            isDead = false;
            flipped = false;
            spriteOrigin = new Vector2(0f, 0f);
            collisionInsetMin = new Vector2(0f, 0f);
            collisionInsetMax = new Vector2(0f, 0f);
            spriteScale = new Vector2(1f, 1f);
            currentAnim = 0;
            currentFrame = 0;
            frameTime = 0.5f;
            frameCounter = frameTime;

            animations = new List<List<Rectangle>>();
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 48));
        }

        public virtual void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch, GameTime gametime)
        {
            if (animations[currentAnim].Count > 1)
            {
                frameCounter -= (float)gametime.ElapsedGameTime.TotalSeconds;
                if (frameCounter <= 0)
                {
                    frameCounter = frameTime;
                    currentFrame++;
                }
                if (currentFrame >= animations[currentAnim].Count) currentFrame = 0;
            }

            SpriteEffects drawEffect;
            if (flipped) drawEffect = SpriteEffects.FlipHorizontally;
            else drawEffect = SpriteEffects.None;

            spriteBatch.Draw(
                spriteSheet,                           
                getRectangleForDraw(),                  
                animations[currentAnim][currentFrame],  
                Color.White,                            
                0f,                                     
                spriteOrigin,                           
                drawEffect,                             
                1f); 

            if (drawCollision) spriteBatch.Draw(
                collisionTexture,                       
                getRectangleForCollision(),             
                Color.Red);                            
        }
        public void setAnim(int newAnim)
        {
            if (currentAnim != newAnim && newAnim < animations.Count) 
            {
                currentAnim = newAnim;
                currentFrame = 0;
                frameCounter = frameTime;
            }
        }
        public bool checkCollision(Sprite otherSprite)
        {
            if (!isColliding || !otherSprite.isColliding) return false; 
            else return getRectangleForCollision().Intersects(otherSprite.getRectangleForCollision()); 
        }
        public bool checkCollisionBelow(Sprite otherSprite)
        {
            return checkCollision(otherSprite) && getEdgeBottomForCollision() < otherSprite.getEdgeBottomForCollision() && getEdgeBottomForCollision() > otherSprite.getEdgeTopForCollision()
                && (getEdgeLeftForCollision() + collPadding < otherSprite.getEdgeRightForCollision() && getEdgeRightForCollision() - collPadding > otherSprite.getEdgeLeftForCollision());
        }
        public bool checkCollisionAbove(Sprite otherSprite)
        {
            return checkCollision(otherSprite) && getEdgeTopForCollision() > otherSprite.getEdgeTopForCollision() && getEdgeTopForCollision() < otherSprite.getEdgeBottomForCollision()
                && (getEdgeLeftForCollision() + collPadding < otherSprite.getEdgeRightForCollision() && getEdgeRightForCollision() - collPadding > otherSprite.getEdgeLeftForCollision());
        }
        public bool checkCollisionLeft(Sprite otherSprite)
        {
            return checkCollision(otherSprite) && getEdgeRightForCollision() < otherSprite.getEdgeRightForCollision() && getEdgeRightForCollision() > otherSprite.getEdgeLeftForCollision()
                && (getEdgeTopForCollision() + collPadding < otherSprite.getEdgeBottomForCollision() && getEdgeBottomForCollision() - collPadding > otherSprite.getEdgeTopForCollision());
        }
        public bool checkCollisionRight(Sprite otherSprite)
        {
            return checkCollision(otherSprite) && getEdgeLeftForCollision() > otherSprite.getEdgeLeftForCollision() && getEdgeLeftForCollision() < otherSprite.getEdgeRightForCollision()
                && (getEdgeTopForCollision() + collPadding < otherSprite.getEdgeBottomForCollision() && getEdgeBottomForCollision() - collPadding > otherSprite.getEdgeTopForCollision());
        }
        public Rectangle getRectangleForDraw()
        {
            return new Rectangle(getEdgeLeftForDraw(), getEdgeTopForDraw(), getWidthForDraw(), getHeightForDraw());
        }
        public int getWidthForDraw()
        {
            return (int)(animations[currentAnim][currentFrame].Width * spriteScale.X);
        }
        public int getEdgeLeftForDraw()
        {
            return (int)(spritePos.X - (getWidthForDraw() * spriteOrigin.X));
        }
        public int getEdgeRightForDraw()
        {
            return getEdgeLeftForDraw() + getWidthForDraw();
        }
        public int getHeightForDraw()
        {
            return (int)(animations[currentAnim][currentFrame].Height * spriteScale.Y);
        }
        public int getEdgeTopForDraw()
        {
            return (int)(spritePos.Y - (getHeightForDraw() * spriteOrigin.Y));
        }
        public int getEdgeBottomForDraw()
        {
            return getEdgeTopForDraw() + getHeightForDraw();
        }
        public Rectangle getRectangleForCollision()
        {
            return new Rectangle(getEdgeLeftForCollision(), getEdgeTopForCollision(), getWidthForCollision(), getHeightForCollision());
        }
        public int getWidthForCollision()
        {
            return getEdgeRightForCollision() - getEdgeLeftForCollision();
        }
        public int getEdgeLeftForCollision()
        {
            return getEdgeLeftForDraw() + (int)(collisionInsetMin.X * getWidthForDraw());
        }
        public int getEdgeRightForCollision()
        {
            return getEdgeRightForDraw() - (int)(collisionInsetMax.X * getWidthForDraw());
        }
        public int getHeightForCollision()
        {
            return getEdgeBottomForCollision() - getEdgeTopForCollision();
        }
        public int getEdgeTopForCollision()
        {
            return getEdgeTopForDraw() + (int)(collisionInsetMin.Y * getHeightForDraw());
        }
        public int getEdgeBottomForCollision()
        {
            return getEdgeBottomForDraw() - (int)(collisionInsetMax.Y * getHeightForDraw());
        }

        public Vector2 getCentreForCollision()
        {
            return new Vector2(getEdgeLeftForCollision() + (getWidthForCollision() * 0.5f), getEdgeTopForCollision() + (getHeightForCollision() * 0.5f));
        }
    }
}
