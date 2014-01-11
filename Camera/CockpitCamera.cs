using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Camera
{
    public class CockpitCamera : ICamera
    {
      
        Game m_Game;
        GameState g = GameState.Get();
        Objects.PhysicsGameObject target;

        //Camera Data
        Vector3 vCamera_Pos,
                vCamera_Velocity = Vector3.Zero;

        //Target Data
        Vector3 vTarget_Pos,
                vTarget_Forward,
                vTarget_Up;

    
        public Vector3 Position {
            get { return vCamera_Pos; }
        }

        Matrix m_CockpitCamera;
        public Matrix CameraMatrix
        {
            get { return m_CockpitCamera; }
        }
        public Matrix GetCameraMatrix() {
            return this.m_CockpitCamera;
        }
        public CockpitCamera(Game game, Objects.PhysicsGameObject target)
        {
            m_Game = game;
            this.target = target;
            ComputeMatrix();
        }

        public void Update(float fDeltaTime)
        {
            vTarget_Forward = target.Forward;
            vTarget_Up = target.Up;
            vTarget_Pos = target.Position;

            //System.Diagnostics.Debug.WriteLine(target.Forward.X + "," + target.Forward.Y + "," + target.Forward.Z);

            vTarget_Up.Normalize();
            vTarget_Forward.Normalize();

            //calculate cam positon
            vCamera_Pos = vTarget_Pos - (vTarget_Forward * -10.0f); //-10.0f offsets the camera so its just in front of the plane nose

            ComputeMatrix();
        }


       public void ComputeMatrix()
        {
            
            Vector3 vCamera_Up, vCamera_Left, vCamera_Forward, vLook_At; 
            
            //Calculate vectors
            vCamera_Forward = vTarget_Pos - vCamera_Pos; 
            Vector3.Normalize(vCamera_Forward);

            vCamera_Left = Vector3.Cross(vTarget_Up, vCamera_Forward);
            vCamera_Up = Vector3.Cross(vCamera_Forward, vCamera_Left);

            vLook_At = vTarget_Pos + vTarget_Forward * 100.0f;

            //Create matrix
            m_CockpitCamera = Matrix.CreateLookAt(vCamera_Pos, vLook_At, vCamera_Up);

        }
    }
}
