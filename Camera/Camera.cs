//-----------------------------------------------------------------------------
// Camera Singleton that for now, doesn't do much.
//
// __Defense Sample for Game Programming Algorithms and Techniques
// Copyright (C) Sanjay Madhav. All rights reserved.
//
// Released under the Microsoft Permissive License.
// See LICENSE.txt for full details.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace itp380.Camera
{
    /// <summary>
    /// A basic free-camera class
    /// </summary>
    public class Camera : ICamera
    {
        Game m_Game;
        Vector3 m_vEye = new Vector3(0, 0, 10);
        Vector3 m_vTarget = Vector3.Zero;

        Matrix m_Camera;
        public Matrix CameraMatrix
        {
            get { return m_Camera; }
        }
        public Matrix GetCameraMatrix() {
            return this.m_Camera;
        }
        // The eye position
        public Vector3 Position { get; private set; }
        // the rotation values
        private float pitch, yaw;

        public Camera(Game game)
        {
            m_Game = game;
            ComputeMatrix();
            Position = new Vector3(0, 10, 0);
            pitch = 0;
            yaw = 0;
        }

        public void Update(float gameTime)
        {/*
            // query the devices
            MouseState ms = Mouse.GetState();
            KeyboardState kbs = Keyboard.GetState();

            // move based on how far off the mouse is from screen center
            yaw -= (ms.X - (Global.ScreenWidth / 2)) * 1.0f * gameTime;
            pitch -= (ms.Y - (Global.ScreenHeight / 2)) * 1.0f * gameTime;

            // reset the mouse to screen center
            Mouse.SetPosition(Global.ScreenWidth / 2, Global.ScreenHeight / 2);

            // generate the forward and right vectors
            // from the rotation values
            Vector3 fwd = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationZ(pitch) * Matrix.CreateRotationY(yaw));
            Vector3 rgt = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationZ(pitch) * Matrix.CreateRotationY(yaw));

            // move if the WASD keyboard keys are pressed
            Position += fwd * (kbs.IsKeyDown(Keys.W) ? 1 : kbs.IsKeyDown(Keys.S) ? -1 : 0) * 10 * gameTime;
            Position += rgt * (kbs.IsKeyDown(Keys.D) ? 1 : kbs.IsKeyDown(Keys.A) ? -1 : 0) * 10 * gameTime;
          * 
          * */
            this.ComputeMatrix();
        }

        /// <summary>
        /// Computes a view matrix for rendering
        /// </summary>
        /// <returns></returns>
        public Matrix GetViewMatrix()
        {
            Vector3 fwd = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationZ(pitch) * Matrix.CreateRotationY(yaw));
            return Matrix.CreateLookAt(Position, Position + fwd, Vector3.Up);
        }


        public void ComputeMatrix()
        {
            Vector3 vEye = m_vEye;
            Vector3 vTarget = m_vTarget;
            Vector3 vUp = Vector3.Cross(Vector3.Zero - vEye, Vector3.Left);
            m_Camera = Matrix.CreateLookAt(vEye, vTarget, vUp);
        }
    }
}