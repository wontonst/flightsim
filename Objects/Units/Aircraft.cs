using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Henge3D.Pipeline;
using Henge3D.Physics;
using Henge3D;
using itp380.Objects.PhysicsModules;
using itp380.Graphics;

namespace itp380.Objects.Units
{


    public enum AircraftState
    {
        ON_DECK, CATAPULTING, IN_FLIGHT, STALLED
    }
    public class Aircraft : PhysicsGameObject
    {

        public static string MODEL_NAME = "Units/eurofighter4";

        private static readonly Vector3 STARTING_POSITION = new Vector3(00, 190, 00);
        private static readonly Quaternion STARTING_ORIENTATION = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI / 5);

        //Constants for WEAPONS
        public static readonly float MISSILE_BASE_DAMAGE = 30f;
        public static readonly int MAX_CANNON_AMMO = 500;
        public static readonly float MAX_HEALTH = 200f;
        public static readonly float MODULE_DESTROYED_THRESHOLD = 75F;
        public static readonly float MODULE_DAMAGED_THRESHOLD = 175F;
        //Constants for FUEL
        private static readonly float MAX_FUEL = 50000000;
        //Constants for relating fDeltaTime of key press to how much we adjust thrust, pitch..etc.
        private static readonly float
            PITCH_MULTIPLIER = 1500f,
              ROLL_MULTIPLIER = 1500f,
              YAW_MULTIPLIER = 250f;

        //Constants related to thrust
        private static readonly float THRUST_GAIN = 75f;//HOW FAST THRUST IS CHANGED! NOT THRUST MULTIPLIER!!!
        private static readonly float MAX_THRUST = 100f;
        private static readonly float THRUST_MULTIPLIER = 500;

        //Constants related to DRAG
        public static readonly float LINEAR_DRAG_COEFFICIENT = (float)(1.2 * .000005);
        public static readonly float ANGULAR_DRAG_COEFFICIENT = .0000015f;

        //Constants related to CATAPULT
        public static readonly float CATAPULT_LAUNCH_TIME = 2.5f;
        private static float CATAPULT_GAIN_FORWARD = 28000;
        private static float CATAPULT_GAIN_UP = 710;

        //Constants for STALL
        public static readonly float STALL_THRESHOLD = 650;
        public static readonly float STALL_GAIN = 8.5f;

        //Constants for MISSILES
        public static readonly float MISSILE_SIDE_OFFSET = 8;
        public static readonly float MISILE_TOP_OFFSET = 2;
        public static readonly float MISSILE_FRONT_OFFSET = 8;

        public static readonly float MISSILE_IGNITION_DELAY = 1.4f;

        //Constants related to LIFT
        public static readonly float LIFT_COEFFICIENT = 0.0022f;

        //Constants related to PITCH
        public static readonly float LINVEL_FORWARD_CONVERSION_RATE = .35f;

        //Constants related to DAMAGE
        public static readonly float YAW_DAMAGED_MULTIPLIER = .75f;
        public static readonly float ROLL_DAMAGED_MULTIPLIER = .75f;
        public static readonly float PITCH_DAMAGED_MULTIPLIER = .75f;
        public static readonly float DAMAGED_FUEL_CONSTANT_PERCENT_LOSS_PER_SECOND = .003f;
        public static readonly float OFFLINE_FUEL_CONSTANT_PERCENT_LOSS_PER_SECOND = .03f;
        public static readonly float ENGINE_DAMAGED_MULTIPLIER = .5f;
        public static readonly float ENGINE_OFFLINE_MULTIPLIER = .15f;



        //Physics Modules
        ThrustModule m_thrustModule;
        LiftModule m_liftModule;
        LinearDragModule m_linDragModule;
        AngularDragModule m_angDragModule;
        DamageModule m_dmgModule;
        PitchDirectionalModule m_pitchDirModule;

        float thrust = 0; //0 - 100 
        public float Thrust { get { return this.thrust; } }

        //For Combat and Firing
        public Vector3 vFireAt_Position = new Vector3();
        public float health = MAX_HEALTH;
        bool canFire = true;
        bool drawBullet = false;
        float fAccuracyCoefficient = 1.0f; //Determines  accuracy, 0 = perfect aim
        float fBulletRange = 20000.0f; //how far bullets will go
        float fRateOfFire = 0.1f; //1.0 = 1 bullet/sec, 0.5 = 2 bullets/sec ...etc.
        Random random = new Random();
        SpriteBatch m_SpriteBatch = new SpriteBatch(PhysicsGraphicsManager.Get().GraphicsDevice);

        public Vector3 LastPosition { get; set; }

        public Missile LeftMissile
        {
            get;
            set;
        }
        public Missile RightMissile { get; set; }

        public override int getDrawOrder()
        {
            return int.MaxValue;
        }
        public Aircraft()
            : base(GameState.Get().Game.Content.Load<Model>(MODEL_NAME))
        {
            initialize();
         //   this.m_thrustModule.setConstantFuelLoss(OFFLINE_FUEL_CONSTANT_PERCENT_LOSS_PER_SECOND);
        }
        public Aircraft(RigidBodyModel rbm)
            : base(GameState.Get().Game.Content.Load<Model>(MODEL_NAME), rbm)
        {
            initialize();

        }
        public bool Flaps { get; set; }
        public bool LGears { get; set; }
        private void initialize()
        {
            this.m_thrustModule = new ThrustModule(this, THRUST_MULTIPLIER * this.thrust, 1f, MAX_FUEL);//1f is fuel gain
            this.m_linDragModule = new LinearDragModule(this, LINEAR_DRAG_COEFFICIENT);
            this.m_liftModule = new LiftModule(this, LIFT_COEFFICIENT);
            this.m_angDragModule = new AngularDragModule(this, ANGULAR_DRAG_COEFFICIENT);
            this.m_dmgModule = new DamageModule(this, 0, 0, 0);
            this.m_pitchDirModule = new PitchDirectionalModule(this, LINVEL_FORWARD_CONVERSION_RATE);
            this.LastPosition = Vector3.Zero;
            this.Flaps = true;
            this.LGears = true;
            SetWorld(
                    STARTING_POSITION,
                    STARTING_ORIENTATION);//* Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI / 4));
            LinearVelocity = Vector3.Zero;
            AngularVelocity = Vector3.Zero;

            Capsule massCapsule = new Capsule(new Vector3(5, 0, 0), Vector3.Zero, 1);
            Vector3 center = new Vector3(2.5f, 0, 0);
            MassProperties mp = MassProperties.FromCapsule(0.15f, massCapsule.P1, massCapsule.P2, massCapsule.Radius, out center);
            this.MassProperties = mp;
            Capsule skinCapsule = new Capsule(new Vector3(25, 0, 0), new Vector3(-25, 0, 0), 15);
            this.Skin.Add(new CapsulePart(skinCapsule));


            //this.initialize
        }
        public void restockPlane()
        {

            if (this.LeftMissile == null)
            {
                AircraftMissile leftm = new AircraftMissile(GameState.Get().Physics, this, MissileState.ATTACHED_LEFT);
                GameState.Get().SpawnPhysicsGameObject(leftm);
                this.LeftMissile = leftm;
                GameState.Get().Physics.Remove(leftm);
                //leftm.Enabled = false;
            }
            if (this.RightMissile == null)
            {
                AircraftMissile rightm = new AircraftMissile(GameState.Get().Physics, this, MissileState.ATTACHED_RIGHT);
                GameState.Get().SpawnPhysicsGameObject(rightm);
                this.RightMissile = rightm;
                GameState.Get().Physics.Remove(rightm);
                //rightm.Enabled = false;
            }
            this.Flaps = true;
            this.LGears = true;
            this.health = MAX_HEALTH;
            HUD.Get().Aileron = HUD.DamageState.Online;
            HUD.Get().Rudder = HUD.DamageState.Online;
            HUD.Get().Elevator = HUD.DamageState.Online;
            HUD.Get().Wing = HUD.DamageState.Online;
            HUD.Get().Engine = HUD.DamageState.Online;
            HUD.Get().Fuel = HUD.DamageState.Online;
            this.m_thrustModule.refuel();
            this.bullets = MAX_CANNON_AMMO;
        }
        public override Vector3 Left
        {
            get
            {
                return -1 * base.Left;
            }
        }
        public Vector3 Right
        {
            get
            {
                return -1 * this.Transform.Combined.Right;
            }
        }
        public override Vector3 Forward
        {
            get
            {
                return base.Forward * -1;
            }
        }
        /*
         public override Vector3 Up {
             get {
                 return base.Up;
             }
         }
         */
        public void fireMissile()
        {
            if (this.LeftMissile != null)
            {
                this.LeftMissile.fire(GameState.Get().getClosestEnemy());
                GameState.Get().addFriendlyMissile(this.LeftMissile);
                this.LeftMissile = null;
                return;
            }
            if (this.RightMissile != null)
            {
                this.RightMissile.fire(GameState.Get().getClosestEnemy());
                GameState.Get().addFriendlyMissile(this.RightMissile);
                this.RightMissile = null;
                return;
            }
        }

        AircraftState state = AircraftState.ON_DECK;
        public AircraftState State { get { return this.state; } set { this.state = value; } }
        public bool canLand()
        {
            if(this.LGears)
            if (this.Position.X < 720 && this.Position.X > 680)
                if (this.Position.Y < 210)
                    if (this.Position.Z < 720 && this.Position.Z > 680)
                        if(this.LinearVelocity.Length() < 400)
                        return true;
            return false;
        }
        float catapultTimer;
        public override void Update(float fDeltaTime)
        {
            if (this.Flaps)
            {
                this.m_linDragModule.setCoefficient(LINEAR_DRAG_COEFFICIENT * 1.5f);
                this.m_liftModule.setCoefficient(LIFT_COEFFICIENT * 1.5f);
            }
            else
            {
                this.m_linDragModule.setCoefficient(LINEAR_DRAG_COEFFICIENT);
                this.m_liftModule.setCoefficient(LIFT_COEFFICIENT);
            }
            this.LastPosition = this.Position;

            base.Update(fDeltaTime);

            Vector3 antigravity = GameState.GRAVITY * -1;
            Vector3 appliedLift = Vector3.Zero;
            Vector3 appliedThrust = Vector3.Zero;
            Vector3 appliedLinearDrag = Vector3.Zero;
            Vector3 appliedAngularDrag = Vector3.Zero;
            Vector3 appliedDamage = Vector3.Zero;
            Vector3 appliedPitchDirectional = Vector3.Zero;
            switch (this.state)
            {
                case AircraftState.ON_DECK:
                    this.SetWorld(STARTING_POSITION);
                        this.SetVelocity(Vector3.Zero, Vector3.Zero);
                    if (this.thrust == 100)
                    {
                        this.state = AircraftState.CATAPULTING;
                        this.catapultTimer = CATAPULT_LAUNCH_TIME;
                    }
                    break;
                case AircraftState.CATAPULTING:
                    this.catapultTimer -= fDeltaTime;
                    Vector3 launch = this.Forward * CATAPULT_GAIN_FORWARD * fDeltaTime + this.Up * CATAPULT_GAIN_UP * fDeltaTime;
                    this.ApplyForce(ref launch);
                    //System.Diagnostics.Debug.WriteLine(launch.Length()+"");
                    if (this.catapultTimer <= 0) this.state = AircraftState.IN_FLIGHT;
                    break;
                case AircraftState.STALLED:
                    Quaternion quat = Quaternion.CreateFromAxisAngle(this.Left, (float)Math.PI / 2) - this.Orientation;
                    Vector3 stallforce = new Vector3(quat.X, quat.Y, quat.Z);
                    stallforce = stallforce * STALL_GAIN * fDeltaTime;
                    this.ApplyTorque(ref stallforce);
                    if (!this.isStalled())
                    {
                        this.state = AircraftState.IN_FLIGHT;
                        HUD.Get().setStall(false);
                    }
                    break;
                case AircraftState.IN_FLIGHT:
                    appliedAngularDrag = this.m_angDragModule.tick(fDeltaTime);
                    appliedLinearDrag = this.m_linDragModule.tick(fDeltaTime);
                    appliedLift = this.m_liftModule.tick(fDeltaTime);
                    appliedPitchDirectional = this.m_pitchDirModule.tick(fDeltaTime);
                    if (this.isStalled())
                    {
                        this.state = AircraftState.STALLED;
                        HUD.Get().setStall(true);
                    }
                    break;
            }
            //System.Diagnostics.Debug.WriteLine(this.state);

            appliedThrust = this.m_thrustModule.tick(fDeltaTime);
            appliedDamage = this.m_dmgModule.tick(fDeltaTime);

            //this.ApplyForce(ref antigravity);
            HUD.Get().formatStrings(this.LinearVelocity.Length(),this.m_thrustModule.getFuelAsPercent());
            HUD.Get().setFuel(this.m_thrustModule.getFuelAsPercent());
            HUD.Get().setVelocity(this.LinearVelocity.Length());
            HUD.Get().setDebugMessage(String.Format(
                "X={0} Y={1} Z={2} VX: {7:F2} VY: {8:F2} VZ: {9:F2}\nLDrag: {3:F2} ADrag: {6:F2} Thrust: {4:F2} Lift: {5:F2}\nPitDir: {11:F2} Damage: {10:F2}",
                this.Position.X, this.Position.Y, this.Position.Z,
                appliedLinearDrag.Length(), appliedThrust.Length(), appliedLift.Length(), appliedAngularDrag.Length(),
                this.LinearVelocity.X, this.LinearVelocity.Y, this.LinearVelocity.Z, appliedDamage.Length(), appliedPitchDirectional.Length()));

        }
        private bool canManeuver()
        {
            return this.state != AircraftState.ON_DECK && this.state != AircraftState.CATAPULTING;
        }
        protected bool isStalled()
        {
            return this.LinearVelocity.Length() < STALL_THRESHOLD;
        }
        private void updateThrustModule()
        {
            if (HUD.Get().Engine == HUD.DamageState.Damaged)
            {
                this.m_thrustModule.setThrustGain(this.thrust * THRUST_MULTIPLIER * ENGINE_DAMAGED_MULTIPLIER);
            }
            else if (HUD.Get().Engine == HUD.DamageState.Offline)
            {
                this.m_thrustModule.setThrustGain(this.thrust * THRUST_MULTIPLIER * ENGINE_OFFLINE_MULTIPLIER);
            }
            else
                this.m_thrustModule.setThrustGain(this.thrust * THRUST_MULTIPLIER);
        }
        public void IncreaseThrust(float fDeltaTime)
        {
            this.thrust = thrust + THRUST_GAIN * fDeltaTime;
            if (this.thrust > MAX_THRUST)
                this.thrust = MAX_THRUST;
            HUD.Get().setThrust(this.thrust);
            this.updateThrustModule();
            //System.Diagnostics.Debug.WriteLine("THRUST: " + this.thrust);
            SoundManager.Get().AdjustEngineSound(this.thrust);
        }

        public void DecreaseThrust(float fDeltaTime)
        {
            this.thrust = thrust - (THRUST_GAIN * fDeltaTime);
            if (this.thrust < 0)
                this.thrust = 0f;
            HUD.Get().setThrust(this.thrust);
            this.updateThrustModule();
            SoundManager.Get().AdjustEngineSound(this.thrust);
        }

        public void PitchUp(float fDeltaTime) //fDeltaTime = time that key has been held down
        {
            if (!this.canManeuver()) return;
            if (HUD.Get().Elevator == HUD.DamageState.Offline) return;

            //Get angle
            float theta = fDeltaTime * PITCH_MULTIPLIER;

            Vector3 power = -1 * this.Left * theta;

            if (this.isStalled()) power = power * .1f;
            if (HUD.Get().Elevator == HUD.DamageState.Damaged)
                power *= PITCH_DAMAGED_MULTIPLIER;
            this.ApplyTorque(ref power);
        }

        public void PitchDown(float fDeltaTime)
        {
            if (!this.canManeuver()) return;
            if (HUD.Get().Elevator == HUD.DamageState.Offline) return;

            //Get angle
            float theta = fDeltaTime * PITCH_MULTIPLIER;

            Vector3 power = this.Left * theta;
            if (this.isStalled()) power = power * .1f;
            //this.ApplyImpulse(ref power, ref offset); no response
            //this.ApplyAngularFlashImpulse(ref power); //works linearly
            //  this.ApplyFlashImpulse(ref power, ref offset); no response
            if (HUD.Get().Elevator == HUD.DamageState.Damaged)
                power *= PITCH_DAMAGED_MULTIPLIER;
            this.ApplyTorque(ref power);
        }

        public void YawRight(float fDeltaTime)
        {
            if (!this.canManeuver()) return;
            if (HUD.Get().Rudder == HUD.DamageState.Offline) return;

            //Get angle
            float theta = fDeltaTime * YAW_MULTIPLIER;

            Vector3 power = -1 * this.Transform.Combined.Up * theta;

            if (HUD.Get().Rudder == HUD.DamageState.Damaged)
                power *= YAW_DAMAGED_MULTIPLIER;
            this.ApplyTorque(ref power);
        }

        public void YawLeft(float fDeltaTime)
        {
            if (!this.canManeuver()) return;
            if (HUD.Get().Rudder == HUD.DamageState.Offline) return;

            //Get angle
            float theta = fDeltaTime * YAW_MULTIPLIER;

            Vector3 power = this.Transform.Combined.Up * theta;

            if (HUD.Get().Rudder == HUD.DamageState.Damaged)
                power *= YAW_DAMAGED_MULTIPLIER;
            this.ApplyTorque(ref power);
        }

        public void RollLeft(float fDeltaTime) //counterclockwise from cockpit perspective
        {
            if (!this.canManeuver()) return;
            if (HUD.Get().Aileron == HUD.DamageState.Offline) return;

            //Get angle
            float theta = fDeltaTime * ROLL_MULTIPLIER;

            Vector3 power = -1 * this.Forward * theta;

            if (HUD.Get().Aileron == HUD.DamageState.Damaged)
                power *= ROLL_DAMAGED_MULTIPLIER;
            this.ApplyTorque(ref power);
        }

        public void RollRight(float fDeltaTime) //clockwise from cockpit perspective
        {
            if (!this.canManeuver()) return;
            if (HUD.Get().Aileron == HUD.DamageState.Offline) return;

            //Get angle
            float theta = fDeltaTime * ROLL_MULTIPLIER;

            Vector3 power = this.Forward * theta;

            if (HUD.Get().Aileron == HUD.DamageState.Damaged)
                power *= ROLL_DAMAGED_MULTIPLIER;
            this.ApplyTorque(ref power);
        }

        public override void Draw(float fDeltaTime)
        {
            //m_SpriteBatch.Begin();
            // {
            //    if (drawBullet)
            //    {
            //        PhysicsGraphicsManager.Get().DrawLine3D(m_SpriteBatch, 1.0f, Color.Yellow, this.Position, vFireAt_Position);
            //        m_Timer.AddTimer("Aircraft Bullet Animation", fRateOfFire, StopDrawingBullet, false);
            //    }
            // }
            // m_SpriteBatch.End();

            base.Draw(fDeltaTime);
        }

        public void TakeDamage(float amount)
        {
            health -= amount;
            //System.Diagnostics.Debug.WriteLine("Health: " + this.health);

            int value = random.Next(0, 30);
            if (value < amount || this.health < 90)
            {
                this.damageRandomModule();
                if (value < amount * 2 / 3)
                {
                    this.damageRandomModule();
                    if (value < amount / 3)
                    {
                        this.damageRandomModule();
                    }
                }
            }
        }
        public void damageRandomModule()
        {
            if (health < MODULE_DESTROYED_THRESHOLD)
            {
                if (HUD.Get().allComponentsDown()) return;
                int value = random.Next(5, 10);
                if (value == 5)
                {
                    this.m_dmgModule.modifyRoll(2500);
                    HUD.Get().Wing = itp380.Graphics.HUD.DamageState.Offline;
                }
                if (value == 6)
                {
                    HUD.Get().Elevator = itp380.Graphics.HUD.DamageState.Offline;
                }
                if (value == 7)
                {
                    HUD.Get().Rudder = itp380.Graphics.HUD.DamageState.Offline;
                }
                if (value == 8)
                {
                    HUD.Get().Aileron = itp380.Graphics.HUD.DamageState.Offline;
                }
                if (value == 9)
                {
                    HUD.Get().Fuel = itp380.Graphics.HUD.DamageState.Offline;
                    this.m_thrustModule.setConstantFuelLoss(OFFLINE_FUEL_CONSTANT_PERCENT_LOSS_PER_SECOND);
                }
                if (value == 10)
                {
                    HUD.Get().Engine = itp380.Graphics.HUD.DamageState.Offline;
                    this.updateThrustModule();
                }
            }
            else if (this.health < MODULE_DAMAGED_THRESHOLD)
            {
                if (HUD.Get().allComponentsDamaged()) return;
                int value = random.Next(5, 10);
                if (value == 5)
                {
                    if (HUD.Get().Wing != HUD.DamageState.Online) this.damageRandomModule();
                    this.m_dmgModule.modifyRoll(1200);
                    HUD.Get().Wing = itp380.Graphics.HUD.DamageState.Damaged;
                }
                if (value == 6)
                {
                    if (HUD.DamageState.Online != HUD.Get().Elevator) this.damageRandomModule();
                    HUD.Get().Elevator = itp380.Graphics.HUD.DamageState.Damaged;
                }
                if (value == 7)
                {
                    HUD.Get().Rudder = itp380.Graphics.HUD.DamageState.Damaged;
                }
                if (value == 8)
                {
                    HUD.Get().Aileron = itp380.Graphics.HUD.DamageState.Damaged;
                }
                if (value == 9)
                {
                    HUD.Get().Fuel = itp380.Graphics.HUD.DamageState.Damaged;
                    this.m_thrustModule.setConstantFuelLoss(DAMAGED_FUEL_CONSTANT_PERCENT_LOSS_PER_SECOND);
                }
                if (value == 10)
                {
                    HUD.Get().Engine = itp380.Graphics.HUD.DamageState.Damaged;
                    this.updateThrustModule();
                }
            }
        }

        public Segment fireBullet()
        {
            canFire = false;
            this.bullets--;
            //calculate innacuracy offset
            Vector3 vOffset = new Vector3((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f);
            vOffset *= fAccuracyCoefficient;

            //calculate position bullet will be fired at
            vFireAt_Position = this.Position;
            vFireAt_Position += this.Forward * fBulletRange;
            vFireAt_Position += vOffset;

            drawBullet = true;

            m_Timer.AddTimer("Aircraft Fire Cooldown", fRateOfFire, resetFire, false);
            Segment Ray = new Segment(this.Position, vFireAt_Position);

            SoundManager.Get().PlaySoundCue("PlaneMachineGun");

            return Ray;
        }

        public void resetFire()
        {
            canFire = true;
        }

        public void StopDrawingBullet()
        {
            drawBullet = false;
        }
        int bullets = MAX_CANNON_AMMO;
        public int Bullets { get { return this.bullets; } }
        public bool CanFire()
        {
            return canFire && this.bullets > 0;
        }
    }
}
