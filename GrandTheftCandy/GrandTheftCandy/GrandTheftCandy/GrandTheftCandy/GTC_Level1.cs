/*
 * Grand Theft Candy
 * Level 1 - Project 1
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

      Player_Controlled_Sprite player;
      NPC_Base_Class mother1;
      NPC_Base_Class guard1;
      Sprite_Base_Class candyEntrance;
      Sprite_Base_Class winScreen;
      Sprite_Base_Class gameOver;
      Sprite_Base_Class mallFloor;
      Sprite_Base_Class mallWall;
      Sprite_Base_Class gameBar;

      Vector2 screenCenter;
      Vector2 candyStoreEntrance;

      Texture2D lineTexture;
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
         lineTexture = this.Content.Load<Texture2D>(@"Resources\Images\Line");

         screenCenter = new Vector2 ( ( graphics.GraphicsDevice.Viewport.Width / 2 ), ( graphics.GraphicsDevice.Viewport.Height / 2 ) );
         candyStoreEntrance = new Vector2 ( 2620, 200);
         string[] MotherSprites = new string[2] { @"Resources\Images\stroller1", @"Resources\Images\stroller2" };
         string[] GuardSprite = new string[2] { @"Resources\Images\guardsprite", null };

         #region Sprite Creation
         player = new Player_Controlled_Sprite (this, @"Resources\Images\mainSpriteLeftStill", screenCenter, Color.White, true, "Player");
         mother1 = new NPC_Base_Class (this, MotherSprites, new Vector2 (50, 400), Color.White, true, "Mother1", true);
         guard1 = new NPC_Base_Class (this, GuardSprite, new Vector2 (1000, 400), Color.White, true, "Guard1", false);

         candyEntrance = new Sprite_Base_Class(this, @"Resources\Images\redsquare", candyStoreEntrance, Color.White, true, "Candy Entrance");
         mallFloor = new Sprite_Base_Class (this, @"Resources\Images\floorbg", new Vector2(1500, 300), false, 0, "Mall Floor");
         mallWall = new Sprite_Base_Class ( this, @"Resources\Images\mallbg", new Vector2(1500, 100), true, 1, "Mall Wall");
         gameBar = new Sprite_Base_Class (this, @"Resources\Images\gamebar", new Vector2 (400, 300), false, 100, "Game bar");
         gameBar.Visible = false;
         candyEntrance.Visible = false;

         Splash_Screen splashScreen = new Splash_Screen(this, @"Resources\Images\SplashScreen", screenCenter, Color.White, "Splash Screen");

         gameOver = new Sprite_Base_Class(this, @"Resources\Images\gameover0", screenCenter, false, 500, "Game Over 1");
         gameOver.Visible = false;

         winScreen = new Sprite_Base_Class(this, @"Resources\Images\winner", screenCenter, false, 1000, "Game Over 2");
         winScreen.Visible = false;
         #endregion

         #region Set Guard behavior
         Vector2[] guard1Path = new Vector2[2];
         guard1Path[0] = new Vector2 (500, 400);
         guard1Path[1] = new Vector2 (1500, 400);

         guard1.moveable = true;
         guard1.patrolPath = guard1Path;
         guard1.movementSpeed = new Vector2 (2, 2);
         guard1.detectionRadius = 100;
         #endregion

         //Song backgroundSound = Content.Load<Song>(@"Resources\Sounds\gameMusic");
         //MediaPlayer.Play(backgroundSound);
         //MediaPlayer.Volume = 50;

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
         // Allows the game to exit.
         KeyboardState keyboardState = Keyboard.GetState ();

         if (keyboardState.IsKeyDown (Keys.Escape) ||
            GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
         {
            this.Exit ();
         }

         if (player.isWithinSpriteBoundry(candyEntrance))
         {
            winScreen.Visible = true;
            player.movementAllowed = false;
            cameraPosition = Matrix.CreateTranslation (Vector3.Zero);
         }

         if (player.collidesWithAbove (mother1) || player.collidesWithBelow (mother1))
         {
            mother1.hasCandy = false;
         }

         if (player.collides(guard1) || player.collidesHorizontally (guard1))
         {
            cameraPosition = Matrix.CreateTranslation (Vector3.Zero);
            player.movementAllowed = false;
            gameOver.Visible = true;
         }

         base.Update(gameTime);
      }

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.CornflowerBlue);

         //// Draw a vertical line through the center.
         //Vector2 verticalStart = new Vector2(400,0);
         //Vector2 verticalEnd = new Vector2(400, 600);
         //DrawLine(lineTexture, this.spriteBatch, verticalStart, verticalEnd, true );

         //// Draw a horizontal line through the center.
         //Vector2 horizontalStart = new Vector2 ( 0, 300 );
         //Vector2 horizontalEnd = new Vector2 ( 800, 300 );
         //DrawLine ( lineTexture, this.spriteBatch, horizontalStart, horizontalEnd, false );

         base.Draw(gameTime);
      }

      #endregion

      #region Functions

      /// <summary>
      /// Draws either a vertical line or horizontal line using a starting point and an ending point.
      /// </summary>
      /// <param name="a_lineTexture"></param>
      /// <param name="spriteBatch"></param>
      /// <param name="a_startingPosition"></param>
      /// <param name="a_endingPosition"></param>
      /// <param name="a_drawVertical"></param>
      public static void DrawLine(Texture2D a_lineTexture, SpriteBatch spriteBatch, 
         Vector2 a_startingPosition, Vector2 a_endingPosition, bool a_drawVertical)
      {
         if ( a_drawVertical )
         {
            spriteBatch.Begin();
            while ( a_startingPosition.Y < a_endingPosition.Y )
            {
               spriteBatch.Draw(a_lineTexture, a_startingPosition, Color.Black);
               a_startingPosition.Y++;
            }
            spriteBatch.End();
         }
         else
         {
            spriteBatch.Begin ();
            while ( a_startingPosition.X < a_endingPosition.X )
            {
               spriteBatch.Draw ( a_lineTexture, a_startingPosition, Color.Black );
               a_startingPosition.X++;
            }
            spriteBatch.End ();
         }
      }

      #endregion

      public Vector2 ladyStart { get; set; }
   }
}
