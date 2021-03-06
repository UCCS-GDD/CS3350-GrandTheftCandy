/*
 * Grand Theft Candy
 * Level 1 - Prototype 1
 * Simple XNA Game
 * Matthew Rawlins
 * Jeffrey St.Germain  
 * Jason Noujok
 * Danyelle Barnett
 * Lauren Benson
*/

/*
 *---------------------Notes--------------------
 * All objects using the Sprite_Base_Class use the first constructor.
 * The second is for the "mall" floors and background objects.
 * Draw Order for background objects should be within the range of 0-4.
 * Normal objects are automatically calculated based on the starting position
 * and changes as the sprite moves up and down. (Range: 5-85)
 * Please use 5 pixel incriments when placing sprites, with a minimum Y value of 200.
 * Note that the starting position for a sprite is the center of the sprite.
 * The top 200 is reserved for the background wall.
 */

// TODO: Download and start using all of the new NPC Class functions.
// Ensure that all the functionality is the same.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GrandTheftCandy
{
   /// <summary>
   /// This is the main type for your game.
   /// </summary>
   public class GTC_Level1 : Microsoft.Xna.Framework.Game
   {

      #region Member Variables
      GraphicsDeviceManager graphics;
      public SpriteBatch spriteBatch;
      public Matrix cameraPosition;
      public bool gameNotPaused;
      private bool ableToPause;
      private int pauseTimer;

      public Player_Controlled_Sprite player;
      NPC_Base_Class[] mothers;
      NPC_Base_Class[] guards;
      Sprite_Base_Class[] folliage;
      Sprite_Base_Class candyEntrance;
      Sprite_Base_Class winScreen;
      Sprite_Base_Class gameOver;
      Sprite_Base_Class mallFloor;
      Sprite_Base_Class mallWall;
      Cotton_Candy_Bomb cottonCandyBomb;
      public Game_Bar gameBar;

      Vector2 screenCenter;
      Vector2 candyStoreEntrance;

      #endregion

      #region Constructors

      public GTC_Level1()
      {
         graphics = new GraphicsDeviceManager(this);
         graphics.PreferredBackBufferWidth = 800;
         graphics.PreferredBackBufferHeight = 600;
         this.IsMouseVisible = true;
         Content.RootDirectory = "Content";
         cameraPosition = Matrix.CreateTranslation(new Vector3(0, 0, 1));
         mothers = new NPC_Base_Class[5];
         guards = new NPC_Base_Class[2];
         folliage = new Sprite_Base_Class[8];
         gameNotPaused = true;
         ableToPause = true;
         pauseTimer = 0;
      }

      #endregion

      #region Overridden Functions
      /// <summary>
      /// Allows the game to perform any initialization it needs to before starting to run.
      /// This is where it can query for any required services and load any non-graphic
      /// related content.  Calling base.Initialize will enumerate through any components
      /// and initialize them as well.
      /// </summary>
      protected override void Initialize()
      {
         screenCenter = new Vector2 ( ( graphics.GraphicsDevice.Viewport.Width / 2 ), ( graphics.GraphicsDevice.Viewport.Height / 2 ) );
         candyStoreEntrance = new Vector2 ( 2640, 200);

         #region Sprite Creation

         initializePlayer ();
         initializeMothers ();
         initializeGuards ();
         initializeMall ();
         initializeFolliage ();

         gameBar = new Game_Bar (this, @"Resources\Images\gamebar", new Vector2 (400, 300), Color.White, "Game bar");
         candyEntrance.Visible = false;
         gameBar.Visible = false;

         Splash_Screen splashScreen = new Splash_Screen(this, @"Resources\Images\SplashScreen", screenCenter, Color.White, "Splash Screen");

         gameOver = new Sprite_Base_Class(this, @"Resources\Images\gameover0", screenCenter, false, 500, "Game Over 1");
         gameOver.Visible = false;

         winScreen = new Sprite_Base_Class(this, @"Resources\Images\winner", screenCenter, false, 1000, "Game Over 2");
         winScreen.Visible = false;

         cottonCandyBomb = new Cotton_Candy_Bomb (this, @"Resources\Images\cottoncandy", screenCenter, 250, "Cotton Candy");
         cottonCandyBomb.Visible = false;

         #endregion

         #region Set Guard behavior

         // Create a path of two waypoints for the guard to follow.
         Vector2[] guard1Path = new Vector2[4];
         guard1Path[0] = new Vector2 (300, 300);
         guard1Path[1] = new Vector2 (300, 550);
         guard1Path[2] = new Vector2 (750, 600);
         guard1Path[3] = new Vector2 (1500, 400);

         Vector2[] guard2Path = new Vector2[4];
         guard2Path[0] = new Vector2 (1500, 300);
         guard2Path[1] = new Vector2 (2500, 300);
         guard2Path[2] = new Vector2 (2500, 800);
         guard2Path[3] = new Vector2 (1500, 800);

         // Enable the guard to move, set the path, speed, and detection radius.
         guards[0].moveable = guards[1].moveable = true;
         guards[0].movementSpeed = guards[1].movementSpeed =new Vector2 (4, 4);
         guards[0].detectionRadius = guards[1].detectionRadius = 0;
         guards[0].followPath = guard1Path;
         guards[1].followPath = guard2Path;

         #endregion

         Song backgroundSound = Content.Load<Song> (@"Resources\Sounds\gameMusic");
         MediaPlayer.Play (backgroundSound);
         MediaPlayer.Volume = 50;

         base.Initialize();
        }

      /// <summary>
      /// LoadContent will be called once per game and is the place to load
      /// all of your content.
      /// </summary>
      protected override void LoadContent()
      {
         // Create a new SpriteBatch, which can be used to draw textures.
         spriteBatch = new SpriteBatch(GraphicsDevice);
      }

      /// <summary>
      /// UnloadContent will be called once per game and is the place to unload
      /// all content.
      /// </summary>
      protected override void UnloadContent()
      {
      }

      /// <summary>
      /// Allows the game to run logic such as updating the world,
      /// checking for collisions, gathering input, and playing audio.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Update(GameTime gameTime)
      {
         KeyboardState keyboardState = Keyboard.GetState ();

         #region Game Exit

         if (keyboardState.IsKeyDown (Keys.Escape) ||
            GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
         {
            this.Exit ();
         }

         #endregion

         #region Win Condition

         if (player.isWithinSpriteBoundry(candyEntrance) && player.candyCount>4)
         {
            winScreen.Visible = true;
            player.movementAllowed = false;
            cameraPosition = Matrix.CreateTranslation (Vector3.Zero);
         }

         #endregion

         #region Candy Stealing

         for (int i = 0; i < mothers.Length; i++)
         {
            if ((player.collidesWithAbove (mothers[i]) || player.collidesWithBelow (mothers[i])) && mothers[i].hasCandy)
            {
               mothers[i].hasCandy = false;
               mothers[i].candyRespawnTimer = 150;
               player.candyCount++;
               for (int c = 0; c < guards.Length; c++)
               {
                  guards[c].detectionRadius = 50 * player.candyCount;
               }
            }
         }

         #endregion

         #region Getting Caught (Losing)

         for (int i = 0; i < guards.Length; i++)
         {
            if (((player.collides (guards[i]) || 
               player.collidesHorizontally (guards[i])) && guards[i].followingPlayer) &&
               !player.isHidden)
            {
               cameraPosition = Matrix.CreateTranslation (Vector3.Zero);
               player.movementAllowed = false;
               gameOver.Visible = true;
            }
         }

         #endregion

         #region Cotton Candy Radius Detection
         if (cottonCandyBomb.isActive)
         {
            for (int i = 0; i < guards.Length; i++)
            {
               if (cottonCandyBomb.isWithinDetectionRadius( guards[i]))
               {
                  guards[i].detectionRadius = (guards[i].detectionRadius/2);
               }
            }
         }

         #endregion

         #region Hiding

         bool isPlayerHidden = false;
         for (int i = 0; i < folliage.Length; i++)
         {

            if (player.collidesWithAbove (folliage[i]) || player.collidesWithBelow (folliage[i]))
            {
               isPlayerHidden = true;
            }

            if (isPlayerHidden)
            {
               player.spriteColor = Color.Gray;
            }
            else
            {
               player.spriteColor = Color.White;
            }

            player.isHidden = isPlayerHidden;
         }

         #endregion

         #region Game Pause

         if (keyboardState.IsKeyDown (Keys.P))
         {
            if (ableToPause)
            {
               gameNotPaused = !(gameNotPaused);
               ableToPause = false;
               pauseTimer = 15;
            }
         }

      if (!ableToPause && pauseTimer > 0)
      {
         pauseTimer--;
      }

      if (pauseTimer < 1 && !ableToPause)
      {
         ableToPause = true;
      }

      #endregion

         base.Update(gameTime);
      }

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.CornflowerBlue);

         base.Draw(gameTime);
      }

      #endregion

      #region Functions

      public void initializePlayer ()
      {
         string[] playerSprites = new string[8]{@"Resources\Images\mainSpriteDownStill", @"Resources\Images\mainSpriteDown",
            @"Resources\Images\mainSpriteLeftStill", @"Resources\Images\mainSpriteLeft",
            @"Resources\Images\mainSpriteRightStill", @"Resources\Images\mainSpriteRight", 
            @"Resources\Images\mainSpriteUpStill", @"Resources\Images\mainSpriteUp"};
         int[] playerSpriteSequences = new int[8] { 1, 7, 1, 7, 1, 7, 1, 7 };
         player = new Player_Controlled_Sprite (this, playerSprites, playerSpriteSequences, screenCenter, Color.White, true, "Player");
      }

      public void initializeMothers ()
      {
         string[] MotherSprites = new string[2] { @"Resources\Images\stroller1", @"Resources\Images\stroller2" };
         mothers[0] = new NPC_Base_Class (this, MotherSprites, new Vector2 (250, 600), Color.White, true, "Mother0", true);
         mothers[1] = new NPC_Base_Class (this, MotherSprites, new Vector2 (1500, 800), Color.White, true, "Mother1", true);
         mothers[2] = new NPC_Base_Class (this, MotherSprites, new Vector2 (2250, 400), Color.White, true, "Mother2", true);
         mothers[3] = new NPC_Base_Class (this, MotherSprites, new Vector2 (2500, 750), Color.White, true, "Mother3", true);
         mothers[4] = new NPC_Base_Class (this, MotherSprites, new Vector2 (1000, 300), Color.White, true, "Mother4", true);
      }

      public void initializeGuards ()
      {
         string[] GuardSprite = new string[2] { @"Resources\Images\guardsprite", null };
         guards[0] = new NPC_Base_Class (this, GuardSprite, new Vector2 (1000, 400), Color.White, true, "Guard0", false);
         guards[1] = new NPC_Base_Class (this, GuardSprite, new Vector2 (2000, 500), Color.White, true, "Guard1", false);
      }

      public void initializeMall ()
      {
         candyEntrance = new Sprite_Base_Class (this, @"Resources\Images\redsquare", candyStoreEntrance, Color.White, true, "Candy Entrance");
         mallFloor = new Sprite_Base_Class (this, @"Resources\Images\floorbg", new Vector2 (1500, 400), false, 0, "Mall Floor");
         mallWall = new Sprite_Base_Class (this, @"Resources\Images\mallbg", new Vector2 (1500, 100), true, 1, "Mall Wall");
      }

      public void initializeFolliage ()
      {
         folliage[0] = new Sprite_Base_Class (this, @"Resources\Images\hidingBush", new Vector2 (500, 450), Color.White, true, "Bush1");
         folliage[1] = new Sprite_Base_Class (this, @"Resources\Images\hidingBush", new Vector2 (2600, 500), Color.White, true, "Bush2");
         folliage[2] = new Sprite_Base_Class (this, @"Resources\Images\hidingTrash", new Vector2 (750, 200), Color.White, true, "Trash Can1");
         folliage[3] = new Sprite_Base_Class (this, @"Resources\Images\hidingtrash", new Vector2 (1800, 200), Color.White, true, "Trash Can2");
         folliage[4] = new Sprite_Base_Class (this, @"Resources\Images\hidingTree", new Vector2 (1600, 400), Color.White, true, "Tree1");
         folliage[5] = new Sprite_Base_Class (this, @"Resources\Images\hidingBush", new Vector2 (1500, 650), Color.White, true, "Bush3");
         folliage[6] = new Sprite_Base_Class (this, @"Resources\Images\hidingTree", new Vector2 (1000, 800), Color.White, true, "Tree2");
         folliage[7] = new Sprite_Base_Class (this, @"Resources\Images\hidingTree", new Vector2 (2250, 700), Color.White, true, "Tree3");
      }

      #endregion
   }
}
