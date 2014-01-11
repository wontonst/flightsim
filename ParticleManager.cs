using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using itp380.Objects.Units;
using Particle3DSample;
using itp380.Objects;
using Microsoft.Xna.Framework;

namespace itp380
{
    public class ParticleManager
    {

        public static readonly float PLANE_AFTERBURNER_FORWARD_OFFSET = 20;
        public static readonly float AFTERBURNER_SMOKE_SPAWN_RATE = .01f;
        public static readonly float DESTROYED_SHIP_SMOKE_SPAWN_RATE = 0.4f;

        public bool Enabled { get; set; }

        ParticleSystem explosionParticles;
        ParticleSystem explosionSmokeParticles;
        ParticleSystem projectileTrailParticles;
        ParticleSystem smokePlumeParticles;
        ParticleSystem fireParticles;
        ParticleSystem shipSmokeParticles;

        List<Aircraft> aircrafts = new List<Aircraft>();
        List<PhysicsGameObject> missiles = new List<PhysicsGameObject>();
        List<PhysicsGameObject> destroyedShips = new List<PhysicsGameObject>();
        double aircrafttime = 0;
        double destroyedtime = 0;
        Random rand = new Random();
        public ParticleManager()
        {

            explosionParticles = new ExplosionParticleSystem(GameState.Get().Game, GameState.Get().Game.Content);
            explosionSmokeParticles = new ExplosionSmokeParticleSystem(GameState.Get().Game, GameState.Get().Game.Content);
            projectileTrailParticles = new ProjectileTrailParticleSystem(GameState.Get().Game, GameState.Get().Game.Content);
            smokePlumeParticles = new SmokePlumeParticleSystem(GameState.Get().Game, GameState.Get().Game.Content);
            fireParticles = new FireParticleSystem(GameState.Get().Game, GameState.Get().Game.Content);
            shipSmokeParticles = new ShipSmokeParticleSystem(GameState.Get().Game, GameState.Get().Game.Content);

            smokePlumeParticles.DrawOrder = 100;
            explosionSmokeParticles.DrawOrder = 200;
            projectileTrailParticles.DrawOrder = 300;
            explosionParticles.DrawOrder = 400;
            fireParticles.DrawOrder = 500;
            shipSmokeParticles.DrawOrder = 100;

            GameState.Get().Game.Components.Add(explosionParticles);
            GameState.Get().Game.Components.Add(explosionSmokeParticles);
            GameState.Get().Game.Components.Add(projectileTrailParticles);
            GameState.Get().Game.Components.Add(smokePlumeParticles);
            GameState.Get().Game.Components.Add(fireParticles);
            GameState.Get().Game.Components.Add(shipSmokeParticles);
        }
        public void addAircraft(Aircraft a)
        {
            this.aircrafts.Add(a);
        }
        public void removeAircraft(Aircraft a)
        {
            this.aircrafts.Remove(a);
        }
        public void addMissile(PhysicsGameObject m)
        {
            this.missiles.Add(m);
        }
        public void removeMissile(PhysicsGameObject m)
        {
            this.missiles.Remove(m);
        }
        public void Draw()
        {
            explosionParticles.SetCamera(GameState.Get().CameraMatrix, PhysicsGraphicsManager.Get().Projection);
            explosionSmokeParticles.SetCamera(GameState.Get().CameraMatrix, PhysicsGraphicsManager.Get().Projection);
            projectileTrailParticles.SetCamera(GameState.Get().CameraMatrix, PhysicsGraphicsManager.Get().Projection);
            smokePlumeParticles.SetCamera(GameState.Get().CameraMatrix, PhysicsGraphicsManager.Get().Projection);
            fireParticles.SetCamera(GameState.Get().CameraMatrix, PhysicsGraphicsManager.Get().Projection);
            this.shipSmokeParticles.SetCamera(GameState.Get().CameraMatrix, PhysicsGraphicsManager.Get().Projection);
            this.projectileTrailParticles.SetCamera(GameState.Get().CameraMatrix, PhysicsGraphicsManager.Get().Projection);
        }
        public void Update(double fDeltaTime)
        {
            aircrafttime += fDeltaTime;
            destroyedtime += fDeltaTime;
            foreach (Aircraft obj in this.aircrafts)
            {
                int thrustMultipler = (int)obj.Thrust / 25;
                int numToSpawn = thrustMultipler * (1 + (int)(fDeltaTime / AFTERBURNER_SMOKE_SPAWN_RATE));
                Vector3 burner_position = obj.Position;
                burner_position -= obj.Forward * PLANE_AFTERBURNER_FORWARD_OFFSET;
                for (int i = 0; i != thrustMultipler; i++)
                    this.fireParticles.AddParticle(burner_position, Vector3.Zero);// obj.LinearVelocity);
                for (int i = 0; i != numToSpawn; i++)
                {
                    this.smokePlumeParticles.AddParticle(burner_position + obj.Left * this.rand.Next(-5, 5), Vector3.Zero);//obj.LinearVelocity);
                }
            }
            if (destroyedtime >= DESTROYED_SHIP_SMOKE_SPAWN_RATE)
            {
                foreach (PhysicsGameObject pos in this.destroyedShips)
                {
                    int numToSpawn = 1;// +(int)(fDeltaTime / DESTROYED_SHIP_SMOKE_SPAWN_RATE);
                    Vector3 spawnpos = pos.Position + pos.Up * 8.5f + pos.Left * -3;
                    for (int i = 0; i != numToSpawn; i++)
                    {
                        for (int ii = -16; ii != 0; ii += 4)
                        {
                            this.shipSmokeParticles.AddParticle(spawnpos + pos.Forward * ii, Vector3.Zero);
                            this.fireParticles.AddParticle(spawnpos + pos.Forward * ii, Vector3.Zero);
                        }
                    }
                }
                destroyedtime = 0;
            }
        }
        public void effectFlakBurst(Vector3 position)
        {
            
            for (int i = 0; i != 5; i++)
            {
                this.explosionParticles.AddParticle(position + new Vector3(this.rand.Next(-5, 5), this.rand.Next(-5, 5), this.rand.Next(-5, 5)), Vector3.Zero);
            }
        }
        public void effectExplosion(Vector3 position)
        {
            for (int i = 0; i != 120; i++)
            {
                this.explosionParticles.AddParticle(position + new Vector3(this.rand.Next(-15, 15), this.rand.Next(-15, 15), this.rand.Next(-15, 15)), Vector3.Zero);
            }
            for (int i = 0; i != 120; i++)
            {
                this.explosionParticles.AddParticle(position + new Vector3(this.rand.Next(-15, 15), this.rand.Next(-15, 15), this.rand.Next(-15, 15)), Vector3.Zero);
            }
        }
        public void addDestroyedShipPosition(PhysicsGameObject ship)
        {
            this.destroyedShips.Add(ship);
        }
    }
}
