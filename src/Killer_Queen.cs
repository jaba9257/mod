using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DuckGame.Magic_Wand
{
    [EditorGroup("Stands")]
    internal class Killer_Queen : Hat
    {
        bool first = true;
        Killer_Queen_Stand us = null;
        Killer_Queen_Bomb plantedBomb = null;
        int cooldown = 0;
        int punch_cooldown = 0;
        float direct = 1f;
        public Killer_Queen(float xval, float yval) : base(xval, yval)
        {
            first = true;
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
                first = true;
                plantedBomb = null;
                return;
            }
            if (first)
            {
                SFX.Play(GetPath("Sounds/KillerQueen_Spawn"), 10f);
                first = false;
                return;
            }
            if (cooldown < 0 && equippedDuck.inputProfile.Pressed("QUACK") && plantedBomb == null)
            {
                Duck duck = equippedDuck;
                if(duck.holdObject != null && plantedBomb == null && !(duck.holdObject is TrappedDuck))
                {
                    /*if(duck.holdObject.GetType() != null)
                    {
                        File.WriteAllText(@"C:\Users\kv-2-\OneDrive\Документы\DuckGame\Mods\Magic_Wand\content\output.txt", duck.holdObject.GetType().ToString());
                    }*/
                    plantedBomb = new Killer_Queen_Bomb(duck.holdObject.x, duck.holdObject.y);
                    plantedBomb.connectedto = duck.holdObject;
                    Level.Add(plantedBomb);
                    SFX.Play(GetPath("Sounds/BombPlant"), 10f);
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
                sprite = new SpriteMap(GetPath("yoshikage"), 32, 32);
                CreateKillerQueen(this, out us);
                punch_cooldown = 60 * 10;
                us.lifetime = 300;
            }

            if (us != null && us.lifetime < 0)
            {
                Level.Remove(us);
                us.own = null;
                _sprite = new SpriteMap(GetPath("Killer_Queen"), 32, 32);
            }

            if (equippedDuck != null &&  equippedDuck.offDir > 0)
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
