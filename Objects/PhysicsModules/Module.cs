using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects.PhysicsModules {
    public abstract class Module {
        protected PhysicsGameObject parent;

        public Module(PhysicsGameObject obj) {
            this.parent = obj;
        }
        public abstract Vector3 tick(float fdelta);
    }
}
