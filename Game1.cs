//-----------------------------------------------------------------------------
// Main Game Class. This basically just creates some Singletons and then
// hands off all other logic to them.
//
// __Defense Sample for Game Programming Algorithms and Techniques
// Copyright (C) Sanjay Madhav. All rights reserved.
//
// Released under the Microsoft Permissive License.
// See LICENSE.txt for full details.
//-----------------------------------------------------------------------------
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
using itp380.Graphics;

namespace itp380
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
        

		public Game1()
		{
			IsFixedTimeStep = false;
            PhysicsGraphicsManager.Get().Start(this, GraphicsManager.Get().Start(this));
            //GraphicsManager.Get().Start(this);
            
			if (DebugDefines.bShowWindowsMouseCursor)
			{
				IsMouseVisible = true;
			}
			else
			{
				IsMouseVisible = false;
			}
			Window.Title = "Flightsim";
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
            
			GameState.Get().Start(this);
			GameState.Get().SetState(eGameState.MainMenu);
			InputManager.Get().Start();
			PhysicsManagerITP.Get().Start(this);


            base.Initialize();


        }

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			GraphicsManager.Get().LoadContent();
            PhysicsGraphicsManager.Get().LoadContent();
			SoundManager.Get().LoadContent(Content);

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
			float fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (fDeltaTime > 0.1f)
			{
				fDeltaTime = 0.1f;
			}
           
			// If the game doesn't have focus don't update anything
			if (IsActive)
			{
                base.Update(gameTime);
				InputManager.Get().Update(fDeltaTime);
				GameState.Get().Update(fDeltaTime);
			}

			
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
            GraphicsManager.Get().Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (World.Get().Enabled) {
                World.Get().Draw(gameTime);
            }
            PhysicsGraphicsManager.Get().Draw((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (GameState.Get().ParticlesManager != null && GameState.Get().ParticlesManager.Enabled)
                GameState.Get().ParticlesManager.Draw();
			base.Draw(gameTime);
		}
	}
}
