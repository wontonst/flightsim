using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace itp380.UI
{
    public class UISecondMenu : UIScreen
    {
        SpriteFont m_TitleFont;
        SpriteFont m_ButtonFont;
        string m_Title;
        Texture2D m_Background;


        public UISecondMenu(ContentManager Content) :
            base(Content)
        {
            m_TitleFont = m_Content.Load<SpriteFont>("Fonts/MotorwerkLarge");
            m_ButtonFont = m_Content.Load<SpriteFont>("Fonts/MotorwerkOblique");
            m_Background = m_Content.Load<Texture2D>("airbase");

            // Create buttons
            Point vPos = new Point();
            vPos.X = (int)(GraphicsManager.Get().Width / 2.0f);
            vPos.Y = (int)(GraphicsManager.Get().Height / 2.0f);

            m_Title = "Flight Sim";

            m_Buttons.AddLast(new Button(vPos, "Tutorial",
                m_ButtonFont, Color.GreenYellow,
                Color.White, Tutorial, eButtonAlign.Center));

            vPos.Y += 50;

            m_Buttons.AddLast(new Button(vPos, "Mission I",
    m_ButtonFont, Color.GreenYellow,
    Color.White, Mission1, eButtonAlign.Center));

            vPos.Y += 50;

            m_Buttons.AddLast(new Button(vPos, "Mission II",
    m_ButtonFont, Color.GreenYellow,
    Color.White, Mission2, eButtonAlign.Center));

            vPos.Y += 50;
            m_Buttons.AddLast(new Button(vPos, "Exit",
                m_ButtonFont, Color.Red,
                Color.White, Exit, eButtonAlign.Center));
        }

        public void Tutorial()
        {
            SoundManager.Get().PlaySoundCue("MenuClick");
            GameState.Get().GameMode = eGameplayState.TUTORIAL;
            GameState.Get().SetState(eGameState.Gameplay);
        }

        public void Mission1()
        {
            SoundManager.Get().PlaySoundCue("MenuClick");
            GameState.Get().SetState(eGameState.Gameplay); GameState.Get().GameMode = eGameplayState.MISSION1;
        }

        public void Mission2()
        {
            SoundManager.Get().PlaySoundCue("MenuClick");
            GameState.Get().SetState(eGameState.Gameplay); GameState.Get().GameMode = eGameplayState.MISSION2;
        }

        private void DrawScenery(SpriteBatch s)
        {
            Rectangle screenRectangle = new Rectangle(0, 0, GraphicsManager.Get().Width, GraphicsManager.Get().Height);
            s.Draw(m_Background, screenRectangle, Color.White);
        }

        public void Options()
        {
        }

        public void Exit()
        {
            SoundManager.Get().PlaySoundCue("MenuClick");
            GameState.Get().Exit();
        }

        public override void Update(float fDeltaTime)
        {
            base.Update(fDeltaTime);
        }

        public override void Draw(float fDeltaTime, SpriteBatch DrawBatch)
        {
            DrawScenery(DrawBatch);
            Vector2 vOffset = Vector2.Zero;
            vOffset.Y = -1.0f * GraphicsManager.Get().Height / 4.0f;
            DrawCenteredString(DrawBatch, m_Title, m_TitleFont, Color.GreenYellow, vOffset);

            base.Draw(fDeltaTime, DrawBatch);
        }
    }
}
