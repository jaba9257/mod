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
        Killer_Queen_Bomb plantedBomb = null;
        int cooldown = 0;
        public Killer_Queen(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Killer Queen";
            graphic = pickupSprite = new Sprite(GetPath("yoshikage"));
            _sprite = new SpriteMap(GetPath("Killer_Queen"), 32, 32);
        }

        public override void Update()
        {
            base.Update();
            cooldown--;
            if(owner == null)
            {
                return;
            }
            if(cooldown < 0 && ((Duck)owner).inputProfile.Pressed("QUACK") && plantedBomb == null)
            {
                Duck duck = ((Duck)owner);
                if(duck.holdObject != null && plantedBomb == null)
                {
                    plantedBomb = new Killer_Queen_Bomb(duck.holdObject.x, duck.holdObject.y);
                    plantedBomb.connectedto = duck.holdObject;
                    Level.Add(plantedBomb);
                }
            }
            else if(plantedBomb != null && ((Duck)owner).inputProfile.Pressed("QUACK"))
            {
                plantedBomb.Detonate();
                plantedBomb = null;
                cooldown = 360;
            }
        }
    }
}
