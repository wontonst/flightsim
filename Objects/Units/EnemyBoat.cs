using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Henge3D;
using Henge3D.Physics;

namespace itp380.Objects.Units 
{
    public class EnemyBoat : PhysicsGameObject
    {
        public override int getDrawOrder()
        {
            return 5;
        }
        SpriteBatch m_SpriteBatch;

        float fAccuracyCoefficient = 15.0f; //Determines  accuracy, 0 = perfect aim
        float fAttackRange = 20000.0f; //How close the plane must be for the ship to fire at it

        Vector3 vTarget_Position = Vector3.Zero; //position of target aircraft (aka the player)
        public Vector3 vFireAt_Position = Vector3.Zero; //actual position boat is shooting at for every given bullet fired
        float fTarget_Distance = 0.0f;
        float fRateOfFire = 0.5f; //1.0 = 1 bullet/sec, 0.5 = 2 bullets/sec ...etc.
        bool canFire = true;
        bool drawBullet = false;

        public float health;

        Random random = new Random();
        
        public EnemyBoat(Vector3 startpos)
        : base(GameState.Get().Game.Content.Load<Model>("blahship")) 
        {
            m_SpriteBatch = new SpriteBatch(GraphicsManager.Get().GraphicsDevice);
           // Capsule massCapsule = new Capsule(new Vector3(5, 0, 0), Vector3.Zero, 44);
            Capsule massCapsule = new Capsule(new Vector3(400, 0, 0), new Vector3(-100,  0, 0), 450);
            Vector3 center = Vector3.Zero;
            MassProperties mp = MassProperties.FromCapsule(05.15f, massCapsule.P1, massCapsule.P2, massCapsule.Radius, out center);
            this.MassProperties = mp;
            this.Skin.Add(new CapsulePart(massCapsule));

            this.SetWorld(100f, startpos, Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0f));

            health = 100.0f;
        }
        public override Vector3 Forward
        {
            get
            {
                return base.Left;
            }
        }
        public override Vector3 Left
        {
            get
            {
                return base.Forward;
            }
        }
        public Segment FireBullet()
        {
            canFire = false;
            drawBullet = true;

            //calculate innacuracy offset
            Vector3 vOffset = new Vector3((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f);
            vOffset *= fAccuracyCoefficient;
            vFireAt_Position = vTarget_Position + vOffset;
 
            m_Timer.AddTimer("EnemyBoat Fire Cooldown", fRateOfFire, resetFire, false);
            Segment Ray = new Segment(this.Position, vFireAt_Position);
            GameState.Get().ParticlesManager.effectFlakBurst(vFireAt_Position + GameState.Get().Aircraft.Forward * 15.0f);
            
            return Ray;
        }

        public bool CanFire()
        {
            //check range and cooldown
            return (fTarget_Distance <= fAttackRange && canFire && this.health > 0);
        }

        public void resetFire()
        {
            canFire = true;
        }

        public void StopDrawingBullet()
        {
            drawBullet = false;
        }

        public void takeDamage(float amt)
        {
            this.health -= amt;
            //System.Diagnostics.Debug.WriteLine("Enemy Health: " + this.health);
        }

        public override void Draw(float fDeltaTime)
        {

         //   m_SpriteBatch.Begin();
         //  {
         //       if (drawBullet)
         //       {
         //           PhysicsGraphicsManager.Get().DrawLine3D(m_SpriteBatch, 1.0f, Color.Yellow, this.Position, vFireAt_Position);
         //           m_Timer.AddTimer("Bullet Animation", 0.25f , StopDrawingBullet, false);
         //       }
         //   }
         //   m_SpriteBatch.End();

            base.Draw(fDeltaTime);
        }


        public override void Update(float fDeltaTime)
        {
            //update plane's position and distance from boat
            vTarget_Position = GameState.Get().Aircraft.Position;
            fTarget_Distance = Vector3.Distance(vTarget_Position, this.Position);
            //System.Diagnostics.Debug.WriteLine("Boat Distance: " + fTarget_Distance);
            //System.Diagnostics.Debug.WriteLine(this.Position);
            if (this.Position.Y > 150 || this.Position.Y < 30)
            {
                Vector3 pos = this.Position;
                pos.Y = 100;
                this.SetWorld(pos);
                this.SetVelocity(Vector3.Zero, Vector3.Zero);
            }
            base.Update(fDeltaTime);
        }
    }
}