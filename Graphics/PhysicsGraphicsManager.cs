//-----------------------------------------------------------------------------
// The GraphicsManager handles all lower-level aspects of rendering.
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using itp380.Objects;
using itp380.Graphics;

namespace itp380
{


    public class PhysicsGraphicsManager : itp380.Patterns.Singleton<PhysicsGraphicsManager>
    {

        HUD hud = HUD.Get();

        GraphicsDeviceManager m_Graphics;
        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                return this.m_Graphics;
            }
        }


        Game m_Game;
        SpriteBatch m_SpriteBatch;
        Texture2D m_Blank;

        TutorialMessages tutorialMsgs = new TutorialMessages();
        public TutorialMessages TutorialMessages { get { return this.tutorialMsgs; } }
        SpriteFont m_FPSFont;
        SpriteFont m_GameFont;
        SpriteFont m_KeyFont;
        SpriteFont m_TextFont;

        class PhysicsGameObjectToDraw
        {
            public int Order { get; set; }
            public PhysicsGameObject GObject { get; set; }
            public PhysicsGameObjectToDraw(PhysicsGameObject obj, int order)
            {
                this.GObject = obj; this.Order = order;
            }
        }
        //LinkedList<PhysicsGameObjectToDraw> m_Objects = new LinkedList<PhysicsGameObjectToDraw>();
        LinkedList<PhysicsGameObject> m_Objects = new LinkedList<PhysicsGameObject>();

        public Matrix Projection;

        public bool IsFullScreen
        {
            get { return m_Graphics.IsFullScreen; }
            set { m_Graphics.IsFullScreen = value; }
        }

        public bool IsVSync
        {
            get { return m_Graphics.SynchronizeWithVerticalRetrace; }
            set { m_Graphics.SynchronizeWithVerticalRetrace = value; }
        }

        public int Width
        {
            get { return m_Graphics.PreferredBackBufferWidth; }
        }

        public int Height
        {
            get { return m_Graphics.PreferredBackBufferHeight; }
        }

        public Texture2D BlankTexture
        {
            get { return this.m_Blank; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return m_Graphics.GraphicsDevice; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return m_SpriteBatch; }
        }

        float m_fZoom = GlobalDefines.fCameraZoom;


        public void Start(Game game, GraphicsDeviceManager gdm)
        {
            m_Graphics = gdm;
            m_Game = game;
            IsVSync = GlobalDefines.bVSync;

            // TODO: Set resolution to what's saved in the INI, or default full screen
            if (!GlobalDefines.bFullScreen)
            {
                SetResolution(GlobalDefines.WindowedWidth, GlobalDefines.WindowHeight);
            }
            else
            {
                SetResolutionToCurrent();
                ToggleFullScreen();
            }
        }

        public GraphicsDeviceManager Start(Game game)
        {
            m_Graphics = new GraphicsDeviceManager(game);
            m_Game = game;
            IsVSync = GlobalDefines.bVSync;

            // TODO: Set resolution to what's saved in the INI, or default full screen
            if (!GlobalDefines.bFullScreen)
            {
                SetResolution(GlobalDefines.WindowedWidth, GlobalDefines.WindowHeight);
            }
            else
            {
                SetResolutionToCurrent();
                ToggleFullScreen();
            }
            return m_Graphics;
        }

        public void LoadContent()
        {
            //InitializeRenderTargets();

            m_SpriteBatch = new SpriteBatch(m_Graphics.GraphicsDevice);

            // Load FPS font
            m_FPSFont = m_Game.Content.Load<SpriteFont>("Fonts/FixedText");
            m_GameFont = m_Game.Content.Load<SpriteFont>("Fonts/MotorwerkOblique");
            m_KeyFont = m_Game.Content.Load<SpriteFont>("Fonts/Keycaps");
            m_TextFont = m_Game.Content.Load<SpriteFont>("Fonts/OCR");

            // Debug stuff for line drawing
            m_Blank = new Texture2D(m_Graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            m_Blank.SetData(new[] { Color.White });
        }

        public void SetResolutionToCurrent()
        {
            m_Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            m_Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            m_fZoom = GlobalDefines.fCameraZoom;
            SetProjection((float)Width / Height);

            if (m_Graphics.GraphicsDevice != null)
            {
                m_Graphics.ApplyChanges();
            }
        }

        public void SetResolution(int Width, int Height)
        {
            m_Graphics.PreferredBackBufferWidth = Width;
            m_Graphics.PreferredBackBufferHeight = Height;

            m_fZoom = GlobalDefines.fCameraZoom;
            SetProjection((float)Width / Height);

            if (m_Graphics.GraphicsDevice != null)
            {
                m_Graphics.ApplyChanges();
            }
        }

        public void SetProjection(float fAspectRatio)
        {
            //Projection = Matrix.CreateOrthographic(m_fZoom, m_fZoom / fAspectRatio, 0.1f, 100.0f);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(85.0f), fAspectRatio, 0.01f, 100000.0f);
        }

        public void ResetProjection()
        {
            m_fZoom = GlobalDefines.fCameraZoom;
            SetProjection((float)Width / Height);
        }

        public void ToggleFullScreen()
        {
            m_Graphics.ToggleFullScreen();
        }

        public void Draw(float fDeltaTime)
        {
            // Clear back buffer
            //m_Graphics.GraphicsDevice.Clear(Color.Black);


            // First draw all 3D components
            m_Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            m_Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //foreach (PhysicsGameObjectToDraw o in m_Objects) {
            ////     if (o.GObject.Enabled) {
            //         o.GObject.Draw(fDeltaTime);
            //     }
            // }
            foreach (PhysicsGameObject o in m_Objects)
            {
                if (o.Enabled) o.Draw(fDeltaTime);
            }

            // Now draw all 2D components
            m_SpriteBatch.Begin();

            // Draw the UI screens
            GameState.Get().DrawUI(fDeltaTime, m_SpriteBatch);

            // Draw FPS counter
            Vector2 vFPSPos = Vector2.Zero;
            if (DebugDefines.bShowBuildString)
            {
                m_SpriteBatch.DrawString(m_FPSFont, DebugDefines.DebugName, vFPSPos, Color.White);
                vFPSPos.Y += 25.0f;
            }
            if (DebugDefines.bShowFPS)
            {
                //draw FPS
                string sFPS = String.Format("FPS: {0}", (int)(1 / fDeltaTime));
                m_SpriteBatch.DrawString(m_FPSFont, sFPS, vFPSPos, Color.White);
            }
            if (GameState.Get().State == eGameState.Gameplay)
            {
                
                if (GameState.Get().DebugMode)
                    this.hud.drawDebug(m_SpriteBatch,m_FPSFont,m_Game);
                if (GameState.Get().Camera is Camera.CockpitCamera)
                {
                   this.hud.Draw(fDeltaTime, m_SpriteBatch, m_FPSFont, m_KeyFont, m_TextFont, m_Game);
                }
                else
                {
                     this.hud.DrawTextHUD(m_SpriteBatch, m_FPSFont, m_Game);
                }
                if (GameState.Get().GameMode == eGameplayState.TUTORIAL)
                {
                    m_SpriteBatch.DrawString(this.m_GameFont, this.tutorialMsgs.getMessage(), TutorialMessages.SCREEN_POSITION, Color.Black);
                }
                else
                {
                    m_SpriteBatch.DrawString(this.m_GameFont, "Eliminate all enemies", TutorialMessages.SCREEN_POSITION, Color.Black);
                }
            }
            m_SpriteBatch.End();
        }
        public void AddGameObject(PhysicsGameObject o)
        {
            //this.m_Objects.AddLast(new PhysicsGameObjectToDraw(o,o.getDrawOrder()));
            //this.m_Objects = this.m_Objects.OrderBy(obj => obj.Order).ToList();
            this.m_Objects.AddLast(o);
        }

        public void RemoveGameObject(PhysicsGameObject o)
        {
            m_Objects.Remove(o);
        }

        public void ClearAllObjects()
        {
            this.m_Objects.Clear();
        }

        // Draws a line
        public void DrawLine(SpriteBatch batch, float width, Color color,
            Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(m_Blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        public void DrawLine3D(SpriteBatch batch, float width, Color color, Vector3 point1, Vector3 point2)
        {
            // Convert the 3D points into screen space points
            Vector3 point1_screen = GraphicsDevice.Viewport.Project(point1, Projection,
                GameState.Get().CameraMatrix, Matrix.Identity);
            Vector3 point2_screen = GraphicsDevice.Viewport.Project(point2, Projection,
                GameState.Get().CameraMatrix, Matrix.Identity);

            // Now draw a 2D line with the appropriate points
            DrawLine(batch, width, color, new Vector2(point1_screen.X, point1_screen.Y),
                new Vector2(point2_screen.X, point2_screen.Y));
        }

        public void DrawFilled(SpriteBatch batch, Rectangle rect, Color color, float outWidth, Color outColor)
        {
            // Draw the background
            batch.Draw(m_Blank, rect, color);

            // Draw the outline
            DrawLine(batch, outWidth, outColor, new Vector2(rect.Left, rect.Top),
                new Vector2(rect.Right, rect.Top));
            DrawLine(batch, outWidth, outColor, new Vector2(rect.Left, rect.Top),
                new Vector2(rect.Left, rect.Bottom + (int)outWidth));
            DrawLine(batch, outWidth, outColor, new Vector2(rect.Left, rect.Bottom),
                new Vector2(rect.Right, rect.Bottom));
            DrawLine(batch, outWidth, outColor, new Vector2(rect.Right, rect.Top),
                new Vector2(rect.Right, rect.Bottom));
        }
    }
}
