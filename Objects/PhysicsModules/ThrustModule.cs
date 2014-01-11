using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects.PhysicsModules {
    public class ThrustModule : Module {
        float thrust_gain, fuel_gain;
        double maxFuel, currentFuel, constantFuelLossPerSecond = 0;
        public ThrustModule(PhysicsGameObject obj, float thrust_gain, float fuel_gain, double max_fuel)
            : base(obj) {
            this.thrust_gain = thrust_gain;
            this.fuel_gain = fuel_gain;
            this.maxFuel = max_fuel;
            this.currentFuel = max_fuel;
        }
        public void setConstantFuelLoss(double loss)
        {
            this.constantFuelLossPerSecond = loss;
        }
        public void setThrustGain(float gain) {
            this.thrust_gain = gain;
        }
        public void setFuelGain(float gain)
        {
            this.fuel_gain = gain;
        }
        public void refuel(){
            this.currentFuel = maxFuel;
        }
        public override Vector3 tick(float fdelta) {
            this.currentFuel -= thrust_gain * fdelta * fuel_gain + constantFuelLossPerSecond * fdelta * this.maxFuel;
            //System.Diagnostics.Debug.WriteLine("constanloss : " + constantFuelLossPerSecond * fdelta * this.maxFuel);
          //  System.Diagnostics.Debug.WriteLine(gain * fdelta + "yay");
            if (this.currentFuel > 0) {
                Vector3 appliedthrust = this.parent.Forward * fdelta * thrust_gain;
                this.parent.ApplyForce(ref(appliedthrust));
                return appliedthrust;
            }
            else { this.currentFuel = 0; }
            return Vector3.Zero;
        }
        public float getFuelAsPercent() {
            return (float)(100*(this.currentFuel / this.maxFuel));
        }
    }
}
