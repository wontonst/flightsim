using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using itp380;
using itp380.Camera;
using itp380.Objects.Units;

namespace itp380
{
    /// <summary>
    /// A class to draw an ocean
    /// </summary>
    class Ocean
    {

        private static int MAP_WIDTH = 100000;

        // the ocean's required content
        private Effect oceanEffect;
        private Texture2D[] OceanNormalMaps;

        // The two-triangle generated model for the ocean
        private VertexPositionNormalTexture[] OceanVerts;
        private VertexDeclaration OceanVD;

        /// <summary>
        /// Creates an Ocean object
        /// </summary>
        public Ocean()
        {

        }

        /// <summary>
        /// Loads the content necessary for the ocean
        /// </summary>
        /// <param name="Content"></param>
        public void Load(ContentManager Content)
        {
            // load the shader
            oceanEffect = Content.Load<Effect>("OceanShader");
            // load the normal maps
            OceanNormalMaps = new Texture2D[4];
            for (int i = 0; i < 4; i++)
                OceanNormalMaps[i] = Content.Load<Texture2D>("Ocean" + (i + 1) + "_N");

            // generate the geometry
            OceanVerts = new VertexPositionNormalTexture[6];
            OceanVerts[0] = new VertexPositionNormalTexture(new Vector3(-MAP_WIDTH, 0, -MAP_WIDTH), Vector3.Up, new Vector2(0, 0));
            OceanVerts[1] = new VertexPositionNormalTexture(new Vector3(MAP_WIDTH, 0, -MAP_WIDTH), Vector3.Up, new Vector2(1, 0));
            OceanVerts[2] = new VertexPositionNormalTexture(new Vector3(-MAP_WIDTH, 0, MAP_WIDTH), Vector3.Up, new Vector2(0, 1));
            OceanVerts[3] = OceanVerts[2];
            OceanVerts[4] = OceanVerts[1];
            OceanVerts[5] = new VertexPositionNormalTexture(new Vector3(MAP_WIDTH, 0, MAP_WIDTH), Vector3.Up, new Vector2(1, 1));
            OceanVD = new VertexDeclaration(VertexPositionNormalTexture.VertexDeclaration.GetVertexElements());
        }
        public void Update(Aircraft a)
        {
            OceanVerts[0].Position = new Vector3(-MAP_WIDTH + a.Position.X, 0, -MAP_WIDTH + a.Position.Z);
            OceanVerts[1].Position = new Vector3(MAP_WIDTH + a.Position.X, 0, -MAP_WIDTH + a.Position.Z);
            OceanVerts[2].Position = new Vector3(-MAP_WIDTH + a.Position.X, 0, MAP_WIDTH + a.Position.Z);
            OceanVerts[3].Position = OceanVerts[2].Position;
            OceanVerts[4].Position = OceanVerts[1].Position;
            OceanVerts[5].Position = new Vector3(MAP_WIDTH + a.Position.X, 0, MAP_WIDTH + a.Position.Z);
        }
        public void Draw(GameTime gameTime, ICamera cam, TextureCube skyTexture, Matrix proj)
        {
            // start the shader
            //oceanEffect.Begin();
            //oceanEffect.CurrentTechnique.Passes[0].Begin();

            // set the transforms
            oceanEffect.Parameters["World"].SetValue(Matrix.Identity);
            oceanEffect.Parameters["View"].SetValue(cam.CameraMatrix);
            oceanEffect.Parameters["Projection"].SetValue(proj);
            oceanEffect.Parameters["EyePos"].SetValue(cam.Position);

            // choose and set the ocean textures
            int oceanTexIndex = ((int)(gameTime.TotalGameTime.TotalSeconds) % 4);
            oceanEffect.Parameters["normalTex"].SetValue(OceanNormalMaps[(oceanTexIndex + 1) % 4]);
            oceanEffect.Parameters["normalTex2"].SetValue(OceanNormalMaps[(oceanTexIndex) % 4]);
            oceanEffect.Parameters["textureLerp"].SetValue((((((float)gameTime.TotalGameTime.TotalSeconds) - (int)(gameTime.TotalGameTime.TotalSeconds)) * 2 - 1) * 0.5f) + 0.5f);

            // set the time used for moving waves
            oceanEffect.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds * 0.02f);

            // set the sky texture
            oceanEffect.Parameters["cubeTex"].SetValue(skyTexture);

            //oceanEffect.CommitChanges();
            oceanEffect.CurrentTechnique.Passes[0].Apply();
            // draw our geometry
            //Global.Graphics.VertexDeclaration = OceanVD;
            Global.Graphics.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, OceanVerts, 0, 2);

            // and we're done!
            //oceanEffect.CurrentTechnique.Passes[0].End();
            //oceanEffect.End();
        }
    }
}
