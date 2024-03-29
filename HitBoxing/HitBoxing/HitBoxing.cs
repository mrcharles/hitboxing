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

namespace HitBoxing
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HitBoxing : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D blockTex;
        SamplerState wrapUV;
        Camera cam;
        InputHelper input;
        InputHelper.PadHelper pad;
        int BaseUnitSize = 64;
        int LevelWidth = 50;
        int LevelHeight = 15;
        int GroundOffset = 100;

		int mode = 0;

		SliderGroup sliders;
		SliderGroup.DoubleSlider initscaleslider;
		SliderGroup.DoubleSlider magicslider;
		SliderGroup.DoubleSlider scalemodslider;
		SliderGroup.DoubleSlider scalemodmodslider;

        public HitBoxing()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            blockTex = Content.Load<Texture2D>("block");
            input = new InputHelper();
            
			sliders = new SliderGroup(blockTex, new Vector2(20,20));
			initscaleslider = new SliderGroup.DoubleSlider(0.004, 0.004, 1.6, 0.004, SliderChanged);
			sliders.RegisterValue(initscaleslider);
			magicslider = new SliderGroup.DoubleSlider(0.1, -1.0, 1.0, 0.8, SliderChanged);
			sliders.RegisterValue(magicslider);
			scalemodslider = new SliderGroup.DoubleSlider(0.1, 0.1, 1.9, 1.0, SliderChanged);
			sliders.RegisterValue(scalemodslider);
			scalemodmodslider = new SliderGroup.DoubleSlider(0.1, 0.1, 1.9, 0.3, SliderChanged);
			sliders.RegisterValue(scalemodmodslider);

			sliders.SelectSlider(0);


            try
            {
                pad = input.AcquireNewPad();
                cam = new CamManual(pad);
            }
            catch(Exception e)
            {
                cam = new CamKeyboard(input);
            }

            //cam = new CamManual(pad);
            //cam = new CamKeyboard(input);
            cam.Viewport = new Vector2(1280, 720);
            cam.Position = new Vector2(0, 0);//-360 + GroundOffset);
            int width = LevelWidth * BaseUnitSize;
            int height = LevelHeight * BaseUnitSize;
            cam.Bounds = new Rectangle(-width / 2, -height, width, height + GroundOffset);
            wrapUV = new SamplerState();
            wrapUV.AddressU = TextureAddressMode.Wrap;
            wrapUV.AddressV = TextureAddressMode.Wrap;
            wrapUV.Filter = TextureFilter.Point;

		    base.Initialize();
        }

		
		void SliderChanged(object slider)
		{
		}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            input.Update();
            // Allows the game to exit
            if (input.JustPressed(Keys.Escape))
            //if( pad.JustReleased(Buttons.A) )
                this.Exit();

			if (input.JustPressed(Keys.Q))
			{
				if (mode == 0)
					mode = 1;
				else mode = 0;
			}

			if (mode == 0)
			{
				UpdateCam(gameTime);

			}
			else 
			{
				UpdateSliders(gameTime);
			}
				
            base.Update(gameTime);
        }
		void UpdateCam(GameTime gameTime)
		{
			// TODO: Add your update logic here
			cam.Update();
		
		}

		void UpdateSliders(GameTime gameTime)
		{
			if (input.JustPressed(Keys.Down))
			{
				sliders.SelectNext();
			}
			if (input.JustPressed(Keys.Up))
			{
				sliders.SelectPrev();
			}
			if (input.JustPressed(Keys.Right))
			{
				sliders.AdjustRight();
			}
			if (input.JustPressed(Keys.Left))
			{
				sliders.AdjustLeft();
			}
		}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Opaque, wrapUV, null, null, null, cam.Transform());

            Rectangle src = new Rectangle(0, 0, LevelWidth * BaseUnitSize, LevelHeight * BaseUnitSize);
            spriteBatch.Draw(blockTex, new Vector2(-LevelWidth * BaseUnitSize / 2, -LevelHeight * BaseUnitSize), src, Color.White);

			

            //spriteBatch.Draw(blockTex, new Rectangle(200, 200, 400, 400), blockTex.Bounds, Color.Gainsboro);
            spriteBatch.End();
            // TODO: Add your drawing code here

			sliders.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
