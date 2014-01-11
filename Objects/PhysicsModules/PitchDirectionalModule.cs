using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace itp380.Objects.PhysicsModules
{
    /**
     * Performs drag and force application based off the angle of attack. Basically it converts linearvelocity force into forward force. I'm not an aeronautical engineer so that's just a fancy way of saying this is a hack.
     * @author RoyZheng
     */
   public class PitchDirectionalModule : Module
    {
       float coefficient;
       /**
        * @param conversion_percent percent of linear velocity to convert to forward
        */
       public PitchDirectionalModule(PhysicsGameObject parent, float conversion_percent) : base(parent)
       {
           this.coefficient = conversion_percent;
       }
       public override Microsoft.Xna.Framework.Vector3 tick(float fdelta)
       {
           Vector3 apply = -1 * this.parent.LinearVelocity * this.coefficient;
           apply += this.parent.Forward * apply.Length();
           this.parent.ApplyForce(ref apply);
           return apply;
       }
    }
}
