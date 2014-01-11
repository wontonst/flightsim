using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using itp380.Patterns;

namespace itp380.Graphics {
    public class World : Singleton<World> {

       // private Camera camera;
        private Ocean ocean;
        private SkyBox skybox;
        private Effect skyboxEffect;
        private TextureCube skyTex;
        GraphicsDeviceManager graphics;
        Boolean enabled = false;
        public Boolean Enabled { set { this.enabled = value; } get { return this.enabled; } }
        public World() {
            this.graphics = PhysicsGraphicsManager.Get().GraphicsDeviceManager;
            Global.Graphics = graphics.GraphicsDevice;
        }
        public void load() {
            Game g = GameState.Get().Game;
            skybox = new SkyBox();
            ocean = new Ocean();
           // camera = new Camera(g);

            ocean.Load(g.Content);

            skyboxEffect = g.Content.Load<Effect>("SkyShader");
            skyTex = g.Content.Load<TextureCube>("Sky");
        }
        public void Draw(GameTime gameTime) {
            if (!this.enabled) return;
            //Matrix proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Global.ScreenWidth / (float)Global.ScreenHeight, 0.01f, 1000000.0f);
            Matrix proj = PhysicsGraphicsManager.Get().Projection;
            Global.Graphics.RasterizerState = RasterizerState.CullNone;
            Global.Graphics.DepthStencilState = DepthStencilState.None;

            // set the sky shader variables
           // skyboxEffect.Parameters["View"].SetValue(camera.CameraMatrix);
            skyboxEffect.Parameters["View"].SetValue(GameState.Get().CameraMatrix);
            skyboxEffect.Parameters["Projection"].SetValue(proj);
            skyboxEffect.Parameters["cubeTex"].SetValue(skyTex);
            //skyboxEffect.CommitChanges();
            skyboxEffect.CurrentTechnique.Passes[0].Apply();

            // and draw
            skybox.Draw();

            //skyboxEffect.CurrentTechnique.Passes[0].End();
            //skyboxEffect.End();

            // turn depth buffering back on for the ocean
            //Global.Graphics.RenderState.DepthBufferEnable = true;
            Global.Graphics.DepthStencilState = DepthStencilState.Default;
            Global.Graphics.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            // and allow the ocean to draw itself
//            ocean.Draw(gameTime, camera, skyTex, proj);
            ocean.Draw(gameTime, GameState.Get().Camera, skyTex, proj);
        }
        public void Update(float fDeltaTime){
            this.ocean.Update(GameState.Get().Aircraft);
        }
    }
}
