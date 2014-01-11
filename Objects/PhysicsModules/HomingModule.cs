using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects.PhysicsModules
{
    public abstract class HomingModule : Module
    {
        private PhysicsGameObject target;
        private float fwd_offset, up_offset;
        public HomingModule(PhysicsGameObject parent, PhysicsGameObject target)
            : base(parent)
        {
            this.target = target;
        }
        public void setFwdOffset(float offset)
        {
            this.fwd_offset = offset;
        }
        public void setUpOffset(float offset)
        {
            this.up_offset = offset;
        }
        public PhysicsGameObject Target { get { return this.target; } }
        public Vector3 TargetPosition { get { return this.target.Position + this.target.Up * this.up_offset + this.target.Forward * this.fwd_offset; } }
    }
}
