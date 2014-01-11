using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using itp380.Objects.Units;

namespace itp380.Graphics
{
    public class HUD : Patterns.Singleton<HUD>
    {
        public static readonly Vector2 THRUST_SCREEN_POSITION = new Vector2(0, 720);
        public static readonly Vector2 HEALTH_SCREEN_POSITION = new Vector2(100, 0);
        public static readonly Vector2 HEALTH_DIM = new Vector2(60, 20);
        public static readonly Vector2 FUEL_SCREEN_POSITION = new Vector2(500, 720);
        public static readonly Vector2 VELOCITY_SCREEN_POSITION = new Vector2(230, 720);
        public static readonly Vector2 AMMO_SCREEN_POSITION = new Vector2(300, 680);
        public static readonly Vector2 STALL_SCREEN_POSITION = new Vector2(560, 750);
        public static readonly Vector2 SYSTEM_DIAGNOSTIC_SCREEN_POSITION = new Vector2(540, 10);
        public static readonly Vector2 RADAR_COCKPIT_SCREEN_POSITION = new Vector2(70, 700);
        public static readonly Vector2 RADAR_THIRDPERSON_SCREEN_POSITION = new Vector2(900, 680);
        public int health = 100;
        public GraphicsDevice gd;
        /*ONSCREEN DRAWABLE INTEGERS*/
        float thrustInt = 0;
        private Objects.Units.Radar m_Radar;
        public Radar Radar { get { return this.m_Radar; } }
        public void setThrust(float i)
        {
            this.thrustInt = i;
            this.str_Thrust = String.Format("Thrust: {0:F2}", i);
        }
        Boolean stall = false;
        public void setStall(Boolean stall)
        {
            this.stall = stall;
        }
        string debugMessage = "";
        public void setDebugMessage(string msg)
        {
            this.debugMessage = msg;
        }
        float fuel = 100;
        public void setFuel(float f)
        {
            this.fuel = f;
        }
        float velocity = 0;
        public void setVelocity(float v)
        {
            this.velocity = v;
        }
        public enum DamageState
        {
            Online, Damaged, Offline, Stuck
        }
        DamageState dmg_elevator = DamageState.Online;
        DamageState dmg_rudder = DamageState.Online;
        DamageState dmg_aileron = DamageState.Online;
        DamageState dmg_wing = DamageState.Online;
        DamageState dmg_fuel = DamageState.Online;
        DamageState dmg_thrust = DamageState.Online;
        public DamageState Elevator { get { return this.dmg_elevator; } set { this.dmg_elevator = value; this.recalculateSysdiag(); } }
        public DamageState Rudder { get { return this.dmg_rudder; } set { this.dmg_rudder = value; this.recalculateSysdiag(); } }
        public DamageState Aileron { get { return this.dmg_aileron; } set { this.dmg_aileron = value; this.recalculateSysdiag(); } }
        public DamageState Wing { get { return this.dmg_wing; } set { this.dmg_wing = value; this.recalculateSysdiag(); } }
        public DamageState Fuel { get { return this.dmg_fuel; } set { this.dmg_fuel = value; this.recalculateSysdiag(); } }
        public DamageState Engine { get { return this.dmg_thrust; } set { this.dmg_thrust = value; this.recalculateSysdiag(); } }
        String sysdiagstr;
        private String recalculateHelper(String name, DamageState good)
        {
            return name + " Status: " + good + "\n";
        }
        public void recalculateSysdiag()
        {
            this.sysdiagstr = recalculateHelper("Elevator", this.Elevator) +
                recalculateHelper("Rudder", this.Rudder) +
                recalculateHelper("Aileron", this.Aileron) +
                recalculateHelper("Fuel", this.dmg_fuel) +
                recalculateHelper("Engine", this.dmg_thrust) +
                (this.Wing == DamageState.Damaged || this.Wing == DamageState.Offline ? "Wing " + this.Wing.ToString() : "");
        }
        public bool allComponentsDown()
        {
            return this.dmg_elevator == DamageState.Offline && dmg_rudder == DamageState.Offline
        && dmg_aileron == DamageState.Offline
        && dmg_wing == DamageState.Offline
        && dmg_fuel == DamageState.Offline
        && dmg_thrust == DamageState.Offline;
        }
        public bool allComponentsDamaged()
        {
            return this.dmg_elevator != DamageState.Online && dmg_rudder != DamageState.Online
        && dmg_aileron != DamageState.Online
        && dmg_wing != DamageState.Online
        && dmg_fuel != DamageState.Online
        && dmg_thrust != DamageState.Online;
        }
        public void startRadar()
        {
            this.m_Radar = new Radar(RADAR_COCKPIT_SCREEN_POSITION, PhysicsGraphicsManager.Get().BlankTexture, 10.5f, 8000, PhysicsGraphicsManager.Get().GraphicsDevice);
        }
        public HUD()
        {
            this.recalculateSysdiag();
        }
        public void Draw(float fDelta, SpriteBatch m_SpriteBatch, SpriteFont m_FPSFont, SpriteFont m_KeyFont, SpriteFont m_TextFont, Game m_Game)
        {
            if (GameState.Get().State != eGameState.Gameplay) return;











            drawGrayHud(m_SpriteBatch, m_Game);
            drawRadar(m_SpriteBatch, m_FPSFont, m_Game, RADAR_COCKPIT_SCREEN_POSITION);
            drawVelocity(m_SpriteBatch, m_KeyFont);
            drawAltitude(m_SpriteBatch, m_KeyFont);
            drawThrust(m_SpriteBatch, m_TextFont, m_Game);





            //draw stall
            if (stall == true)
            {
                m_SpriteBatch.DrawString(m_TextFont, "Stall", STALL_SCREEN_POSITION, Color.Red);
            }
            else
            {
                m_SpriteBatch.DrawString(m_TextFont, "Stall", STALL_SCREEN_POSITION, Color.White);
            }

            return;

            //Components

        }
        string str_Thrust ="Thrust:  0.00", str_Velocity="", str_Fuel="";
        public void formatStrings(float velocity, float fuel)
        {
            this.str_Velocity = String.Format("Velocity: {0:F2}", this.velocity);
            this.str_Fuel = String.Format("Fuel: {0:F2}", this.fuel); 
        }
        public void DrawTextHUD(SpriteBatch m_SpriteBatch, SpriteFont font, Game m_Game)
        {
            //draw thrust
           // string tHrust = String.Format("Thrust: {0:F2}", thrustInt);
            //m_SpriteBatch.DrawString(font, tHrust, THRUST_SCREEN_POSITION, Color.White);
            m_SpriteBatch.DrawString(font, str_Thrust, THRUST_SCREEN_POSITION, Color.White);
            //draw velocity
           // string sVelocity = String.Format("Velocity: {0:F2}", this.velocity);
            //m_SpriteBatch.DrawString(font, sVelocity, VELOCITY_SCREEN_POSITION, Color.White);
            m_SpriteBatch.DrawString(font, str_Velocity, VELOCITY_SCREEN_POSITION, Color.White);
            //draw fuel
           // string sFuel = String.Format("Fuel: {0:F2}", this.fuel);
            //m_SpriteBatch.DrawString(font, sFuel, FUEL_SCREEN_POSITION, Color.White);
            m_SpriteBatch.DrawString(font, str_Fuel, FUEL_SCREEN_POSITION, Color.White);
            //draw sysdiag
            m_SpriteBatch.DrawString(font, String.Format("Ammo: " + GameState.Get().Aircraft.Bullets), AMMO_SCREEN_POSITION, Color.White);
            //Vector2 compStr = SYSTEM_DIAGNOSTIC_SCREEN_POSITION;
            //compStr.Y -= 50;

            //string compS = String.Format("Components");
            //m_SpriteBatch.DrawString(font, compS, compStr, Color.White);

            Vector2 flaps = new Vector2(700, 700);
            string temp= GameState.Get().Aircraft.Flaps ? "On" : "Off";
            
            string sFlaps = String.Format("Flaps: " + temp);

            m_SpriteBatch.DrawString(font, sFlaps, flaps, Color.White);



            Vector2 gears = new Vector2(700, 720);
            string temp2;
            if (GameState.Get().Aircraft.LGears == true)
            {
                temp2 = "On";
            }

            else
            {
                temp2 = "Off";
            }
            string sGears = String.Format("Gears: " + temp2);

            m_SpriteBatch.DrawString(font, sGears, gears, Color.White);

            m_SpriteBatch.DrawString(font, this.sysdiagstr, SYSTEM_DIAGNOSTIC_SCREEN_POSITION, Color.Red);

            this.drawRadar(m_SpriteBatch, font, m_Game, RADAR_THIRDPERSON_SCREEN_POSITION);

        }
        public void drawDebug(SpriteBatch m_SpriteBatch, SpriteFont font, Game m_Game)
        {

            Vector2 vDebugMessage = new Vector2(0, 550);
            m_SpriteBatch.DrawString(font, this.debugMessage, vDebugMessage, Color.Yellow);

        }

        public void drawGrayHud(SpriteBatch m_SpriteBatch, Game m_Game)
        {
            Color grayColor = new Color(220, 220, 220, 255);
            Rectangle hudRectangle = new Rectangle();
            Texture2D background = m_Game.Content.Load<Texture2D>("cockpit");
            hudRectangle.Width = 1020;
            hudRectangle.Height = 775;
            hudRectangle.X = 0;
            hudRectangle.Y = 0;
            m_SpriteBatch.Draw(background, hudRectangle, grayColor);
        }

        public void drawVelocity(SpriteBatch m_SpriteBatch, SpriteFont m_KeyFont)
        {
            Color blackColor = new Color(0, 0, 0, 255);
            Rectangle velRectangle = new Rectangle();
            Texture2D dummyTexture = new Texture2D(gd, 1, 1);
            dummyTexture.SetData(new Color[] { blackColor });
            velRectangle.Width = 50;
            velRectangle.Height = 20;
            velRectangle.X = 210;
            velRectangle.Y = 525;
            m_SpriteBatch.Draw(dummyTexture, velRectangle, blackColor);





            Vector2 velocityStr2 = new Vector2(197, 525);

            string velocityS2 = String.Format("" + (int)GameState.Get().Aircraft.LinearVelocity.Length());
            m_SpriteBatch.DrawString(m_KeyFont, velocityS2, velocityStr2, Color.White);
        }

        public void drawAltitude(SpriteBatch m_SpriteBatch, SpriteFont m_KeyFont)
        {
            Color blackColor = new Color(0, 0, 0, 255);
            Rectangle velRectangle = new Rectangle();
            Texture2D dummyTexture = new Texture2D(gd, 1, 1);
            dummyTexture.SetData(new Color[] { blackColor });
            velRectangle.Width = 50;
            velRectangle.Height = 20;
            velRectangle.X = 210;
            velRectangle.Y = 665;
            m_SpriteBatch.Draw(dummyTexture, velRectangle, blackColor);


            Vector2 altitudeStr = new Vector2(197, 665);

            string velocityS2 = String.Format(" " + (int)GameState.Get().Aircraft.Position.Y);
            m_SpriteBatch.DrawString(m_KeyFont, velocityS2, altitudeStr, Color.White);

        }


        Texture2D background = null;
        public void drawRadar(SpriteBatch m_SpriteBatch, SpriteFont m_FPSFont, Game m_Game, Vector2 pos)
        {
            if (this.background == null)
                this.background = GameState.Get().Game.Content.Load<Texture2D>("Radar");
            m_Radar.setPosition(pos);
            Color blackColor = new Color(169, 169, 169, 255);
            Rectangle velRectangle = new Rectangle();
            velRectangle.Width = 152;
            velRectangle.Height = 152;
            velRectangle.X = (int)(pos.X - 72);
            velRectangle.Y = (int)(pos.Y - 70);
            m_SpriteBatch.Draw(background, velRectangle, blackColor);
            Vector2 radarStr = new Vector2(35, 625);



            m_Radar.Draw(m_SpriteBatch);
        }

        public void drawThrust(SpriteBatch m_SpriteBatch, SpriteFont m_TextFont, Game m_Game)
        {
            float percent = thrustInt / 100;
            Color thrustColor = new Color(220, 20, 60, 255);
            Color backgroundColor = new Color(0, 0, 0, 128);
            Color outlineColor = new Color(255, 215, 0, 255);

            Vector2 thrustStr = new Vector2(132, 510);






            string tHrust = String.Format("Thr");
            m_SpriteBatch.DrawString(m_TextFont, tHrust, thrustStr, Color.White);

            Rectangle backgroundRectangle = new Rectangle();
            backgroundRectangle.Width = (int)HEALTH_DIM.Y;
            backgroundRectangle.Height = (int)HEALTH_DIM.X;
            backgroundRectangle.X = 145;
            backgroundRectangle.Y = 550;

            Texture2D dummyTexture = new Texture2D(gd, 1, 1);
            dummyTexture.SetData(new Color[] { backgroundColor });

            m_SpriteBatch.Draw(dummyTexture, backgroundRectangle, backgroundColor);



            backgroundRectangle.Width = (int)(HEALTH_DIM.Y * 0.9);
            backgroundRectangle.Height = (int)(percent * HEALTH_DIM.X);
            backgroundRectangle.X = (int)(145) + (int)(HEALTH_DIM.Y * 0.05);
            backgroundRectangle.Y = (int)(550);

            dummyTexture.SetData(new Color[] { thrustColor });

            m_SpriteBatch.Draw(dummyTexture, backgroundRectangle, thrustColor);



        }



    }
}

