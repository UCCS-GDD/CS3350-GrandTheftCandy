/*
 * TODO: See the TODO near the Animated_Sprite class
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
      protected int m_SpriteWidth;
      protected int m_SpriteHeight;

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
         if(a_textureFileName != null)
         {
            m_textureFileName = a_textureFileName;
         }
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
               m_SpriteWidth, m_SpriteHeight);
         }
      }

      #endregion

      #region Overridden Functions

      protected override void LoadContent()
      {
         if (m_textureFileName != null)
         {
            m_textureImage = Game.Content.Load<Texture2D> (m_textureFileName);
            m_spriteCenter = new Vector2 (m_textureImage.Width * 0.5f, m_textureImage.Height * 0.5f);
            m_SpriteHeight = m_textureImage.Height;
            m_SpriteWidth = m_textureImage.Width;
         }

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
         if (m_textureImage != null)
         {
            SpriteBatch sb = ((GTC_Level1)this.Game).spriteBatch;

            sb.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, ((GTC_Level1)this.Game).cameraPosition);
            sb.Draw (m_textureImage, m_spritePosition, null, m_spriteRenderColor, 0f, m_spriteCenter,
               1.0f, SpriteEffects.None, 0f);
            sb.End ();         }

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

   // TODO: Finish animation class and make the NPC class inherit from it.
   class Animated_Sprite : Sprite_Base_Class
   {
      #region Member Variables

      private String[] m_AnimatedTextureNames;
      private Texture2D[] m_AnimatedSprites;
      /// <summary>
      /// An intiger array for storing when a sprite animation ends.
      /// </summary>
      private int[] m_SpriteAnimationSequence;

      #endregion

      #region Constructors

      public Animated_Sprite(Game a_game, String[] a_textureFileNames, int[] a_SpriteAnimationSequence, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName)
         : base (a_game, null, a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {
         m_AnimatedTextureNames = a_textureFileNames;
         m_SpriteAnimationSequence = a_SpriteAnimationSequence;
      }

      #endregion

      #region Getters and Setters

      #endregion

      #region Overridden Function

      protected override void LoadContent ()
      {
         for(int i = 0; i < m_AnimatedTextureNames.Length; i++)
         {
            m_AnimatedSprites[i] = Game.Content.Load<Texture2D> (m_AnimatedTextureNames[i]);
         }

         base.LoadContent ();
      }

      public override void Initialize ()
      {
         base.Initialize ();
      }

      public override void Update (GameTime gameTime)
      {
         base.Update (gameTime);
      }

      public override void Draw (GameTime gameTime)
      {
         //SpriteBatch sb = ((GTC_Level1) this.Game).spriteBatch;

         //sb.Begin ();
         //sb.Draw (m_textureImage, m_spritePosition, null, m_spriteRenderColor, 0f, m_spriteCenter,
         //   1.0f, SpriteEffects.None, 0f);
         //sb.End ();

         base.Draw (gameTime);
      }

      #endregion

      #region Functions

      #endregion
   } // End Animated_Sprite Class.

   class Player_Controlled_Sprite : Sprite_Base_Class
   {
      #region Member Variables

      private bool m_MovementAllowed;

      #endregion

      #region Constructors

      public Player_Controlled_Sprite (Game a_game, String a_textureFileName, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName)
         : base(a_game, a_textureFileName, a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {
         m_MovementAllowed = false;
      }

      #endregion

      #region Getters and Setters

      public bool movementAllowed
      {
         get
         {
            return m_MovementAllowed;
         }
         set
         {
            m_MovementAllowed = value;
         }
      }

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
         if (m_MovementAllowed)
         {
            KeyboardState keyboardInput = Keyboard.GetState ();

            #region Move Down
            if (keyboardInput.IsKeyDown (Keys.S) || keyboardInput.IsKeyDown (Keys.Down))
            {
               if (m_spritePosition.Y < (this.GraphicsDevice.Viewport.Height - 5))
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
            if (keyboardInput.IsKeyDown (Keys.A) || keyboardInput.IsKeyDown (Keys.Left))
            {
               if (m_spritePosition.X - (m_textureImage.Width / 2) > 0)
               {
                  m_spritePosition.X -= 5;
                  Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
                  this.Game.Components.CopyTo (spriteList, 0);
                  for (int i = 0; i < spriteList.Length; i++)
                  {
                     if (this.collidesHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
                     {
                        m_spritePosition.X += 5;
                     }

                     if (m_spritePosition.X > 400 && m_spritePosition.X < 2600)
                     {
                        ((GTC_Level1)this.Game).cameraPosition = Matrix.CreateTranslation (400 - m_spritePosition.X, 0, 0);
                     }
                     else if (m_spritePosition.X < 400)
                     {
                        ((GTC_Level1)this.Game).cameraPosition = Matrix.CreateTranslation (0, 0, 0);
                     }
                  }
               }
            }
            #endregion

            #region Move Right
            if (keyboardInput.IsKeyDown (Keys.D) || keyboardInput.IsKeyDown (Keys.Right))
            {
               if (m_spritePosition.X < 2600)
               {
                  m_spritePosition.X += 5;
                  Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
                  this.Game.Components.CopyTo (spriteList, 0);
                  for (int i = 0; i < spriteList.Length; i++)
                  {
                     if (this.collidesHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
                     {
                        m_spritePosition.X -= 5;
                     }

                     if (m_spritePosition.X < 2600 && m_spritePosition.X > 400)
                     {
                        ((GTC_Level1)this.Game).cameraPosition = Matrix.CreateTranslation (400 - m_spritePosition.X, 0, 0);
                     }
                     else if (m_spritePosition.X > 2600)
                     {
                        ((GTC_Level1)this.Game).cameraPosition = Matrix.CreateTranslation (2600, 0, 0);
                     }
                  }
               }
            }
            #endregion

            #region Move Up
            if (keyboardInput.IsKeyDown (Keys.W) || keyboardInput.IsKeyDown (Keys.Up))
            {
               m_spritePosition.Y -= 5;
               this.DrawOrder--;
               Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
               this.Game.Components.CopyTo (spriteList, 0);
               for (int i = 0; i < spriteList.Length; i++)
               {
                  if (this.collidesHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
                  {
                     m_spritePosition.Y += 5;
                     this.DrawOrder++;
                  }
               }
            }
            #endregion
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

   } // End Player_Controlled_Sprite Class.

   /// <summary>
   /// For the mother NPC pass in true for the boolean and give two sprite names for the textures as an array in a_textureFileNames.
   /// The first is the sprite with the baby holding the candy, the second should have the candy missing.
   /// The sprite chosen to draw will be automatic based on if the mother currently has candy.
   /// You can set "hasCandy" to true manually but if the NPC has the boolean of "isMother" as true,
   /// the contructor will set "hasCandy" to true. The manual set is for when you steal the candy.
   /// 
   /// For a guard, just pass in a single sprite name and leave the second as null.
   /// </summary>
   class NPC_Base_Class : Sprite_Base_Class
   {
      #region Member Variables

      private String[] m_SpriteVersionTextureNames;
      private Texture2D[] m_SpriteVersions;
      private bool m_IsMother;
      private bool m_HasCandy;

      #endregion

      #region Constructors

      public NPC_Base_Class (Game a_game, String[] a_textureFileNames, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName, bool a_IsMother)
         : base (a_game, null, a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {
         m_IsMother = a_IsMother;
         if(m_IsMother)
         {
            m_HasCandy = true;
         }
         m_SpriteVersionTextureNames = a_textureFileNames;
         m_SpriteVersions = new Texture2D[2];
      }

      #endregion

      #region Getters and Setters

      public bool hasCandy
      {
         get
         {
            return m_HasCandy;
         }
         set
         {
            m_HasCandy = value;
         }
      }

      public bool isMother
      {
         get
         {
            return m_IsMother;
         }
         set
         {
            m_IsMother = value;
         }
      }

      #endregion

      #region Overridden Function

      protected override void LoadContent ()
      {
         if(m_IsMother)
         {
            for(int i = 0; i < m_SpriteVersionTextureNames.Length; i++)
            {
               m_SpriteVersions[i] = Game.Content.Load<Texture2D> (m_SpriteVersionTextureNames[i]);
            }
         }
         else
         {
            m_SpriteVersions[0] = Game.Content.Load<Texture2D> (m_SpriteVersionTextureNames[0]);
         }

         m_spriteCenter = new Vector2 (m_SpriteVersions[0].Height * .5f, m_SpriteVersions[0].Width * .5f);
         m_SpriteHeight = m_SpriteVersions[0].Height;
         m_SpriteWidth = m_SpriteVersions[0].Width;

         base.LoadContent ();
      }

      public override void Initialize ()
      {
         base.Initialize ();
      }

      public override void Update (GameTime gameTime)
      {
         base.Update (gameTime);
      }

      public override void Draw (GameTime gameTime)
      {
         SpriteBatch sb = ((GTC_Level1) this.Game).spriteBatch;

         sb.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, ((GTC_Level1)this.Game).cameraPosition);
         // If the mother has candy use the sprite that has the baby holding candy.
         if (m_IsMother)
         {
            if (m_HasCandy)
            {
               sb.Draw (m_SpriteVersions[0], m_spritePosition, null, m_spriteRenderColor, 0f, m_spriteCenter,
                  1.0f, SpriteEffects.None, 0f);
            }
            else
            {
               sb.Draw (m_SpriteVersions[1], m_spritePosition, null, m_spriteRenderColor, 0f, m_spriteCenter,
                  1.0f, SpriteEffects.None, 0f);
            }
         }
         else
         {
            sb.Draw (m_SpriteVersions[0], m_spritePosition, null, m_spriteRenderColor, 0f, m_spriteCenter,
                  1.0f, SpriteEffects.None, 0f);
         }
         sb.End ();

         base.Draw (gameTime);
      }

      #endregion

      #region Functions

      #endregion
   }

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
            this.Game.IsMouseVisible = false;
            ((Player_Controlled_Sprite)((GTC_Level1)this.Game).Components[0]).movementAllowed = true;
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