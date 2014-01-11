using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects.PhysicsModules {
    public class LiftModule : Module {
        float coefficient;
        public LiftModule(PhysicsGameObject obj, float coefficient)
            : base(obj) {
            this.coefficient = coefficient;

        }
        public void setCoefficient(float coefficient)
        {
            this.coefficient = coefficient;
        }
        public override Microsoft.Xna.Framework.Vector3 tick(float fdelta) {
            Vector3 lift = this.parent.Up * this.parent.LinearVelocity.LengthSquared() * coefficient * fdelta;
            lift *= (float)Math.PI/2-(float)Math.Abs(Math.Asin(Vector3.Dot(Vector3.UnitY, this.parent.Forward)));
            this.parent.ApplyForce(ref lift);
            return lift;
        }
    }
}
