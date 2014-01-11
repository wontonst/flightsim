using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using itp380.Objects.PhysicsModules;
using Microsoft.Xna.Framework;

namespace itp380.Objects {
    public class LinearDragModule : Module {
        public static readonly float ANGULAR_GAIN = 1.5f;//max multiplier for angular effect on linear drag. MUST BE >1
        float linear_coefficient;
        public LinearDragModule(PhysicsGameObject obj, float linear_coefficient)
            : base(obj) {
            this.linear_coefficient = linear_coefficient;
        }
        public void setCoefficient(float coefficient)
        {
            this.linear_coefficient = coefficient;
        }
        public override Vector3 tick(float fdelta) {
            Vector3 drag = -1 * this.parent.LinearVelocity * linear_coefficient * this.parent.LinearVelocity.LengthSquared() * fdelta;

            //going to calculate angular affect; this next line finds angle between forward and velocity direction
            float angularMultiplier = (float)Math.Acos(Vector3.Dot(this.parent.LinearVelocity, this.parent.Forward) / (this.parent.LinearVelocity.Length() * this.parent.Forward.Length()));
            //drag is at peak when forward is normal to velocity direction, and angle = Math.PI when facing backwards, 
            //so we want to maximize drag when at midpoint. If past midpoint, we adjust angle to make calculations easier.
            if (angularMultiplier > Math.PI / 2)
                angularMultiplier = (float)Math.PI - angularMultiplier;
            //convert multiplier to a number between 1 and ANGULAR_GAIN
            angularMultiplier = 1 + (float)(angularMultiplier * (ANGULAR_GAIN - 1) / (Math.PI / 2));
            //if angularMultiplier is negligible, do not apply (this prevents some weird issues of NaN)
            if (angularMultiplier > 1.01)
                drag *= angularMultiplier;
            //System.Diagnostics.Debug.WriteLine(angularMultiplier + "");
            this.parent.ApplyForce(ref drag);

            return drag;
        }
    }
}
