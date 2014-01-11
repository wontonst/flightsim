using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Camera
{
    public class FollowCamera : ICamera
    {
        
        // Enable/Disable Spring Camera
        bool ENABLE_SPRING = true;

        //Spring Cam Settings
        float DAMPEN_AMT = 1.0f; //Default 2.0f
        float STIFFNESS_AMT = 800.0f; //Default 128.0f
                                      //Higher numer = greater stiffness
        
        Game m_Game;
        GameState g = GameState.Get();
        Objects.PhysicsGameObject target;
        
        //Follow Distance
        float fHDist = 25.0f,
              fVDist = 5.0f;

        //Camera Data
        Vector3 vCamera_ActualPos,
                vCamera_IdealPos,
                vCamera_Velocity = Vector3.Zero;

        //Target Data
        Vector3 vTarget_Pos,
                vTarget_Forward,
                vTarget_Up;

    
        public Vector3 Position {
            get 
            {
                if (ENABLE_SPRING) return vCamera_ActualPos;
                else return vCamera_IdealPos;
            }
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

            //System.Diagnostics.Debug.WriteLine(target.Forward.X + "," + target.Forward.Y + "," + target.Forward.Z);

            //calculate cam positon
            vCamera_IdealPos = vTarget_Pos - (vTarget_Forward * fHDist) + (vTarget_Up * fVDist);
            if (ENABLE_SPRING) calculateSpringCamera(fDeltaTime);

            ComputeMatrix();
        }

        public void calculateSpringCamera(float fDeltaTime)
        {
            //Spring Camera Algorithm
            float fSpringConstant = STIFFNESS_AMT;
            float fDampConstant = DAMPEN_AMT * (float)Math.Sqrt((double)fSpringConstant);
            Vector3 vDisplacement = vCamera_ActualPos - vCamera_IdealPos;
            Vector3 vSpringAccel = (-fSpringConstant * vDisplacement) - (fDampConstant * vCamera_Velocity);
            
            vCamera_Velocity += vSpringAccel * fDeltaTime;
            vCamera_ActualPos += vCamera_Velocity * fDeltaTime;
        }

       public void ComputeMatrix()
        {
            Vector3 vCamera_Pos;
            if (ENABLE_SPRING) vCamera_Pos = vCamera_ActualPos;
            else vCamera_Pos = vCamera_IdealPos;
            
            Vector3 vCamera_Up, vCamera_Left, vCamera_Forward; 
            
            //Calculate vectors
            vCamera_Forward = vTarget_Pos - vCamera_Pos; 
            Vector3.Normalize(vCamera_Forward);

            vCamera_Left = Vector3.Cross(vTarget_Up, vCamera_Forward);
            vCamera_Up = Vector3.Cross(vCamera_Forward, vCamera_Left);

            //Create matrix
            m_FollowCamera = Matrix.CreateLookAt(vCamera_Pos, vTarget_Pos, vCamera_Up);

        }
    }
}
