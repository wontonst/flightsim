using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects.PhysicsModules {
    public class AngularDragModule : Module{

        float angular_coefficient;

        public AngularDragModule(PhysicsGameObject obj, float coefficient) : base(obj) {
            this.angular_coefficient = coefficient;
        }
        public override Vector3 tick(float fdelta) {
            Vector3 linVel = this.parent.LinearVelocity;
            linVel.Normalize();
            float magnitude = angular_coefficient * fdelta * this.parent.LinearVelocity.LengthSquared();
            float angleOfAttack = (float)Math.Abs(Math.Acos(Vector3.Dot(linVel, this.parent.Forward)));
            magnitude *= angleOfAttack * angleOfAttack;

            //System.Diagnostics.Debug.WriteLine((float)Math.Abs(Math.Acos(Vector3.Dot(linVel, this.parent.Forward))));

            Vector3 rotateAxis = Vector3.Cross(this.parent.Forward, this.parent.LinearVelocity);
            rotateAxis *= magnitude;
            this.parent.ApplyTorque(ref rotateAxis);
            return rotateAxis;
        }
    }
}
