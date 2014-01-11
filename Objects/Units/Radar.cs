using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using itp380.Objects;


namespace itp380.Objects.Units
{
    public class Radar 
    {
        public static readonly float RADAR_RADIUS = 65;
        private SpriteBatch spriteBatch;
        GraphicsDevice graphicsDevice;


        private bool enabled = false;


        private float scale = 1.0f;
        private int dimension;


        private Vector2 position;
        public void setPosition(Vector2 pos)
        {
            this.position = pos;
        }

        private Texture2D backgroundImage;

        public float currentAngle = 0;


        private Rectangle[] objectPositions;
        private int highlight;

        public static readonly float SCALE = 0.5f;
        public static readonly int DIMENSION = 1;

        public Radar(Vector2 position, Texture2D backgroundImage, float scale, int dimension, GraphicsDevice graphicsDevice)
            
        {
            this.position = position;


            this.backgroundImage = backgroundImage;


            
            this.graphicsDevice = graphicsDevice;


            this.scale = scale;
            this.dimension = dimension;
        }


        public void Update(Vector3[] objects, int highlight, float currentAngle, Vector3 myPosition)
        {
            this.objectPositions = new Rectangle[objects.Length];
            this.highlight = highlight;
            this.currentAngle = currentAngle;

            for (int i = 0; i != objects.Length; i++)
            {
                Vector3 temp = objects[i];
                temp.X = (myPosition.X-temp.X)  / dimension * backgroundImage.Width / 2 * scale;
                temp.Y = 0;
                temp.Z = (myPosition.Z-temp.Z ) / dimension * backgroundImage.Height / 2 * scale;

                if (temp.Length() > RADAR_RADIUS)
                    continue;
               //float angle = (float)(Math.Acos(Vector3.Dot(objects[i], Vector3.UnitX)));
                temp = Vector3.Transform(temp, Matrix.CreateRotationY(currentAngle));

                Rectangle backgroundRectangle = new Rectangle();
                backgroundRectangle.Width = 2;
                backgroundRectangle.Height = 2;
                backgroundRectangle.X = (int)(position.X + temp.X);
                backgroundRectangle.Y = (int)(position.Y + temp.Z);
                this.objectPositions[i] = backgroundRectangle;
            }

            //System.Console.WriteLine("Update");
        }


        public void Enable(bool enabled)
        {
            this.enabled = enabled;
            //System.Console.WriteLine("Enable");
        }


        public void Draw(SpriteBatch b)
        {
            this.spriteBatch = b;
            if (enabled)
            {
               
               // spriteBatch.Draw(backgroundImage, position, null, Color.White, 0, new Vector2(20f, 20f), scale, SpriteEffects.None, 0);


                for(int i = 0 ; i != this.objectPositions.Length ;i++)
                {
                    Color myTransparentColor = new Color(255, 0, 0);
                    if (highlight == i)
                    {
                        myTransparentColor = new Color(255, 255, 0);
                    }
                    else if (highlight > i)
                    {
                        myTransparentColor = new Color(0, 255, 0);
                    }


                    Rectangle backgroundRectangle = this.objectPositions[i];


                    Texture2D dummyTexture = new Texture2D(graphicsDevice, 1, 1);
                    dummyTexture.SetData(new Color[] { myTransparentColor });


                    spriteBatch.Draw(dummyTexture, backgroundRectangle, myTransparentColor);
                }


                //myPosition.X = backgroundImage.Width / 2 * scale;
                //myPosition.Z = backgroundImage.Height / 2 * scale;


                ////myPosition = Vector3.Transform(myPosition, Matrix.CreateRotationY(MathHelper.ToRadians(currentAngle)));


                //Rectangle backgroundRectangle2 = new Rectangle();
                //backgroundRectangle2.Width = 5;
                //backgroundRectangle2.Height = 5;
                //backgroundRectangle2.X = (int)(position.X + myPosition.X);
                //backgroundRectangle2.Y = (int)(position.Y + myPosition.Z);


                //Texture2D dummyTexture2 = new Texture2D(graphicsDevice, 1, 1);
                //dummyTexture2.SetData(new Color[] { Color.Pink });


                //spriteBatch.Draw(dummyTexture2, backgroundRectangle2, Color.Pink);
            }

            //System.Console.WriteLine("Draw");
        }
    }


}

