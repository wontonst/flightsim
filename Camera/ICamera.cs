using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Camera {
    public interface ICamera {
         void ComputeMatrix();
         Matrix CameraMatrix { get; }
         Vector3 Position { get; }
         void Update(float fDeltaTime);
    }
}
