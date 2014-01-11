using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects.PhysicsModules
{
    public class DamageModule : Module
    {
        float yaw = 0, pitch = 0, roll = 0;
        public DamageModule(PhysicsGameObject parent, float yaw_coefficient, float pitch_coefficient, float roll_coefficient)
            : base(parent)
        {
            this.yaw = yaw_coefficient; this.pitch = pitch_coefficient; this.roll = roll_coefficient;
        }
        public void modifyRoll(float magnitude)
        {
            this.roll += magnitude;
        }
        public void modifyPitch(float magnitude)
        {
            this.pitch += magnitude;
        }
        public void modifyYaw(float magnitude)
        {
            this.yaw += magnitude;
        }
        public override Microsoft.Xna.Framework.Vector3 tick(float fdelta)
        {
            Vector3 power = Vector3.Zero;
            if (this.yaw != 0)
            {
                power += this.parent.Up * yaw;
            }
            if (this.roll != 0)
            {
                power += this.parent.Forward * roll;
            }
            if (this.pitch != 0)
            {
                power += this.parent.Left * pitch;
            }
            power *= fdelta;
            this.parent.ApplyTorque(ref power);
            return power;
        }
    }
}
