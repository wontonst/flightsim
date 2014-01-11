//-----------------------------------------------------------------------------
// SoundManager maintains a list of cues and their corresponding files.
// This is a very bare bones way to play sound files.
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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace itp380
{
	public class SoundManager : Patterns.Singleton<SoundManager>
	{
		Dictionary<string, SoundEffect> m_Sounds = new Dictionary<string, SoundEffect>();

        SoundEffectInstance engineBurnSound;
        SoundEffect engineBurnSoundEffect;
        

        SoundEffectInstance engineTurbineSound;
        SoundEffect engineTurbineSoundEffect;
       
            


		public SoundManager()
		{
            
		}

		// Load the SFX
		public void LoadContent(ContentManager Content)
		{
			m_Sounds.Add("Shoot", Content.Load<SoundEffect>("Sounds/Shoot"));
			m_Sounds.Add("MenuClick", Content.Load<SoundEffect>("Sounds/MenuClick"));
			m_Sounds.Add("Build", Content.Load<SoundEffect>("Sounds/Build"));
			m_Sounds.Add("GameOver", Content.Load<SoundEffect>("Sounds/GameOver"));
			m_Sounds.Add("Victory", Content.Load<SoundEffect>("Sounds/Victory"));
			m_Sounds.Add("Error", Content.Load<SoundEffect>("Sounds/Error"));
			m_Sounds.Add("Snared", Content.Load<SoundEffect>("Sounds/Snared"));
			m_Sounds.Add("Alarm", Content.Load<SoundEffect>("Sounds/Alarm"));
            m_Sounds.Add("PlaneCrash", Content.Load<SoundEffect>("Sounds/PlaneCrashSound"));
            m_Sounds.Add("PlaneMachineGun", Content.Load<SoundEffect>("Sounds/PlaneMachineGunSound"));
            m_Sounds.Add("MissileLaunch", Content.Load<SoundEffect>("Sounds/MissileLaunchSound"));
            m_Sounds.Add("FlakExplosion", Content.Load<SoundEffect>("Sounds/FlakExplosionSound"));
            m_Sounds.Add("BulletHittingPlane", Content.Load<SoundEffect>("Sounds/BulletHittingPlaneSound"));
            // TODO: Add any additional sounds here

            //Load Engine Sounds
            engineBurnSoundEffect = Content.Load<SoundEffect>("Sounds/EngineBurnSound");
            engineBurnSound = engineBurnSoundEffect.CreateInstance();

            engineTurbineSoundEffect = Content.Load<SoundEffect>("Sounds/EngineTurbineSound");
            engineTurbineSound = engineTurbineSoundEffect.CreateInstance();

         
        }

		public void PlaySoundCue(string cue)
		{
			m_Sounds[cue].Play();
		}

        public void AdjustEngineSound(float rawThrust) //0 - 100
        {

            
            float thrust = rawThrust / 100; //normalized to 0-1
            float   turbineVol = 1.0f, 
                    turbinePitch = 0.0f;
            float   burnVol = 1.0f, 
                    burnPitch = 0.0f;


            //calculate turbine vol/pitch
            turbineVol = thrust;
            turbineVol *= 0.5f;
            turbinePitch = thrust * 2;
            turbinePitch -= 1.0f;
            

            //calculate burn vol/pitch
            burnVol = thrust + 0.1f;

            //pitch range: -1, 1
            //vol range: 0, 1
            if (burnVol > 1.0f) burnVol = 1.0f;
            if (burnVol < 0.0f) burnVol = 0.0f;
            if (turbineVol > 1.0f) turbineVol = 1.0f;
            if (turbineVol < 0.0f) turbineVol = 0.0f;
            if (burnPitch > 1.0f) burnPitch = 1.0f;
            if (burnPitch < -1.0f) burnPitch = -1.0f;
            if (turbinePitch > 1.0f) turbinePitch = 1.0f;
            if (turbinePitch < -1.0f) turbinePitch = -1.0f;
            
          
            engineBurnSound.Volume = burnVol;
            engineBurnSound.Pitch = burnPitch;
            engineTurbineSound.Volume = turbineVol;
            engineTurbineSound.Pitch = turbinePitch;
        }

        public void StartEngineSound()
        {
            if (!engineBurnSound.IsLooped) engineBurnSound.IsLooped = true;
            if (!engineTurbineSound.IsLooped) engineTurbineSound.IsLooped = true;
            
            engineTurbineSound.Play();
            engineBurnSound.Play();
        }

        public void StopEngineSound()
        {
            engineBurnSound.Stop();
            engineTurbineSound.Stop();
        }

        
	}
}
