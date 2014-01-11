using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Henge3D;
using Henge3D.Physics;
using itp380.Objects.PhysicsModules;

namespace itp380.Objects.Units
{
    public enum MissileState
    {
        ATTACHED_LEFT, ATTACHED_RIGHT, FIRING_LEFT, FIRING_RIGHT, FIRED
    }
    public class Missile : PhysicsGameObject
    {
        public override int getDrawOrder()
        {
            return 6;
        }
        public static readonly float ANGULAR_HOMING_COEFFICIENT = 100f;
        public static readonly float DIRECTIONAL_HOMING_CONVERSION = .35f;

        public static readonly float LINEAR_DRAG_COEFFICIENT = Aircraft.LINEAR_DRAG_COEFFICIENT;//(float)(1.2 * .000001);
        public static readonly float ANGULAR_DRAG_COEFFICIENT = Aircraft.ANGULAR_DRAG_COEFFICIENT;//.01f;
        public static readonly float MAX_FUEL = 5000000;
        PhysicsManager pm;
        public static readonly float THRUST_MULTIPLIER = 150000;//200000;//max aircraft speed is 100*500 (thrust S* gain)
        protected MissileState state;
        protected float fire_delay = 0f;
        public float Damage { get; set; }
        LinearDragModule m_dragModule;
        ThrustModule m_thrustModule;
        AngularDragModule m_angDragModule;
        protected AngularHomingModule m_AngularHomingModule;
        protected DirectionalHomingModule m_DirectionalHomingModule;

        public Missile(PhysicsManager physicsenabler, float damage)
            : base(GameState.Get().Game.Content.Load<Model>("missile"))
        {
            this.Damage = damage;
            this.pm = physicsenabler;
            SetWorld(1f,
                    new Vector3(110, 10, 110),
                    Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI / 5));//* Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI / 4));
            LinearVelocity = Vector3.Zero;
            AngularVelocity = Vector3.Zero;

            this.m_dragModule = new LinearDragModule(this, LINEAR_DRAG_COEFFICIENT);
            this.m_thrustModule = new ThrustModule(this, THRUST_MULTIPLIER, 1f, MAX_FUEL);
            this.m_angDragModule = new AngularDragModule(this, ANGULAR_DRAG_COEFFICIENT);

            Capsule capsule = new Capsule(new Vector3(5, 0, 0), Vector3.Zero, 1);
            Vector3 center = new Vector3(2.5f, 0, 0);
            MassProperties mp = MassProperties.FromCapsule(0.15f, capsule.P1, capsule.P2, capsule.Radius, out center);
            this.MassProperties = mp;
            this.Skin.Add(new CapsulePart(capsule));
        }
        public virtual void fire(PhysicsGameObject target)
        {
            this.m_AngularHomingModule = new AngularHomingModule(this, target, ANGULAR_HOMING_COEFFICIENT);
            this.m_DirectionalHomingModule = new DirectionalHomingModule(this, target, DIRECTIONAL_HOMING_CONVERSION);
            this.pm.Add(this);
            this.state = MissileState.FIRED;
            
        }
        public override void Update(float fDeltaTime)
        {
            // System.Diagnostics.Debug.WriteLine(this.state);
            base.Update(fDeltaTime);

            if (this.state == MissileState.FIRED || this.state == MissileState.FIRING_RIGHT || this.state == MissileState.FIRING_LEFT)
            {
                Vector3 dragmag = this.m_dragModule.tick(fDeltaTime);
                Vector3 angdragmag = this.m_angDragModule.tick(fDeltaTime);
                //System.Diagnostics.Debug.WriteLine(dragmag.Length());
            }
            if (this.state == MissileState.FIRED)
            {
                Vector3 thrustmag = this.m_thrustModule.tick(fDeltaTime);
                if (this.m_AngularHomingModule != null)
                {
                    this.m_AngularHomingModule.tick(fDeltaTime);
                }
                if (this.m_DirectionalHomingModule != null)
                {
                    this.m_DirectionalHomingModule.tick(fDeltaTime);
                    //System.Diagnostics.Debug.WriteLine("Distance to target: " + (this.m_DirectionalHomingModule.Target.Position - this.Position).Length());
                }
                //System.Diagnostics.Debug.WriteLine(thrustmag.Length());
                //System.Diagnostics.Debug.WriteLine(this.LinearVelocity.Length());
            }
        }
        public override Vector3 Forward
        {
            get
            {
                return this.Transform.Combined.Left;
            }
        }
    }
}
