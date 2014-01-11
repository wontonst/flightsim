//-----------------------------------------------------------------------------
// The main GameState Singleton. All actions that change the game state,
// as well as any global updates that happen during gameplay occur in here.
// Because of this, the file is relatively lengthy.
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
using Microsoft.Xna.Framework.Graphics;
using itp380.Objects.Units;
using itp380.Objects;
using itp380.Graphics;
using Henge3D.Pipeline;
using itp380.Camera;
using Henge3D;
using Henge3D.Physics;
using System.Threading;

namespace itp380
{
    public enum eGameState
    {
        None = 0,
        MainMenu,
        SecondMenu,
        Gameplay,
    }
    public enum eGameplayState
    {
        NONE,
        TUTORIAL,
        MISSION1,
        MISSION2
    }


    public class GameState : itp380.Patterns.Singleton<GameState>
    {

        public static readonly Vector3 GRAVITY = new Vector3(0, -19.6f, 0);
        public static readonly float ANGULAR_DAMPING = 0.10f;
        public static readonly float ANGULAR_LIMIT = 500;
        public static readonly float LINEAR_DAMPING = 1.0f;
        public static readonly float LINEAR_LIMIT = 20000;

        public static readonly int NUM_ENEMIES = 10;
        public static readonly float ENEMY_MAXBULLETDMG = 40.0f;
        public static readonly float AIRCRAFT_BULLETDMG = 5.0f;
        public static readonly float AIRCRAFT_DMGRADIUS = 5.0f;

        public static readonly float RADAR_REFRESH_RATE = .8f;
        public bool DebugMode { get; set; }
        bool fullscreenEnabled = false;
        Game m_Game;
        public Game Game
        {
            get { return m_Game; }
        }
        eGameState m_State;
        public eGameState State
        {
            get { return m_State; }
        }

        eGameplayState m_GameMode = eGameplayState.NONE;
        public eGameplayState GameMode
        {
            get;
            set;
        }
        Objects.Units.Aircraft m_Aircraft;
        public AircraftCarrier AircraftCarrier { get; set; }
        public Objects.Units.Aircraft Aircraft
        {
            get { return m_Aircraft; }
        }
        bool endgame = false;
        public bool handleAircraftCollision(RigidBody aircraft, RigidBody target)
        {
            //System.Diagnostics.Debug.WriteLine("Aircraft collide");
            if (target is EnemyBoat)
            {
                endgame = true;
                //System.Diagnostics.Debug.WriteLine(target.GetType());
            }
            return true;
        }

        //Enemies
        List<Objects.Units.EnemyBoat> m_Enemies = new List<EnemyBoat>();
        List<Missile> m_FriendlyMissiles = new List<Missile>();
        List<Missile> m_EnemyMissiles = new List<Missile>();

        public void addFriendlyMissile(Missile m)
        {
            m.OnCollision = new CollisionEventHandler(handleFriendlyToEnemyCollision);
            this.m_FriendlyMissiles.Add(m);
        }
        public bool handleFriendlyToEnemyCollision(RigidBody source, RigidBody target)
        {
            if (source is Missile && target is EnemyBoat)
            {
                Missile m = (Missile)source;
                EnemyBoat b = (EnemyBoat)target;
                //System.Diagnostics.Debug.WriteLine("Missile-Enemy ship collide!");
                this.ParticlesManager.effectExplosion(m.Position);
                b.takeDamage(m.Damage);
                if (this.Camera == this.m_MissileCamera) { this.currentCamera = this.m_FarFollowCamera; }
                this.m_MissileCamera = null;
                this.m_FriendlyMissiles.Remove(m);
                m.Enabled = false;
                return true;
            }
            return false;
        }
        public void addEnemyMissile(Missile m)
        {
            this.m_EnemyMissiles.Add(m);
        }

        public EnemyBoat getClosestEnemy()
        {
            EnemyBoat closest = null;
            float distance = float.MaxValue;
            foreach (EnemyBoat e in this.m_Enemies)
            {
                float d = (e.Position - m_Aircraft.Position).LengthSquared();
                if (d < distance)
                {
                    closest = e;
                    distance = d;
                }
            }
            return closest;
        }

        eGameState m_NextState;
        Stack<UI.UIScreen> m_UIStack;
        bool m_bPaused = false;
        public bool IsPaused
        {
            get { return m_bPaused; }
            set { m_bPaused = value; }
        }

        // Keeps track of all active game objects
        LinkedList<GameObject> m_GameObjects = new LinkedList<GameObject>();
        LinkedList<PhysicsGameObject> m_PhysicsGameObjects = new LinkedList<PhysicsGameObject>();

        bool aircraftFiring = false;

        // Cameras
        FollowCamera m_FollowCamera;
        CockpitCamera m_CockpitCamera;
        RearCamera m_RearCamera;
        FarFollowCamera m_FarFollowCamera;
        FollowCamera m_MissileCamera;

        public void setMissileCamera(FollowCamera cam)
        {
            this.m_MissileCamera = cam;
        }

        ICamera currentCamera;
        public ICamera Camera
        {
            get { return this.currentCamera; }
        }

        public Matrix CameraMatrix
        {
            get { return this.currentCamera.CameraMatrix; }
        }

        Henge3D.Physics.PhysicsManager m_PhysicsManager;
        public Henge3D.Physics.PhysicsManager Physics
        {
            get { return this.m_PhysicsManager; }
        }

        ParticleManager m_ParticleManager;
        public ParticleManager ParticlesManager
        {
            get { return this.m_ParticleManager; }
        }
        // Timer class for the global GameState
        Utils.Timer m_Timer = new Utils.Timer();

        UI.UIGameplay m_UIGameplay;

        public void Start(Game game)
        {
            this.DebugMode = false;
            m_Game = game;
            m_State = eGameState.None;
            m_UIStack = new Stack<UI.UIScreen>();
            m_PhysicsManager = new Henge3D.Physics.PhysicsManager(this.m_Game);
            m_PhysicsManager.Enabled = false;
            //SET GRAVITY HERE
            m_PhysicsManager.Gravity = GRAVITY;
            m_PhysicsManager.AngularDamping = ANGULAR_DAMPING;
            m_PhysicsManager.AngularLimit = ANGULAR_LIMIT;
            m_PhysicsManager.LinearDamping = LINEAR_DAMPING;
            m_PhysicsManager.LinearLimit = LINEAR_LIMIT;

        }

        public void SetState(eGameState NewState)
        {
            m_NextState = NewState;
        }

        private void HandleStateChange()
        {
            if (m_NextState == m_State)
                return;

            switch (m_NextState)
            {
                case eGameState.MainMenu:
                    m_UIStack.Clear();
                    m_UIGameplay = null;
                    m_Timer.RemoveAll();
                    m_UIStack.Push(new UI.UIMainMenu(m_Game.Content));
                    ClearGameObjects();
                    break;
                case eGameState.SecondMenu:
                    m_UIStack.Clear();
                    m_UIGameplay = null;
                    m_Timer.RemoveAll();
                    m_UIStack.Push(new UI.UISecondMenu(m_Game.Content));
                    ClearGameObjects();
                    break;
                case eGameState.Gameplay:
                    SetupGameplay();
                    break;
            }

            m_State = m_NextState;
        }

        protected void ClearGameObjects()
        {
            // Clear out any and all game objects
            foreach (GameObject o in m_GameObjects)
            {
                RemoveGameObject(o, false);
            }
            m_GameObjects.Clear();
        }

        public void SetupGameplay()
        {
            ClearGameObjects();
            m_UIStack.Clear();

            m_UIGameplay = new UI.UIGameplay(m_Game.Content);
            m_UIStack.Push(m_UIGameplay);

            m_bPaused = false;
            GraphicsManager.Get().ResetProjection();

            m_Timer.RemoveAll();


            // TODO: Add any gameplay setup here
            this.endgame = false;
            //setup water collision
            WaterBody b = new WaterBody();
            this.m_PhysicsManager.Add(b);

            //spawn player
            Aircraft p2;
            p2 = new Aircraft();
            this.SpawnPhysicsGameObject(p2);
            this.m_Aircraft = p2;
            this.m_Aircraft.OnCollision = handleAircraftCollision;

            this.m_Aircraft.restockPlane();

            //spawn carrier
            this.AircraftCarrier = new AircraftCarrier();
            this.SpawnPhysicsGameObject(this.AircraftCarrier);

            switch (this.GameMode)//spawn enemies
            {
                case eGameplayState.TUTORIAL:
                    break;
                case eGameplayState.MISSION1:
                    Vector3 missileTestTarget = m_Aircraft.Forward * 30200;
                    missileTestTarget.Y = 50;
                    this.spawnEnemy(missileTestTarget);
                    break;
                case eGameplayState.MISSION2:
                    this.spawnEnemyWave();
                    break;
            }

            //setup cameras
            m_FollowCamera = new FollowCamera(m_Game, p2); //follow aircraft with follow camera
            m_CockpitCamera = new CockpitCamera(m_Game, p2); //cockpit camera
            m_RearCamera = new RearCamera(m_Game, p2); //rear facing head on camera
            m_FarFollowCamera = new FarFollowCamera(m_Game, p2); // same as follow cam but farther away

            //Set default camera
            this.currentCamera = this.m_FollowCamera;


            World.Get().load();
            World.Get().Enabled = true; //draw skybox and ocean

            //System.Diagnostics.Debug.WriteLine("Angularlim: " + this.Physics.AngularLimit + ", Angular damping; " + this.Physics.AngularDamping + ", Linearlim: " + this.Physics.LinearLimit + ", Lineardamp: " + this.Physics.LinearDamping);

            this.m_ParticleManager = new ParticleManager();
            this.m_ParticleManager.addAircraft(this.m_Aircraft);
            this.m_ParticleManager.Enabled = true;

            this.Physics.Enabled = true;

            if (this.fullscreenEnabled)
            {
                PhysicsGraphicsManager.Get().ToggleFullScreen();
            }
            HUD.Get().startRadar();
            this.updateRadar();
            HUD.Get().Radar.Enable(true);
            HUD.Get().gd = PhysicsGraphicsManager.Get().GraphicsDevice;

            //Start Engine Sound
            SoundManager.Get().StartEngineSound();
            SoundManager.Get().AdjustEngineSound(0.0f);

        }
        public void spawnEnemyWave()
        {
            //spawn enemies in random locations
            Random rand = new Random();
            int spawnRadius = 50000; // distance from origin (0,0,0) that planes will be allowed to spawn from
            for (int i = 0; i < NUM_ENEMIES; i++)
            {
                Vector3 carrierOffset = this.m_Aircraft.Forward * rand.Next(60000, 150000);
                carrierOffset += this.m_Aircraft.Left * rand.Next(-31000, 31000);
                this.spawnEnemy(carrierOffset);
               // this.spawnEnemy(new Vector3(rand.Next(spawnRadius * 2) - spawnRadius, 50, rand.Next(spawnRadius * 2) - spawnRadius));
            }
        }
        public void spawnEnemy(Vector3 position)
        {
            EnemyBoat e = new EnemyBoat(position);
            this.SpawnPhysicsGameObject(e);
            m_Enemies.Add(e);
        }
        public void Update(float fDeltaTime)
        {
            HandleStateChange();

            switch (m_State)
            {
                case eGameState.MainMenu:
                    UpdateMainMenu(fDeltaTime);
                    break;
                case eGameState.Gameplay:
                    UpdateGameplay(fDeltaTime);
                    break;
            }

            foreach (UI.UIScreen u in m_UIStack)
            {
                u.Update(fDeltaTime);
            }
        }

        void UpdateMainMenu(float fDeltaTime)
        {

        }
        bool hak = true;
        void UpdateGameplay(float fDeltaTime)
        {
            if (IsPaused) return;

            if (hak)
            {
                hak = false;

                this.Aircraft.SetWorld(this.Aircraft.Position - this.Aircraft.Forward * 21000);
            }
            if (this.Aircraft.State == AircraftState.IN_FLIGHT)
            {
                if (this.Aircraft.canLand())
                {
                    if (this.GameMode == eGameplayState.TUTORIAL)
                    {

                        this.GameOver(true);
                        return;
                    }
                    //System.Diagnostics.Debug.WriteLine("LANDED");
                    this.Aircraft.State = AircraftState.CATAPULTING;
                    this.Aircraft.restockPlane();
                }
            }

            if (this.endgame) { this.GameOver(false); return; }
            switch (this.GameMode)
            {
                case eGameplayState.TUTORIAL:
                    PhysicsGraphicsManager.Get().TutorialMessages.Update(fDeltaTime);
                    break;
            }
            //Update cameras
            this.currentCamera.Update(fDeltaTime);
            //m_FollowCamera.Update(fDeltaTime);
            //m_CockpitCamera.Update(fDeltaTime);
            //m_RearCamera.Update(fDeltaTime);
            //m_FarFollowCamera.Update(fDeltaTime);

            // Update objects in the world
            // We have to make a temp copy in case the objects list changes
            LinkedList<GameObject> temp = new LinkedList<GameObject>(m_GameObjects);
            foreach (GameObject o in temp)
            {
                if (o.Enabled)
                {
                    o.Update(fDeltaTime);
                }
            }
            LinkedList<PhysicsGameObject> temp2 = new LinkedList<PhysicsGameObject>(m_PhysicsGameObjects);
            foreach (PhysicsGameObject o in temp2)
            {
                if (o.Enabled)
                {
                    o.Update(fDeltaTime);
                }
            }
            m_Timer.Update(fDeltaTime);

            this.m_ParticleManager.Update(fDeltaTime);

            // TODO: Any update code not for a specific game object should go here;

            //aircraft firing
            if (aircraftFiring)
            {
                if (m_Aircraft.CanFire())
                {
                    //Fire bullet (ray cast)
                    Segment bulletCast = m_Aircraft.fireBullet();

                    //dummy variables needed for passing into Intersect() method
                    float scalar = 0.0f;
                    Vector3 intersectionPoint = Vector3.Zero;


                    foreach (EnemyBoat b in m_Enemies)
                    {
                        if (b.health <= 0) continue;
                        if (b.Skin.Intersect(ref bulletCast, out scalar, out intersectionPoint))
                        {
                            this.ParticlesManager.effectFlakBurst(intersectionPoint);

                            //if bullet hits an enemy, deal damage to that enemy
                            b.takeDamage(AIRCRAFT_BULLETDMG);
                            if (b.health <= 0)
                            {
                                this.ParticlesManager.addDestroyedShipPosition(b);
                            }
                        }
                        else
                        {
                            this.ParticlesManager.effectFlakBurst(m_Aircraft.vFireAt_Position);
                        }
                    }
                }
            }

            //Enemy Boats Fire at Plane
            foreach (EnemyBoat e in m_Enemies)
            {
                if (e.CanFire())
                {
                    //cast ray along bullet path
                    Segment bulletCast = e.FireBullet();

                    float bulletDist = Vector3.Distance(m_Aircraft.Position, e.vFireAt_Position);
                    SoundManager.Get().PlaySoundCue("FlakExplosion");
                    if (bulletDist <= AIRCRAFT_DMGRADIUS)
                    {
                        SoundManager.Get().PlaySoundCue("BulletHittingPlane");
                        if (bulletDist != 0.0f)
                        {
                            m_Aircraft.TakeDamage(ENEMY_MAXBULLETDMG / bulletDist);
                        }
                        else
                        {
                            m_Aircraft.TakeDamage(ENEMY_MAXBULLETDMG);
                            
                        }
                    }

                }
            }

            //World.Get().Update(fDeltaTime);
            foreach (Missile m in this.m_FriendlyMissiles)
            {
                if (m.Enabled == false)
                {
                    this.RemovePhysicsGameObject(m);
                }
            }

            if (this.m_Aircraft.health <= 0.0f)
            {
                this.GameOver(false);
                SoundManager.Get().PlaySoundCue("PlaneCrash");
                return;
            }

            if (this.m_Aircraft.Position.Y < 5)
            {
                this.GameOver(false);
                SoundManager.Get().PlaySoundCue("PlaneCrash");
                return;
            }


            radar_up -= fDeltaTime;
            if (radar_up < 0)
            {
                this.updateRadar();
               //Thread t = new Thread(new ThreadStart(updateRadar));
               //t.Start();
            }
            if (this.GameMode != eGameplayState.TUTORIAL)
            {
                if (this.hasWon()) this.GameOver(true);
                return;
            }

        }
        float radar_up = RADAR_REFRESH_RATE;
        public void updateRadar()
        {
            radar_up = RADAR_REFRESH_RATE;
            Vector3[] v3temp = new Vector3[this.m_Enemies.Count + 1];
            v3temp[0] = this.AircraftCarrier.Position;
            for (int i = 0; i < m_Enemies.Count; i++)
            {
                v3temp[i + 1] = m_Enemies[i].Position;
            }
            float angle = (float)Math.Acos(Vector3.Dot(Aircraft.Forward, Vector3.UnitX));
            angle -= (float)Math.PI / 2;
            HUD.Get().Radar.Update(v3temp, 0, angle, this.Aircraft.Position);
        }
        private bool hasWon()
        {
            foreach (EnemyBoat eb in this.m_Enemies)
            {
                if (eb.health > 0) return false;
            }
            //System.Diagnostics.Debug.WriteLine("WIN!");
            return true;
        }
        public void SpawnGameObject(GameObject o)
        {
            o.Load();
            m_GameObjects.AddLast(o);
            GraphicsManager.Get().AddGameObject(o);
        }
        public void SpawnPhysicsGameObject(PhysicsGameObject o)
        {
            m_PhysicsGameObjects.AddLast(o);
            PhysicsGraphicsManager.Get().AddGameObject(o);
            this.m_PhysicsManager.Add(o);
        }

        public void RemoveGameObject(GameObject o, bool bRemoveFromList = true)
        {
            o.Enabled = false;
            o.Unload();
            GraphicsManager.Get().RemoveGameObject(o);
            if (bRemoveFromList)
            {
                m_GameObjects.Remove(o);
            }
        }
        public void RemovePhysicsGameObject(PhysicsGameObject o, bool bRemoveFromList = true)
        {
            o.Enabled = false;
            PhysicsGraphicsManager.Get().RemoveGameObject(o);
            if (bRemoveFromList)
            {
                this.m_PhysicsGameObjects.Remove(o);
            }
            this.m_PhysicsManager.Remove(o);
        }

        public void MouseClick(Point Position)
        {
            if (m_State == eGameState.Gameplay && !IsPaused)
            {
                // TODO: Respond to mouse clicks here
            }
        }

        // I'm the last person to get keyboard input, so don't need to remove
        public void KeyboardInput(SortedList<eBindings, BindInfo> binds, float fDeltaTime)
        {
            if (m_State == eGameState.Gameplay && !IsPaused)
            {
                // TODO: Add keyboard input handling for Gameplay


                if (binds.ContainsKey(eBindings.PITCH_UP))
                {
                    m_Aircraft.PitchUp(fDeltaTime);
                }
                if (binds.ContainsKey(eBindings.PITCH_DOWN))
                {
                    m_Aircraft.PitchDown(fDeltaTime);
                }

                if (binds.ContainsKey(eBindings.ROLL_RIGHT))
                {
                    m_Aircraft.RollRight(fDeltaTime);
                }

                if (binds.ContainsKey(eBindings.ROLL_LEFT))
                {
                    m_Aircraft.RollLeft(fDeltaTime);
                }

                if (binds.ContainsKey(eBindings.YAW_RIGHT))
                {
                    m_Aircraft.YawRight(fDeltaTime);
                }

                if (binds.ContainsKey(eBindings.YAW_LEFT))
                {
                    m_Aircraft.YawLeft(fDeltaTime);
                }

                if (binds.ContainsKey(eBindings.THRUST_UP))
                {
                    m_Aircraft.IncreaseThrust(fDeltaTime);
                }

                if (binds.ContainsKey(eBindings.THRUST_DOWN))
                {
                    m_Aircraft.DecreaseThrust(fDeltaTime);
                }
                if (binds.ContainsKey(eBindings.START_FIREBULLET))
                {
                    //System.Diagnostics.Debug.WriteLine("start firing");
                    aircraftFiring = true;
                }
                if (binds.ContainsKey(eBindings.STOP_FIREBULLET))
                {
                    //System.Diagnostics.Debug.WriteLine("stop firing");
                    aircraftFiring = false;
                }
                if (binds.ContainsKey(eBindings.SEL_FOLLOWCAM))
                {
                    currentCamera = m_FollowCamera;
                }
                if (binds.ContainsKey(eBindings.SEL_COCKPITCAM))
                {
                    currentCamera = m_CockpitCamera;
                    World.Get().Update(0);
                }

                if (binds.ContainsKey(eBindings.SEL_REARCAM))
                {
                    currentCamera = m_RearCamera;
                }
                if (binds.ContainsKey(eBindings.SEL_FARFOLLOWCAM))
                {
                    currentCamera = m_FarFollowCamera;
                }
                if (binds.ContainsKey(eBindings.SEL_MISSILECAM))
                {
                    if (this.m_MissileCamera != null)
                    {
                        this.currentCamera = this.m_MissileCamera;
                        World.Get().Update(0);
                    }
                }
                if (binds.ContainsKey(eBindings.FIRE_MISSILE))
                {
                    this.m_Aircraft.fireMissile();
                }
                if (binds.ContainsKey(eBindings.TOGGLE_FLAPS))
                {
                    this.Aircraft.Flaps = !this.Aircraft.Flaps;
                }
                if (binds.ContainsKey(eBindings.TOGGLE_LANDING_GEARS))
                {
                    this.Aircraft.LGears = !this.Aircraft.LGears;
                }
                if (binds.ContainsKey(eBindings.TOGGLE_DEBUG_MODE)) this.DebugMode = !this.DebugMode;
            }


        }

        public UI.UIScreen GetCurrentUI()
        {
            return m_UIStack.Peek();
        }

        public int UICount
        {
            get { return m_UIStack.Count; }
        }

        // Has to be here because only this can access stack!
        public void DrawUI(float fDeltaTime, SpriteBatch batch)
        {
            // We draw in reverse so the items at the TOP of the stack are drawn after those on the bottom
            foreach (UI.UIScreen u in m_UIStack.Reverse())
            {
                u.Draw(fDeltaTime, batch);


            }

        }

        // Pops the current UI
        public void PopUI()
        {
            m_UIStack.Peek().OnExit();
            m_UIStack.Pop();
        }

        public void ShowPauseMenu()
        {
            IsPaused = true;
            m_UIStack.Push(new UI.UIPauseMenu(m_Game.Content));
        }

        public void Exit()
        {
            m_Game.Exit();
        }

        void GameOver(bool victorious)
        {
            IsPaused = true;
            SoundManager.Get().StopEngineSound();
            m_UIStack.Push(new UI.UIGameOver(m_Game.Content, victorious));
            if (this.fullscreenEnabled)
            {
                PhysicsGraphicsManager.Get().ToggleFullScreen();
            }
            this.m_ParticleManager.Enabled = false;
            while (this.m_PhysicsGameObjects.Count != 0)
            {
                this.RemovePhysicsGameObject(this.m_PhysicsGameObjects.First.Value);
            }
            this.m_FriendlyMissiles.Clear();
            this.m_EnemyMissiles.Clear();
            this.m_Enemies.Clear();
        }
    }
}
