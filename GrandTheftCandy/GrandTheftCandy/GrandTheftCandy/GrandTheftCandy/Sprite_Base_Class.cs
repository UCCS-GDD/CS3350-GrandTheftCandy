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

      protected void calculateDrawOrder ()
      {
         this.DrawOrder = (((int)m_spritePosition.Y - 200) / 5) + 5;
      }

      #endregion

   } // End Sprite_Base_Class.

   // TODO: Hook Player Class into Animated Sprite class and ensure that animation works.
   /// <summary>
   /// When providing the file names for the animated textures,
   /// provide them in the same order as the enum spriteAnimationSequence.
   /// </summary>
   class Animated_Sprite : Sprite_Base_Class
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
      private int[] m_SpriteAnimationSequences;

      #endregion

      #region Constructors

      public Animated_Sprite(Game a_game, String[] a_textureFileNames, int[] a_SpriteAnimationSequence, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName)
         : base (a_game, null, a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {
         m_AnimatedTextureNames = a_textureFileNames;
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

         base.Update (gameTime);
      }

      public override void Draw (GameTime gameTime)
      {
         if (m_DrawThisFrame)
         {
            SpriteBatch sb = ((GTC_Level1)this.Game).spriteBatch;

            sb.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, ((GTC_Level1)this.Game).cameraPosition);
            sb.Draw (m_AnimatedSprites[(int)m_CurrentAnimation], m_spritePosition, m_CurrentDrawRectangle, m_spriteRenderColor, 0f, m_spriteCenter,
               1.0f, SpriteEffects.None, 0f);
            sb.End ();

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
                  m_CurrentDrawRectangle.Location = new Point (0, 0);
               }
            }
         }

         m_DrawThisFrame = !m_DrawThisFrame;

         //base.Draw (gameTime);
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
         if (m_CurrentMovement.X >= m_CurrentMovement.Y)
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
      }

      #endregion
   } // End Animated_Sprite Class.

   public class Player_Controlled_Sprite : Sprite_Base_Class
   {
      #region Member Variables

      private bool m_MovementAllowed;
      private int m_CandyCount;

      #endregion

      #region Constructors

      public Player_Controlled_Sprite (Game a_game, String a_textureFileName, Vector2 a_startingPosition, 
         Color a_renderColor, bool a_collidable, String a_SpriteName)
         : base(a_game, a_textureFileName, a_startingPosition, a_renderColor, a_collidable, a_SpriteName)
      {
         m_MovementAllowed = false;
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
                     if (this.collidesHalfHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
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
                     if (this.collidesHalfHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
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
               if (m_spritePosition.X < 3000)
               {
                  m_spritePosition.X += 5;
                  Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
                  this.Game.Components.CopyTo (spriteList, 0);
                  for (int i = 0; i < spriteList.Length; i++)
                  {
                     if (this.collidesHalfHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
                     {
                        m_spritePosition.X -= 5;
                     }

                     if (m_spritePosition.X < 2600 && m_spritePosition.X > 400)
                     {
                        ((GTC_Level1)this.Game).cameraPosition = Matrix.CreateTranslation (400 - m_spritePosition.X, 0, 0);
                     }
                     else if (m_spritePosition.X > 2600)
                     {
                        ((GTC_Level1)this.Game).cameraPosition = Matrix.CreateTranslation (-2200, 0, 0);
                     }
                  }
               }
            }
            #endregion

            #region Move Up
            if ((keyboardInput.IsKeyDown (Keys.W) || keyboardInput.IsKeyDown (Keys.Up)) && this.spritePosition.Y > 190)
            {
               m_spritePosition.Y -= 5;
               this.DrawOrder--;
               Sprite_Base_Class[] spriteList = new Sprite_Base_Class[this.Game.Components.Count];
               this.Game.Components.CopyTo (spriteList, 0);
               for (int i = 0; i < spriteList.Length; i++)
               {
                  if (this.collidesHalfHorizontally (spriteList[i]) && this.spriteName != spriteList[i].spriteName)
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
   public class NPC_Base_Class : Sprite_Base_Class
   {
      #region Member Variables

      private String[] m_SpriteVersionTextureNames;
      private Texture2D[] m_SpriteVersions;
      private bool m_IsMother;
      private bool m_HasCandy;
      private bool m_FollowingPlayer;
      private bool m_Moveable;
      private int m_DetectionRadius;
      private int m_PathIndex;
      private int m_RespawnCandyTimer;
      private Vector2 m_CurrentDestination;
      private Vector2 m_MovementSpeed;
      private Vector2[] m_PatrolPath;

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
            m_RespawnCandyTimer = 0;
         }
         m_SpriteVersionTextureNames = a_textureFileNames;
         m_SpriteVersions = new Texture2D[2];
         m_PatrolPath = new Vector2[2];
         m_FollowingPlayer = false;
         m_Moveable = false;
         m_MovementSpeed = new Vector2 (5, 5);
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
            m_MovementSpeed = value;
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
         #region Guard Follow Behavior
         if (!isMother && m_Moveable)
         {
            // If the player is within the detection radius of the guard

            bool withinDetectionRadius = (((GTC_Level1)this.Game).player.playerPosition - this.m_spritePosition).Length () < m_DetectionRadius;

            if (withinDetectionRadius && !m_FollowingPlayer)
            {
               m_FollowingPlayer = true;
            }
            else if (!withinDetectionRadius && m_FollowingPlayer)
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
               if (m_spritePosition == m_PatrolPath[m_PathIndex] || isWithinDistanceOfDestination(3))
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

            this.m_spritePosition += (movementDestination * m_MovementSpeed);
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
         m_CandyCounterPosition = new Vector2 (5, 10);
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
}