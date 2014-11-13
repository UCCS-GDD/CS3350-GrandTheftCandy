/*
 * TODO: See the TODO near the Animated_Sprite class
 * TODO: Calculate Sprite Width by the texture width by the number of animations.
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
   public enum spriteAnimationSequence
   {DownStill, DownMoving, LeftStill, LeftMoving, RightStill, RightMoving, UpStill, UpMoving};

   public class Sprite_Base_Class : DrawableGameComponent
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
         calculateDrawOrder ();
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

      public Color spriteColor
      {
         set
         {
            m_spriteRenderColor = value;
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

      public Rectangle halfWidthBoundingBox
      {
         get
         {
            return new Rectangle ((int)(m_spritePosition.X - (m_spriteCenter.X/2)),
               (int)(m_spritePosition.Y - m_spriteCenter.Y),
               (m_SpriteWidth/2), m_SpriteHeight);
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
         if (((GTC_Level1)this.Game).gameNotPaused)
         {
            base.Update (gameTime);
         }
      }

      public override void Draw(GameTime gameTime)
      {
         if (m_textureImage != null)
         {
            SpriteBatch sb = ((GTC_Level1)this.Game).spriteBatch;

            sb.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, ((GTC_Level1)this.Game).cameraPosition);
            sb.Draw (m_textureImage, m_spritePosition, null, m_spriteRenderColor, 0f, m_spriteCenter,
               1.0f, SpriteEffects.None, 0f);
            sb.End ();
         }

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

      public bool isWithinHalfSpriteBoundry (Sprite_Base_Class a_sprite)
      {
         if (this.halfWidthBoundingBox.Intersects (a_sprite.halfWidthBoundingBox))
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

      public bool collidesHalfHorizontally (Sprite_Base_Class a_sprite)
      {
         if (this.DrawOrder == a_sprite.DrawOrder && a_sprite.isSpriteCollidable)
         {
            return isWithinHalfSpriteBoundry (a_sprite);
         }
         return false;
      }

      public bool collidesWithBelow (Sprite_Base_Class a_sprite)
      {
         if ((a_sprite.DrawOrder < this.DrawOrder && a_sprite.DrawOrder + 5 > this.DrawOrder) && a_sprite.isSpriteCollidable)
         {
            return isWithinHalfSpriteBoundry (a_sprite);
         }
         return false;
      }

      public bool collidesWithAbove (Sprite_Base_Class a_sprite)
      {
         if ((a_sprite.DrawOrder > this.DrawOrder && a_sprite.DrawOrder - 5 < this.DrawOrder) && a_sprite.isSpriteCollidable)
         {
            return isWithinHalfSpriteBoundry (a_sprite);
         }
         return false;
      }

      public bool collides (Sprite_Base_Class a_sprite)
      {
         bool collidesWith = collidesWithAbove (a_sprite) || collidesWithBelow (a_sprite);
         if (collidesHorizontally(a_sprite))
         {
            collidesWith = collidesWith || collidesHalfHorizontally (a_sprite);
         }
         return collidesWith;
      }

      /// <summary>
      /// Returns if the provided sprite is above this sprite.
      /// </summary>
      /// <param name="a_sprite"></param>
      /// <returns></returns>
      public bool isSpriteAbove (Sprite_Base_Class a_sprite)
      {
         if (this.m_spritePosition.Y < a_sprite.m_spritePosition.Y)
         {
            return true;
         }
         return false;
      }

      public bool isSpriteBelow (Sprite_Base_Class a_sprite)
      {
         if (this.m_spritePosition.Y > a_sprite.m_spritePosition.Y)
         {
            return true;
         }
         return false;
      }

      protected void calculateDrawOrder ()
      {
         this.DrawOrder = (((int)m_spritePosition.Y - 200) / 5) + 5;
      }

      #endregion

   } // End Sprite_Base_Class.

   // TODO: Tie in NPC Class to Animated Sprites
   /// <summary>
   /// When providing the file names for the animated textures,
   /// provide them in the same order as the enum spriteAnimationSequence.
   /// </summary>
   public class Animated_Sprite : Sprite_Base_Class
   {
      #region Member Variables

      private bool m_DrawThisFrame;
      private spriteAnimationSequence m_CurrentAnimation;
      private int m_CurrentAnimationSequence;
      private Rectangle m_CurrentDrawRectangle;
      protected Vector2 m_MovementSpeed;
      private Vector2 m_PreviousMovement;
      protected Vector2 m_CurrentMovement;
      // String names for each animation.
      private String[] m_AnimatedTextureNames;
      // The texture array containing all of the animations
      protected Texture2D[] m_AnimatedSprites;
      protected int[] m_SpriteAnimationSequences;

      #endregion

      #region Constructors

      public Animated_Sprite(Game a_game, String[] a_textureFileNames, int[] a_SpriteAnimationSequence, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName)
         : base (a_game, a_textureFileNames[0], a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {
         m_AnimatedTextureNames = a_textureFileNames;
         m_AnimatedSprites = new Texture2D[m_AnimatedTextureNames.Length];
         m_SpriteAnimationSequences = a_SpriteAnimationSequence;
         m_PreviousMovement = Vector2.Zero;
         m_MovementSpeed = Vector2.Zero;
         m_CurrentAnimation = spriteAnimationSequence.LeftStill;
         m_CurrentAnimationSequence = 0;
         m_DrawThisFrame = true;
      }

      #endregion

      #region Getters and Setters

      public Vector2 movementSpeed
      {
         get
         {
            return m_MovementSpeed;
         }
         set
         {
            m_MovementSpeed = value;
         }
      }

      public Vector2 currentMovement
      {
         get
         {
            return m_CurrentMovement;
         }
         set
         {
            m_CurrentMovement = value;
         }
      }

      #endregion

      #region Overridden Function

      protected override void LoadContent ()
      {
         for(int i = 0; i < m_AnimatedTextureNames.Length; i++)
         {
            m_AnimatedSprites[i] = Game.Content.Load<Texture2D> (m_AnimatedTextureNames[i]);
         }

         // Initialize the draw rectangle with the diminsions of a "still" sprite.
         m_CurrentDrawRectangle = new Rectangle (0, 0, m_AnimatedSprites[0].Width, m_AnimatedSprites[0].Height);

         base.LoadContent ();
      }

      public override void Initialize ()
      {
         base.Initialize ();
      }

      public override void Update (GameTime gameTime)
      {
         if (((GTC_Level1)this.Game).gameNotPaused)
         {
            #region Sprite Sequence Update

            // Case 1: Previously still, now moving
            if ((m_PreviousMovement == Vector2.Zero) && (m_CurrentMovement != Vector2.Zero))
            {
               calculateCurrentMovingAnimation ();
            }

            // Case 2: Previously moving, now still
            else if ((m_CurrentMovement == Vector2.Zero) && (m_PreviousMovement != Vector2.Zero))
            {
               calculateCurrentStillAnimation ();
            }
            // Case 3: Previously moving, Still moving (Make sure the general direction is the same)
            else if ((m_CurrentMovement != Vector2.Zero) && (m_PreviousMovement != Vector2.Zero))
            {
               calculateCurrentMovingAnimation ();
            }

            // Update the previous movement for the next cycle.
            m_PreviousMovement = m_CurrentMovement;

            #endregion
         }

         base.Update (gameTime);
      }

      public override void Draw (GameTime gameTime)
      {
         SpriteBatch sb = ((GTC_Level1)this.Game).spriteBatch;

         sb.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, ((GTC_Level1)this.Game).cameraPosition);
         sb.Draw (m_AnimatedSprites[(int)m_CurrentAnimation], m_spritePosition, m_CurrentDrawRectangle, m_spriteRenderColor, 0f, m_spriteCenter,
            1.0f, SpriteEffects.None, 0f);
         sb.End ();

         if (((GTC_Level1)this.Game).gameNotPaused)
         {
            if (m_DrawThisFrame)
            {
               // If the animation sequence is a movement one, incriment to the next animation sequence and move the draw rectangle.
               if ((int)m_CurrentAnimation % 2 != 0)
               {
                  if (m_CurrentAnimationSequence < m_SpriteAnimationSequences[(int)m_CurrentAnimation])
                  {
                     m_CurrentAnimationSequence++;
                     m_CurrentDrawRectangle.Offset (m_AnimatedSprites[0].Width, 0);
                  }
                  else
                  {
                     m_CurrentAnimationSequence = 0;
                     m_CurrentDrawRectangle.X = 0;
                  }
               }
            }
            m_DrawThisFrame = !m_DrawThisFrame;
         }
      }

      #endregion

      #region Functions

      public void setMotion (Vector2 a_MovementSpeed, Vector2 a_CurrentMovement)
      {
         m_MovementSpeed = a_MovementSpeed;
         m_CurrentMovement = a_CurrentMovement;
      }

      protected void calculateCurrentMovingAnimation()
      {
         // If the sprite is moving more horizontally than vertically (or equal)
         if (Math.Abs(m_CurrentMovement.X) >= Math.Abs(m_CurrentMovement.Y))
         {
            // If the sprite is moving Left
            if (m_CurrentMovement.X < 0)
            {
               m_CurrentAnimation = spriteAnimationSequence.LeftMoving;
            }
            // Else if the sprite is moving Right
            else
            {
               m_CurrentAnimation = spriteAnimationSequence.RightMoving;
            }
         }
         // If the sprite is moving more vertically than horizontally
         else
         {
            // If the sprite is moving Up
            if (m_CurrentMovement.Y < 0)
            {
               m_CurrentAnimation = spriteAnimationSequence.UpMoving;
            }
            // Else If the sprite is moving Down
            else
            {
               m_CurrentAnimation = spriteAnimationSequence.DownMoving;
            }
         }
      }

      protected void calculateCurrentStillAnimation ()
      {
         // If the sprite was previously moving down.
         if (m_CurrentAnimation == spriteAnimationSequence.DownMoving)
         {
            m_CurrentAnimation = spriteAnimationSequence.DownStill;
         }
         else if (m_CurrentAnimation == spriteAnimationSequence.LeftMoving)
         {
            m_CurrentAnimation = spriteAnimationSequence.LeftStill;
         }
         else if (m_CurrentAnimation == spriteAnimationSequence.RightMoving)
         {
            m_CurrentAnimation = spriteAnimationSequence.RightStill;
         }
         else if (m_CurrentAnimation == spriteAnimationSequence.UpMoving)
         {
            m_CurrentAnimation = spriteAnimationSequence.UpStill;
         }

         m_CurrentDrawRectangle.X = 0;
      }

      #endregion
   } // End Animated_Sprite Class.

   public class Player_Controlled_Sprite : Animated_Sprite
   {
      #region Member Variables

      private bool m_MovementAllowed;
      private bool m_IsHidden;
      private int m_CandyCount;

      #endregion

      #region Constructors

      public Player_Controlled_Sprite (Game a_game, String[] a_textureFileNames, int[] a_SpriteAnimationSequence, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName)
         : base(a_game, a_textureFileNames, a_SpriteAnimationSequence, a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {
         m_MovementAllowed = false;
         m_IsHidden = false;
         m_CandyCount = 0;
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

      public bool isHidden
      {
         get
         {
            return m_IsHidden;
         }
         set
         {
            m_IsHidden = value;
         }
      }

      public int candyCount
      {
         get
         {
            return m_CandyCount;
         }
         set
         {
            m_CandyCount = value;
         }
      }

      public Vector2 playerPosition
      {
         get
         {
            return m_spritePosition;
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
         if (((GTC_Level1)this.Game).gameNotPaused)
         {
            if (m_MovementAllowed)
            {
               KeyboardState keyboardInput = Keyboard.GetState ();

               #region Player Movement

               Vector2 tempMovement = Vector2.Zero;

               #region Move Down
               if (keyboardInput.IsKeyDown (Keys.S) || keyboardInput.IsKeyDown (Keys.Down))
               {
                  if (m_spritePosition.Y < 950)
                  {
                     tempMovement.Y = 5;
                  }
               }
               #endregion

               #region Move Left
               if (keyboardInput.IsKeyDown (Keys.A) || keyboardInput.IsKeyDown (Keys.Left))
               {
                  if (m_spritePosition.X - (this.boundingBox.Width / 2) > 0)
                  {
                     tempMovement.X = -5;
                  }
               }
               #endregion

               #region Move Right
               if (keyboardInput.IsKeyDown (Keys.D) || keyboardInput.IsKeyDown (Keys.Right))
               {
                  if (m_spritePosition.X < 3000)
                  {
                     tempMovement.X = 5;
                  }
               }
               #endregion

               #region Move Up
               if ((keyboardInput.IsKeyDown (Keys.W) || keyboardInput.IsKeyDown (Keys.Up)) && this.spritePosition.Y > 190)
               {
                  tempMovement.Y = -5;
               }
               #endregion

               // Move the player.
               this.m_spritePosition.X += tempMovement.X;
               this.m_spritePosition.Y += tempMovement.Y;
               this.calculateDrawOrder ();

               // If the player Collides with anything, undo the mvoement.
               if (playerCollidesWithAnything ())
               {
                  this.m_spritePosition.X -= tempMovement.X;
                  this.m_spritePosition.Y -= tempMovement.Y;
                  this.calculateDrawOrder ();
               }

               #endregion

               #region Camera Movement

               Vector2 cameraTranslation = Vector2.Zero;

               #region Camera X Translation

               if (m_spritePosition.X <= 400)
               {
                  cameraTranslation.X = 0;
               }
               else if (m_spritePosition.X >= 2600)
               {
                  cameraTranslation.X = -2200;
               }
               else
               {
                  cameraTranslation.X = 400 - m_spritePosition.X;
               }

               #endregion

               #region Camera Y Translation

               if (m_spritePosition.Y <= 300)
               {
                  cameraTranslation.Y = 0;
               }
               else if (m_spritePosition.Y >= 700)
               {
                  cameraTranslation.Y = -400;
               }
               else
               {
                  cameraTranslation.Y = 300 - m_spritePosition.Y;
               }

               #endregion

               ((GTC_Level1)this.Game).cameraPosition = Matrix.CreateTranslation (cameraTranslation.X, cameraTranslation.Y, 0);

               #endregion

               m_CurrentMovement = tempMovement;
            }

            base.Update (gameTime);
         }
      }

      public override void Draw(GameTime gameTime)
      {
         base.Draw (gameTime);
      }

      #endregion

      #region Functions

      private bool playerCollidesWithAnything ()
      {
         Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
         this.Game.Components.CopyTo (spriteList, 0);
         for (int i = 0; i < spriteList.Length; i++)
         {
            if (this.collidesHalfHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
            {
               return true;
            }
         }
         return false;
      }

      #endregion

   } // End Player_Controlled_Sprite Class.


   // TODO: 
   // Inherit from Animated Class and redo constructor.
   // Add temporaty destination. (Temporary path?)
   // Add a function to set/remove the temporaty destination.
   // Add the ability to run into objects and detect if continuously running into the and plot a path around.
   /// <summary>
   /// For the mother NPC pass in true for the boolean and give two sprite names for the textures as an array in a_textureFileNames.
   /// The first is the sprite with the baby holding the candy, the second should have the candy missing.
   /// The sprite chosen to draw will be automatic based on if the mother currently has candy.
   /// You can set "hasCandy" to true manually but if the NPC has the boolean of "isMother" as true,
   /// the contructor will set "hasCandy" to true. The manual set is for when you steal the candy.
   /// 
   /// For a guard, just pass in a single sprite name and leave the second as null.
   /// </summary>
   public class NPC_Base_Class : Animated_Sprite
   {
      #region Member Variables

      private String[] m_SpriteVersionTextureNames; // Move to NPC_MOTHER for candy/non-candy switching
      private Texture2D[] m_SpriteVersions; // Move to NPC_MOTHER for candy/non-candy switching
      private bool m_IsMother; // Remove
      private bool m_HasCandy; // Move to NPC_MOTHER
      private bool m_FollowingPlayer; // Move to NPC_GUARD
      private bool m_Moveable; // Keep
      private int m_DetectionRadius; // Move to NPC_GUARD
      private int m_PathIndex; // Keep
      private int m_RespawnCandyTimer; // Move to NPC_MOTHER
      private Vector2 m_CurrentDestination; // Keep
      private Vector2 m_PreviousMovementSpeed; // Remove
      private Vector2[] m_PatrolPath; // Keep but rename


      #endregion

      #region Constructors

      public NPC_Base_Class (Game a_game, String[] a_textureFileNames, int[] a_SpriteAnimationSequence, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName)
         : base (a_game, a_textureFileNames, a_SpriteAnimationSequence, a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {
         if(m_IsMother)
         {
            m_HasCandy = true;
            m_RespawnCandyTimer = 0;
         }
         m_SpriteVersionTextureNames = a_textureFileNames;
         m_SpriteVersions = new Texture2D[2];
         m_PatrolPath = new Vector2[2];
         m_FollowingPlayer = false;
         m_Moveable = false;
         m_PreviousMovementSpeed = new Vector2 (5, 5);
         m_DetectionRadius = 200;
         m_PathIndex = 0;
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

      public bool followingPlayer
      {
         get
         {
            return m_FollowingPlayer;
         }
         set
         {
            m_FollowingPlayer = value;
         }
      }

      public bool moveable
      {
         get
         {
            return m_Moveable;
         }
         set
         {
            m_Moveable = value;
         }
      }

      public int detectionRadius
      {
         get
         {
            return m_DetectionRadius;
         }
         set
         {
            m_DetectionRadius = value;
         }
      }

      public int candyRespawnTimer
      {
         set
         {
            m_RespawnCandyTimer = value;
         }
      }

      public Vector2 currentDestination
      {
         get
         {
            return m_CurrentDestination;
         }
         set
         {
            m_CurrentDestination = value;
         }
      }

      public Vector2 movementSpeed
      {
         set
         {
            m_PreviousMovementSpeed = value;
         }
      }

      public Vector2[] patrolPath
      {
         set
         {
            m_PatrolPath = value;
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
         if (((GTC_Level1)this.Game).gameNotPaused)
         {
            #region Guard Follow Behavior
            if (!isMother && m_Moveable)
            {
               // If the player is within the detection radius of the guard

               bool withinDetectionRadius = (((GTC_Level1)this.Game).player.playerPosition - this.m_spritePosition).Length () < m_DetectionRadius;

               if (withinDetectionRadius && !m_FollowingPlayer && !((GTC_Level1)this.Game).player.isHidden)
               {
                  m_FollowingPlayer = true;
               }
               else if (!withinDetectionRadius && m_FollowingPlayer)
               {
                  m_FollowingPlayer = false;
               }

               if (((GTC_Level1)this.Game).player.isHidden)
               {
                  m_FollowingPlayer = false;
               }

               // If the guard is currently following the player, set the current destination to the players position.
               if (m_FollowingPlayer)
               {
                  m_CurrentDestination = ((Player_Controlled_Sprite)((GTC_Level1)this.Game).Components[0]).playerPosition;
               }
               else
               {
                  // Otherwise, test if the guard has reached its current destination.
                  if (m_spritePosition == m_PatrolPath[m_PathIndex] || isWithinDistanceOfDestination (3))
                  {
                     if ((m_PathIndex + 1) >= m_PatrolPath.Length)
                     {
                        m_PathIndex = 0;
                     }
                     else
                     {
                        m_PathIndex++;
                     }
                  }
                  // Set the new destination.
                  m_CurrentDestination = m_PatrolPath[m_PathIndex];
               }

               // Calculate the new movement vector based on the current destination.
               Vector2 movementDestination = m_CurrentDestination - this.spritePosition;
               movementDestination.Normalize ();

               this.m_spritePosition += (movementDestination * m_PreviousMovementSpeed);
               calculateDrawOrder ();
            }
            #endregion

            #region Mother Candy Respawn
            if (m_IsMother)
            {
               if (!m_HasCandy && m_RespawnCandyTimer > 0)
               {
                  m_RespawnCandyTimer--;
               }
               else if (!m_HasCandy && m_RespawnCandyTimer < 1)
               {
                  m_HasCandy = true;
               }
            }
            #endregion

            base.Update (gameTime);
         }
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

      protected bool isWithinDistanceOfDestination (int a_Disatance)
      {
         bool isWithinDistance;

         isWithinDistance = (this.m_CurrentDestination - this.spritePosition).Length () < a_Disatance;

         return isWithinDistance;
      }

      #endregion
   } // End NPC_Base_Class

   // TODO: Finish all mother functions.
   public class NPC_Mother_Class : NPC_Base_Class
   {
      #region Member Variables

      private bool m_HasCandy;
      private bool m_PreviousCandyState;
      private int m_CandyRespawnTimer;
      private int[] m_AlternateSpriteAnimationSequence;
      private String[] m_AlternateSpriteVersionTextureNames;
      private Texture2D[] m_AlternateSpriteVersions;

      #endregion

      #region Constructors

      public NPC_Mother_Class (Game a_game, String[] a_TextureFileNames, String[] a_AlternateTextureFileNames,
         int[] a_SpriteAnimationSequence, int[] a_AlternateSpriteAnimationSequence, Vector2 a_startingPosition,
         Color a_renderColor, bool a_collidable, String a_SpriteName, int a_CandyRespawnTimer)
         : base (a_game, a_TextureFileNames, a_SpriteAnimationSequence, a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {
         m_HasCandy = true;
         m_PreviousCandyState = true;
         m_CandyRespawnTimer = a_CandyRespawnTimer;
         m_AlternateSpriteAnimationSequence = a_AlternateSpriteAnimationSequence;
         m_AlternateSpriteVersionTextureNames = a_AlternateTextureFileNames;
      }

      #endregion

      #region Getters and Setters

      public bool hasCandy
      {
         get
         {
            return m_HasCandy;
         }
      }

      public int candyRespawnTimer
      {
         set
         {
            m_CandyRespawnTimer = value;
         }
      }

      #endregion

      #region Overridden Functions

      protected override void LoadContent ()
      {
         // Load the swappable drawing textures.
         for(int i = 0; i < m_AlternateSpriteVersionTextureNames.Length; i++)
         {
            m_AlternateSpriteVersions[i] = Game.Content.Load<Texture2D> (m_AlternateSpriteVersionTextureNames[i]);
         }

         base.LoadContent ();
      }

      public override void Initialize ()
      {
         base.Initialize ();
      }

      public override void Update (GameTime gameTime)
      {
         if (((GTC_Level1)this.Game).gameNotPaused)
         {
            // If the drawn sprites need to switch (there was a recent change)
            if (m_HasCandy != m_PreviousCandyState)
            {
               m_PreviousCandyState = m_HasCandy;

               // Swap the textures.
               Texture2D[] tempTextures = m_AnimatedSprites;
               m_AnimatedSprites = m_AlternateSpriteVersions;
               m_AlternateSpriteVersions = tempTextures;

               // Swap the animation sequences.
               int[] tempAnimations = m_SpriteAnimationSequences;
               m_SpriteAnimationSequences = m_AlternateSpriteAnimationSequence;
               m_AlternateSpriteAnimationSequence = tempAnimations;
            }
         }
      }

      #endregion

      #region Functions



      #endregion
   } // End NPC_Mother_Class

   // TODO: Start NPC_Guard_Class
   public class NPC_Guard_Class : NPC_Base_Class
   {
      #region Member Variables



      #endregion

      #region Constructors



      #endregion

      #region Getters and Setters



      #endregion

      #region Overridden Functions



      #endregion

      #region Functions



      #endregion
   } // End NPC_Guard_Class

   public class Splash_Screen : Sprite_Base_Class
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
            ((GTC_Level1)this.Game).gameBar.Visible = true;
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

   // TODO: Add a method to draw text
   public class Game_Bar : Sprite_Base_Class
   {
      #region Member Variables

      private SpriteFont m_DrawableFont;
      private Vector2 m_CandyCounterPosition;

      #endregion

      #region Constructors

      public Game_Bar (Game a_game, String a_textureFileName, Vector2 a_startingPosition, Color a_renderColor, String a_SpriteName)
         : base(a_game, a_textureFileName, a_startingPosition, a_renderColor, false, a_SpriteName)
      {
         m_CandyCounterPosition = new Vector2 (110, 2);
         this.DrawOrder = 1000;
      }

      #endregion

      #region Getters and Setters

      #endregion

      #region Overridden Functions

      protected override void LoadContent()
      {
         m_DrawableFont = this.Game.Content.Load<SpriteFont> ("SpriteFont1");

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

         sb.Begin ();

         sb.Draw (m_textureImage, m_spritePosition, null, m_spriteRenderColor, 0f, m_spriteCenter,
                  1.0f, SpriteEffects.None, 0f);
         if (this.Visible)
         {
            string s_CandyCountString = "Candy Count: " + ((GTC_Level1)this.Game).player.candyCount; 
            sb.DrawString (m_DrawableFont, s_CandyCountString, m_CandyCounterPosition, Color.Black);
         }

         sb.End ();
      }

      #endregion

      #region Functions

      #endregion
   }

   public class Cotton_Candy_Bomb : Sprite_Base_Class
   {
      #region Member Variables

      protected bool m_IsActive;
      protected bool m_AbleToActivate;
      protected bool m_UpdateThisFrame;
      protected int m_CooldownTimer;
      protected int m_ActiveTimer;
      protected int m_EffectRadius;
      protected int m_CurrentDrawSequence;
      protected Rectangle m_DrawRectangle;
      protected Texture2D m_AnimatedTexture;

      #endregion

      #region Constructors

      public Cotton_Candy_Bomb (Game a_game, String a_textureFileName, Vector2 a_startingPosition,
         int a_EffectRadius, String a_SpriteName):
         base(a_game, null, a_startingPosition, Color.White, false, a_SpriteName)
      {
         m_IsActive = false;
         m_AbleToActivate = true;
         m_UpdateThisFrame = true;
         m_CooldownTimer = 0;
         m_ActiveTimer = 0;
         m_CurrentDrawSequence = 0;
         m_EffectRadius = a_EffectRadius;
         m_textureFileName = a_textureFileName;
      }

      #endregion

      #region Getters and Setters

      public bool isActive
      {
         get
         {
            return m_IsActive;
         }
      }

      #endregion

      #region Overridden Functions

      protected override void LoadContent ()
      {
         m_AnimatedTexture = Game.Content.Load<Texture2D> (m_textureFileName);
         m_SpriteHeight = 500;
         m_SpriteWidth = 500;
         m_spriteCenter = new Vector2 (m_SpriteWidth/2, m_SpriteHeight/2);
         m_DrawRectangle = new Rectangle (0, 0, m_SpriteWidth, m_SpriteHeight);
         this.DrawOrder = 500;
      }

      public override void Initialize ()
      {
         base.Initialize ();
      }

      public override void Update (GameTime gameTime)
      {
         if (((GTC_Level1)this.Game).gameNotPaused)
         {
            base.Update (gameTime);

            #region Activate

            if (!m_IsActive && m_AbleToActivate &&(((GTC_Level1)this.Game).player.candyCount > 0))
            {
               KeyboardState keyboardInput = Keyboard.GetState ();

               if (keyboardInput.IsKeyDown (Keys.Space))
               {
                  this.Visible = true;
                  m_IsActive = true;
                  m_AbleToActivate = false;
                  m_CooldownTimer = (15 * 30); //Half a minute
                  m_ActiveTimer = 60;
                  m_spritePosition = ((GTC_Level1)this.Game).player.playerPosition;
                  ((GTC_Level1)this.Game).player.candyCount--;
               }
            }

            #endregion

            #region Update Timers

            if (m_CooldownTimer > 0)
            {
               m_CooldownTimer--;

               if (m_CooldownTimer < 1)
               {
                  m_AbleToActivate = true;
               }
            }

            if (m_ActiveTimer > 0)
            {
               m_ActiveTimer--;

               // If the bomb is no longer active.
               if (m_ActiveTimer < 1)
               {
                  m_IsActive = false;
                  this.Visible = false;
               }
            }

            #endregion
         }
      }

      public override void Draw (GameTime gameTime)
      {

         if (this.Visible)
         {
            SpriteBatch sb = ((GTC_Level1)this.Game).spriteBatch;

            sb.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, ((GTC_Level1)this.Game).cameraPosition);
            sb.Draw (m_AnimatedTexture, m_spritePosition, m_DrawRectangle, m_spriteRenderColor, 0f, m_spriteCenter,
               1.0f, SpriteEffects.None, 0f);
            sb.End ();

            if (m_UpdateThisFrame)
            {
               m_CurrentDrawSequence++;
               if (m_CurrentDrawSequence < 6)
               {
                  m_DrawRectangle.Offset (m_SpriteWidth, 0);
               }
               else
               {
                  m_CurrentDrawSequence = 0;
                  m_DrawRectangle.X = 0;
               }
            }

            m_UpdateThisFrame = !m_UpdateThisFrame;
         }

         base.Draw (gameTime);
      }

      #endregion

      #region Functions

      public bool isWithinDetectionRadius (Sprite_Base_Class a_sprite)
      {
         bool isWithinDistance;

         isWithinDistance = (this.spritePosition - a_sprite.spritePosition).Length () < m_EffectRadius;

         return isWithinDistance;
      }

      #endregion
   }
}