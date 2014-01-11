#region File Description
//-----------------------------------------------------------------------------
// SmokePlumeParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Particle3DSample
{
    /// <summary>
    /// Custom particle system for creating a giant plume of long lasting smoke.
    /// </summary>
    public class ShipSmokeParticleSystem : ParticleSystem
    {
        public ShipSmokeParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "smoke";

            settings.MaxParticles = 1600;

            settings.Duration = TimeSpan.FromSeconds(25);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 15;

            settings.MinVerticalVelocity = 150;
            settings.MaxVerticalVelocity = 250;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(-20, -15, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -3;
            settings.MaxRotateSpeed = 3;

            settings.MinStartSize = 224;
            settings.MaxStartSize = 337;

            settings.MinEndSize = 735;
            settings.MaxEndSize = 1470;
        }
    }
}
