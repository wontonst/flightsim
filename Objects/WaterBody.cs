using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Henge3D.Physics;
using Microsoft.Xna.Framework;
using Henge3D;
using Microsoft.Xna.Framework.Graphics;

namespace itp380.Objects {
    public class WaterBody : RigidBody {

        public WaterBody() {
            this.Skin.Add(new PlanePart(new Vector3(0, -15, 0), Vector3.UnitY));

            this.SetWorld(Vector3.Zero, Quaternion.Identity);
        }
    }
}
