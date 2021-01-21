using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace Semester1Project
{
    class BrainEnemySprite2 : Sprite
    {
        public BrainEnemySprite2(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation) : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            spriteOrigin = new Vector2(0.5f, 0.5f); //setting the origin of the sprite
            isColliding = true; //defaulting the variable 'isColliding' to true

            animations = new List<List<Rectangle>>(); //creating a list of lists of rectangles
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(48, 48, 48, 48));
            animations[0].Add(new Rectangle(96, 48, 48, 48));
            animations[0].Add(new Rectangle(144, 48, 48, 48));
            animations[0].Add(new Rectangle(96, 48, 48, 48)); //adding these coordinate rectangle cutouts from the spritesheet to the animations list
        }
    }
}

