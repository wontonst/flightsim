using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Graphics
{
    public class TutorialMessages : GameMessages
    {
        public static readonly Vector2 SCREEN_POSITION = new Vector2(30,100);
        int counter = 0;
        String[] tutorial_messages;
        float timeCounter = 0;
        public TutorialMessages()
        {
            String[] msgs ={ "Welcome to FlightSim!",
                               "The basic flight controls are simple.\nPitch is controlled by W and S.\n"+
                               "Roll is controlled by A and D.\n"+
                               "Yaw is controlled by Q and E.",
                               "Thrust is controlled by\nthe arrow up and arrow down keys.\n"+
                               "To begin your flight, \nthrust all the way up to \nengage the catapult.",
                               "Now that you're in flight, \nuse F1-F4 to switch between the \nvarious cameras.",
                               "Switch to the Cockpit View with F3.\nNext, press the spacebar to \nfire the forward cannon.",
                               "To fire a missile, press m. Missile are \nhoming, but have limited fuel and a \nlarge turning radius.",
                               "To complete the tutorial, fly back to\nthe carrier and land. The carrier \nis the blue dot on the radar."
                           };
            this.tutorial_messages = msgs;
            next();
        }
        public void next()
        {
            if(this.counter < 7)
            this.setMessage(this.tutorial_messages[this.counter]);
            this.counter++;
        }
        public void Update(float fdelta)
        {
            //System.Diagnostics.Debug.WriteLine(this.counter);
            if (this.counter == 3)
            {
                if (GameState.Get().Aircraft.Thrust == 100)
                {
                    next();
                    this.timeCounter = 0;
                }
                return;
            }
            this.timeCounter += fdelta;
            int waittime = this.counter == 1 ? 3 : 6;
            if (this.timeCounter >= waittime)
            {
                this.next();
                this.timeCounter = 0;
            }
        }
        bool won = false;
        public bool hasWon()
        {
            return this.won;
        }
    }
}
