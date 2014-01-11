using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Henge3D;
using Henge3D.Physics;

namespace itp380.Objects.Units {
    public class AircraftCarrier : PhysicsGameObject {
        public override int getDrawOrder()
        {
            return 5;
        }
        public AircraftCarrier()
            : base(GameState.Get().Game.Content.Load<Model>("Units/carriernew")) {
            this.initialize();
        }
        private void initialize() {
            SetWorld(7f,
                 new Vector3(0, 2, 00),
                 Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI/5));
            LinearVelocity = Vector3.Zero;
            AngularVelocity = Vector3.Zero;

            Capsule capsule = new Capsule(new Vector3(10, 0, 0), Vector3.Zero, 5);
            Vector3 center = new Vector3(5, 0, 0);

            MassProperties mp = MassProperties.FromCapsule(100f, capsule.P1, capsule.P2, capsule.Radius, out center);
            this.MassProperties = mp;

          

           this.Skin.Add(new CapsulePart(capsule));
           //  this.Skin.Add(collide);
        }
    }
}
