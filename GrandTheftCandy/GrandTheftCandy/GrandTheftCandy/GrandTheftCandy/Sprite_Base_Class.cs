/*
 * TODO:
*/

/*
 * Notes: DrawOrder is calculated by where a sprite's center is.
 * An offset of 5 is the minimum for movable sprites.
 * This allows for 5 levels for background objects.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GrandTheftCandy
{
   class Sprite_Base_Class : DrawableGameComponent
   {
      #region Member Variables

      protected string m_textureFileName;
      protected Texture2D m_textureImage;
      protected Color m_spriteRenderColor;
      protected Vector2 m_spriteCenter;
      protected Vector2 m_spritePosition;
      protected bool m_isSpriteCollidable;
      protected String m_spriteName;

      #endregion

      #region Constructors

      /// <summary>
      /// Basic constructor for sprites. Any objects that need to be drawn should use this constructor.
      /// </summary>
      /// <param name="a_game"></param>
      /// <param name="a_textureFileName"></param>
      /// <param name="a_startingPosition"></param>
      /// <param name="a_renderColor"></param>
      /// <param name="a_collidable"></param>
      public Sprite_Base_Class(Game a_game, String a_textureFileName, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName)
         : base(a_game)
      {
         m_textureFileName = a_textureFileName;
         m_spritePosition = a_startingPosition;
         this.DrawOrder = (((int)m_spritePosition.Y - 200) / 5) + 5;
         m_spriteRenderColor = a_renderColor;
         m_isSpriteCollidable = a_collidable;
         m_spriteName = a_SpriteName;
         a_game.Components.Add(this);
      }

      /// <summary>
      /// Special constructor for when the draw order needs to be specified.
      /// An example of this is the floor or background walls.
      /// </summary>
      /// <param name="a_game"></param>
      /// <param name="a_textureFileName"></param>
      /// <param name="a_startingPosition"></param>
      /// <param name="a_collidable"></param>
      /// <param name="a_drawOrder"></param>
      public Sprite_Base_Class (Game a_game, String a_textureFileName, Vector2 a_startingPosition, 
         bool a_collidable, int a_drawOrder, String a_SpriteName)
         : base (a_game)
      {
         m_textureFileName = a_textureFileName;
         m_spritePosition = a_startingPosition;
         this.DrawOrder = a_drawOrder;
         m_spriteRenderColor = Color.White;
         m_isSpriteCollidable = a_collidable;
         m_spriteName = a_SpriteName;
         a_game.Components.Add (this);
      }

      #endregion

      #region Getters and Setters

      public Vector2 spriteCenter
      {
         get
         {
               return m_spriteCenter;
         }
         set
         {
            m_spriteCenter = spriteCenter;
         }
      }

      public Vector2 spritePosition
      {
         get
         {
            return m_spritePosition;
         }
         set
         {
            m_spritePosition = spritePosition;
         }
      }

      public String textureFileName
      {
         get
         {
            return m_textureFileName;
         }
         set
         {
            m_textureFileName = textureFileName;
         }
        }

      public String spriteName
      {
         get
         {
            return m_textureFileName;
         }
         set
         {
            m_spriteName = spriteName;
         }
      }

      public bool isSpriteCollidable
      {
         get
         {
            return m_isSpriteCollidable;
         }
         set
         {
            m_isSpriteCollidable = isSpriteCollidable;
         }
      }

      /// <summary>
      /// Returns the bounding box for the sprite.
      /// </summary>
      public Rectangle boundingBox
      {
         get
         {
            return new Rectangle ((int)(m_spritePosition.X - m_spriteCenter.X),
               (int)(m_spritePosition.Y - m_spriteCenter.Y),
               m_textureImage.Width, m_textureImage.Height);
         }
      }

      #endregion

      #region Overridden Functions

      protected override void LoadContent()
      {
         m_textureImage = Game.Content.Load<Texture2D>(m_textureFileName);
         m_spriteCenter = new Vector2 (m_textureImage.Width * 0.5f, m_textureImage.Height * 0.5f);

         base.LoadContent();
      }

      public override void Initialize()
      {
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         SpriteBatch sb = ((GTC_Level1)this.Game).spriteBatch;

         sb.Begin();
         sb.Draw(m_textureImage, m_spritePosition, null, m_spriteRenderColor, 0f, m_spriteCenter,
            1.0f, SpriteEffects.None, 0f);
         sb.End();

         base.Draw(gameTime);
      }

      #endregion

      #region Functions

      public bool isWithinSpriteBoundry (Sprite_Base_Class a_sprite)
      {
         if (this.boundingBox.Intersects (a_sprite.boundingBox))
         {
            return true;
         }
         return false;
      }

      /// <summary>
      /// Determines if two sprites collide when on the same srawing level.
      /// </summary>
      /// <param name="a_sprite"></param>
      /// <returns></returns>
      public bool collidesHorizontally (Sprite_Base_Class a_sprite)
      {
         if (this.DrawOrder == a_sprite.DrawOrder && a_sprite.isSpriteCollidable)
         {
            return isWithinSpriteBoundry (a_sprite);
         }
         return false;
      }

      public bool collidesWithBelow (Sprite_Base_Class a_sprite)
      {
         if ((a_sprite.DrawOrder < this.DrawOrder) && a_sprite.isSpriteCollidable)
         {
            return isWithinSpriteBoundry (a_sprite);
         }
         return false;
      }

      public bool collidesWithAbove (Sprite_Base_Class a_sprite)
      {
         if ((a_sprite.DrawOrder > this.DrawOrder) && a_sprite.isSpriteCollidable)
         {
            return isWithinSpriteBoundry (a_sprite);
         }
         return false;
      }

      #endregion

   } // End Sprite_Base_Class.

   class Player_Controlled_Sprite : Sprite_Base_Class
   {
      #region Member Variables

      #endregion

      #region Constructors

      public Player_Controlled_Sprite (Game a_game, String a_textureFileName, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName)
         : base(a_game, a_textureFileName, a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {

      }

      #endregion

      #region Getters and Setters

      #endregion

      #region Overridden Functions

      protected override void LoadContent()
      {
         base.LoadContent();
      }

      public override void Initialize()
      {
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         KeyboardState keyboardInput = Keyboard.GetState();

         #region Move Down
         if (keyboardInput.IsKeyDown(Keys.S) || keyboardInput.IsKeyDown(Keys.Down)) 
         {
            if(m_spritePosition.Y < (this.GraphicsDevice.Viewport.Height - 5))
            {
               m_spritePosition.Y += 5;
               this.DrawOrder++;
               Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
               this.Game.Components.CopyTo (spriteList, 0);
               for (int i = 0; i < spriteList.Length; i++)
               {
                  if (this.collidesHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
                  {
                     m_spritePosition.Y -= 5;
                     this.DrawOrder--;
                  }
               }
            }
         }
         #endregion

         #region Move Left
         if (keyboardInput.IsKeyDown(Keys.A) || keyboardInput.IsKeyDown(Keys.Left))
         {
            m_spritePosition.X -= 5;
            Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
            this.Game.Components.CopyTo (spriteList, 0);
            for(int i = 0; i < spriteList.Length; i++)
            {
               if(this.collidesHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
               {
                  m_spritePosition.X += 5;
               }
            }
         }
         #endregion

         #region Move Right
         if (keyboardInput.IsKeyDown(Keys.D) || keyboardInput.IsKeyDown(Keys.Right))
         {
            m_spritePosition.X += 5;
            Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
            this.Game.Components.CopyTo (spriteList, 0);
            for(int i = 0; i < spriteList.Length; i++)
            {
               if(this.collidesHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
               {
                  m_spritePosition.X -= 5;
               }
            }
         }
         #endregion

         #region Move Up
         if (keyboardInput.IsKeyDown(Keys.W) || keyboardInput.IsKeyDown(Keys.Up))
         {
            m_spritePosition.Y -= 5;
            this.DrawOrder--;
            Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
            this.Game.Components.CopyTo (spriteList, 0);
            for(int i = 0; i < spriteList.Length; i++)
            {
               if(this.collidesHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
               {
                  m_spritePosition.Y += 5;
                  this.DrawOrder++;
               }
            }
         }
         #endregion

         base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         base.Draw(gameTime);
      }

      #endregion

      #region Functions

      #endregion

   } // End Player_Controlled_Sprite Class.

   class Splash_Screen : Sprite_Base_Class
   {
      #region Member Variables

      #endregion

      #region Constructors

      public Splash_Screen (Game a_game, String a_textureFileName, Vector2 a_startingPosition, Color a_renderColor, String a_SpriteName)
         : base(a_game, a_textureFileName, a_startingPosition, a_renderColor, false, a_SpriteName)
      {
         this.DrawOrder = 500; 
      }

      #endregion

      #region Getters and Setters

      #endregion

      #region Overridden Functions

      protected override void LoadContent()
      {
         base.LoadContent();
      }

      public override void Initialize()
      {
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         MouseState mouse = Mouse.GetState();

         if (mouse.LeftButton == ButtonState.Pressed)
         {
            this.Visible = false;
            this.DrawOrder = 100;
         }

         base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         base.Draw(gameTime);
      }

      #endregion

      #region Functions

      #endregion
   } // End Splash_Screen Class.
}