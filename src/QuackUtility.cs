using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    public static class QuackUtility
    {
        public static void DoQuack(ref Duck duck)
        {
            if (duck.dead || duck.inputProfile == null || Level.current == null)
                return;
            float leftTrigger = duck.inputProfile.leftTrigger;
            if (duck.inputProfile.hasMotionAxis)
                leftTrigger += duck.inputProfile.motionAxis;
            var equipment = duck.GetEquipment(typeof(Hat));
            if (Network.isActive)
                duck._netQuack.Play(pit: leftTrigger);
            else if (equipment != null && ((Hat) equipment).quacks)
                ((Hat) equipment).Quack(1f, leftTrigger);
            else 
                duck._netQuack.Play(pit: leftTrigger);
            if (duck.isServerForObject)
                ++Global.data.quacks.valueInt;
            ++duck.profile.stats.quacks;
            duck.quack = 20;
        }
    }
}
