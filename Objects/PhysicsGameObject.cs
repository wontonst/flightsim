using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Henge3D.Physics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Henge3D.Pipeline;

namespace itp380.Objects {
    public abstract class PhysicsGameObject : RigidBody {

        private Model _model;
        private Matrix[] _meshTransforms;

        //protected eDrawOrder m_DrawOrder = eDrawOrder.Default;
        //public eDrawOrder DrawOrder {
        //    get { return m_DrawOrder; }
        //}
    
        public abstract int getDrawOrder();

        // Anything that's timer logic is assumed to be affected by time factor
        protected Utils.Timer m_Timer = new Utils.Timer();

        protected PhysicsGameObject( Model m){
                setup(m);
        }
        public PhysicsGameObject( Model m, RigidBodyModel rgm)
            : base(rgm) {
                setup(m);

        }
        private void setup(Model m) {

            _model = m;
            _meshTransforms = new Matrix[_model.Bones.Count];

            _model.CopyAbsoluteBoneTransformsTo(_meshTransforms);
            _meshTransforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(_meshTransforms);
        }


        Vector3 m_vPos = Vector3.Zero;
        //public Vector3 Position {
        //    get { return m_vPos; }
        //    set { m_vPos = value; m_bTransformDirty = true; }
       // }
        
        public virtual Vector3 Up {
            get { return this.Transform.Combined.Up; }
        }

        public virtual Vector3 Forward {
            get {
                return this.Transform.Combined.Forward;
            }
        }
        public virtual Vector3 Left
        {
            get { return this.Transform.Combined.Left; }
        }

        public bool m_bEnabled = true;
        public bool Enabled {
            get { return m_bEnabled; }
            set { m_bEnabled = value; }
        }


        public virtual void Draw(float fDeltaTime) {

            foreach (ModelMesh mesh in _model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.World = _meshTransforms[mesh.ParentBone.Index] * Transform.Combined; // *m_WorldTransform;
                    effect.View = GameState.Get().CameraMatrix;
                    effect.Projection = GraphicsManager.Get().Projection;
                    effect.EnableDefaultLighting();
                    effect.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
                    effect.DirectionalLight0.Enabled = false;
                    effect.DirectionalLight1.Enabled = false;
                    effect.DirectionalLight2.Enabled = false;
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }
        public virtual void Unload() {

        }

        public virtual void Update(float fDeltaTime) {
            m_Timer.Update(fDeltaTime);
        }
    }
}
