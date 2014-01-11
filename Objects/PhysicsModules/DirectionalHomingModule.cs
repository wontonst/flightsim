using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects.PhysicsModules
{
    public class DirectionalHomingModule : HomingModule
    {
        float coefficient;
        public DirectionalHomingModule(PhysicsGameObject parent, PhysicsGameObject target, float conversion_percent)
            : base(parent, target)
        {
            this.coefficient = conversion_percent;
        }
        public override Microsoft.Xna.Framework.Vector3 tick(float fdelta)
        {
            if (this.Target == null) return Vector3.Zero;
            Vector3 speedRedux = this.parent.LinearVelocity;
            speedRedux.Normalize();
            speedRedux *= -1 * this.coefficient;
            Vector3 directionMod = (this.parent.Position - this.TargetPosition);
            directionMod.Normalize();
            directionMod *= 1 - this.coefficient;
            Vector3 apply = speedRedux + directionMod;
            this.parent.ApplyForce(ref apply);
            return apply;
        }
    }
}
