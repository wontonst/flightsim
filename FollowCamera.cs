using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380
{
    public class FollowCamera : ICamera
    {
        Game m_Game;
        GameState g = GameState.Get();
        Objects.PhysicsGameObject target;
        
        //Follow Distance
        float fHDist = 00.3f,
              fVDist = 0.0f;

        //Camera Data
        Vector3 vCamera_ActualPos,
                vCamera_IdealPos,
                vCamera_Velocity = Vector3.Zero;

        Vector3 vTarget_Pos,
                vTarget_Forward,
                vTarget_Up;

        public Vector3 Position {
            get { return vCamera_IdealPos; }
        }

        Matrix m_FollowCamera;
        public Matrix CameraMatrix
        {
            get { return m_FollowCamera; }
        }
        public Matrix GetCameraMatrix() {
            return this.m_FollowCamera;
        }
        public FollowCamera(Game game, Objects.PhysicsGameObject target)
        {
            m_Game = game;
            this.target = target;
            ComputeMatrix();
        }

        public void Update(float fDeltaTime)
        {
            vTarget_Pos = target.Position;
            vTarget_Forward = target.Forward;
            vTarget_Up = target.Up;

            System.Diagnostics.Debug.WriteLine(target.Forward.X + "," + target.Forward.Y + "," + target.Forward.Z);

            //calculate cam positon
            vCamera_IdealPos = vTarget_Pos - vTarget_Forward * fHDist + vTarget_Up * fVDist;
            calculateSpringCamera(fDeltaTime);

            ComputeMatrix();
        }

        public void calculateSpringCamera(float fDeltaTime)
        {
            //Spring Camera Algorithm
            float fSpringConstant = 128.0f; //higher constant = stiffer spring
            float fDampConstant = 2.0f * (float)Math.Sqrt((double)fSpringConstant);
            Vector3 vDisplacement = vCamera_ActualPos - vCamera_IdealPos;
            Vector3 vSpringAccel = (-fSpringConstant * vDisplacement) - (fDampConstant * vCamera_Velocity);
            vCamera_Velocity += vSpringAccel * fDeltaTime;
            vCamera_ActualPos += vCamera_Velocity * fDeltaTime;
        }

       public void ComputeMatrix()
        {
            Vector3 vCamera_Pos = vCamera_IdealPos; //For basic follow cam, set vCamera_Pos = vCamera_IdealPos. For spring camera, set vCamera_Pos = vCamera_ActualPos
            Vector3 vCamera_Up, vCamera_Left, vCamera_Forward; 
            
            //Calculate vectors
            vCamera_Forward = vTarget_Pos - vCamera_Pos; 
            Vector3.Normalize(vCamera_Forward);

            vCamera_Left = Vector3.Cross(vTarget_Up, vCamera_Forward);
            vCamera_Up = Vector3.Cross(vCamera_Forward, vCamera_Left);
           // vCamera_Up = new Vector3(0, 0, 1);
           // vCamera_Left = Vector3.Cross(vCamera_Up, vCamera_Forward);

            //Create matrix
            m_FollowCamera = Matrix.CreateLookAt(vCamera_Pos, vTarget_Pos, vCamera_Up);           
        }
    }
}
