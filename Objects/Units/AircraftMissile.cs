using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Henge3D.Physics;
using Microsoft.Xna.Framework;
using itp380.Objects.PhysicsModules;
using itp380.Camera;

namespace itp380.Objects.Units
{
    public class AircraftMissile : Missile
    {
        private Aircraft parent;
        public AircraftMissile(PhysicsManager physicsenabler, Aircraft parent, MissileState state)
            : base(physicsenabler, Aircraft.MISSILE_BASE_DAMAGE)
        {
            this.parent = parent;
            this.state = state;
            this.fire_delay = Aircraft.MISSILE_IGNITION_DELAY;
            this.Damage = 100f;
        }

        public override void Update(float fDeltaTime)
        {
            base.Update(fDeltaTime);
            if (this.state == MissileState.ATTACHED_RIGHT)
            {
                Vector3 lmispos = this.parent.Position;
                lmispos += this.parent.Left * Aircraft.MISSILE_SIDE_OFFSET;
                lmispos += this.parent.Up * -Aircraft.MISILE_TOP_OFFSET;
                lmispos += -1 * this.parent.Forward * Aircraft.MISSILE_FRONT_OFFSET;
                this.SetWorld(lmispos, Quaternion.CreateFromAxisAngle(this.parent.Up, (float)Math.PI / 2) * this.parent.Orientation);
                //System.Diagnostics.Debug.Write(lmispos.X + "," + lmispos.Y);
            }
            else if (this.state == MissileState.ATTACHED_LEFT)
            {
                Vector3 rightpos = this.parent.Position;
                rightpos += this.parent.Right * Aircraft.MISSILE_SIDE_OFFSET;
                rightpos += this.parent.Up * -Aircraft.MISILE_TOP_OFFSET;
                rightpos += this.parent.Forward * -Aircraft.MISSILE_FRONT_OFFSET;
                this.SetWorld(rightpos, Quaternion.CreateFromAxisAngle(this.parent.Up, (float)Math.PI / 2) * this.parent.Orientation);
                //System.Diagnostics.Debug.WriteLine(rightpos.X + "," + rightpos.Y);
            }


            else if (this.state == MissileState.FIRING_LEFT || this.state == MissileState.FIRING_RIGHT)
            {
                this.fire_delay -= fDeltaTime;
                if (this.fire_delay <= 0)
                {
                    this.state = MissileState.FIRED;
                    SoundManager.Get().PlaySoundCue("MissileLaunch");
                }
            }
        }
        public override void fire(PhysicsGameObject target)
        {
            //System.Diagnostics.Debug.WriteLine("firing at " + target + " at " + target.Position);
            base.fire(target);
            this.m_AngularHomingModule.setFwdOffset(-10);
            this.m_DirectionalHomingModule.setFwdOffset(-10);
            this.m_AngularHomingModule.setUpOffset(3);
            this.m_DirectionalHomingModule.setUpOffset(3);
            this.SetVelocity(this.parent.LinearVelocity, this.parent.AngularVelocity);
            this.state = this.state == MissileState.ATTACHED_LEFT ? MissileState.FIRING_LEFT : MissileState.FIRING_RIGHT;
            GameState.Get().setMissileCamera(new FollowCamera(GameState.Get().Game, this));
        }
    }
}
