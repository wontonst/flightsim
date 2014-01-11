using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects.PhysicsModules
{
    public class AngularHomingModule : HomingModule
    {
        float coefficient;
        public AngularHomingModule(PhysicsGameObject parent, PhysicsGameObject target, float coefficient)
            : base(parent,target)
        {
            this.coefficient = coefficient;
        }
        public override Vector3 tick(float fdelta)
        {
            if (this.Target == null) return Vector3.Zero;
            Vector3 targetingVec = this.TargetPosition - this.parent.Position;
            targetingVec.Normalize();
            Vector3 homer = -1*Vector3.Cross(targetingVec, this.parent.Forward);
            homer.Normalize();
            float magnitude = this.coefficient * (float)Math.Acos( Vector3.Dot(targetingVec, this.parent.Forward));
            homer *= magnitude;


            this.parent.ApplyTorque(ref homer);
            return homer;
        }
    }
}
