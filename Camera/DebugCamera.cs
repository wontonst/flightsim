using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Camera {
    public class DebugCamera : ICamera {


        Vector3 vPosition = new Vector3(0, 0, 100),
                vTarget = Vector3.Zero,
                vUp = new Vector3(0, 0, 1);


        public Vector3 Position
        {
            get { return vPosition; }
        }

        Matrix m_DebugCamera;
        public Matrix CameraMatrix
        {
            get { return m_DebugCamera; }
        }
        public Matrix GetCameraMatrix()
        {
            return this.m_DebugCamera;
        }
        public DebugCamera(Game game) {
            ComputeMatrix();
        }

        public void Update(float fDeltaTime)
        {       
            ComputeMatrix();
        }

       public void ComputeMatrix()
        {
            //Create matrix
            m_DebugCamera = Matrix.CreateLookAt(vPosition, vTarget, vUp);           
        }
  


    }
}
