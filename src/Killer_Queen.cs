using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    [EditorGroup("Stands")]
    internal class Killer_Queen : Hat
    {
        Killer_Queen_Stand us = null;
        Killer_Queen_Bomb plantedBomb = null;
        int cooldown = 0;
        int punch_cooldown = 0;
        float direct = 1f;
        public Killer_Queen(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Killer Queen";
            graphic = pickupSprite = new Sprite(GetPath("yoshikage"));
            _sprite = new SpriteMap(GetPath("Killer_Queen"), 32, 32);
        }

        public void CreateKillerQueen(Killer_Queen u, out Killer_Queen_Stand us)
        {
            us = new Killer_Queen_Stand(u.x + u.direct * 8f, u.y, u.direct);
            us.own = u.equippedDuck;
            Level.Add(us);
        }

        public override void Update()
        {
            base.Update();
            cooldown--;
            punch_cooldown--;
            if(equippedDuck == null)
            {
                plantedBomb = null;
                return;
            }
            if(cooldown < 0 && equippedDuck.inputProfile.Pressed("QUACK") && plantedBomb == null)
            {
                Duck duck = equippedDuck;
                if(duck.holdObject != null && plantedBomb == null && !(holdObject is TrappedDuck))
                {
                    plantedBomb = new Killer_Queen_Bomb(duck.holdObject.x, duck.holdObject.y);
                    plantedBomb.connectedto = duck.holdObject;
                    Level.Add(plantedBomb);
                }
            }
            else if(plantedBomb != null && equippedDuck.inputProfile.Pressed("QUACK"))
            {
                plantedBomb.Detonate();
                plantedBomb = null;
                cooldown = 360;
            }
            if (!equippedDuck.moveLock && punch_cooldown < 0 && equippedDuck.inputProfile.Pressed("STRAFE"))
            {
                sprite = new SpriteMap(GetPath("jotaro2"), 32, 32);
                CreateKillerQueen(this, out us);
                punch_cooldown = 60 * 10;
                us.lifetime = 300;
            }

            if(equippedDuck != null &&  equippedDuck.offDir > 0)
            {
                direct = 1f;
            }
            else if(equippedDuck != null && equippedDuck.offDir < 0)
            {
                direct = -1f;
            }
        }
    }
}
