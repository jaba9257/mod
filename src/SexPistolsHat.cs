using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    [EditorGroup("Stands")]
    internal class SexPistolsHat : Hat
    {
        SexPistolsRevolver revolver = null;
        Duck lastOwner = null;
        bool giveNextFrame = false;
        

        public SexPistolsHat(float xpos, float ypos) : base(xpos, ypos)
        {
            _editorName = "Sex Pistols";
            graphic = pickupSprite = new Sprite(GetPath("jotaro2"));
            sprite = new SpriteMap("jotaro3", 32, 32);
        }
        
        public override void Update()
        {
            base.Update();
            if (equippedDuck == null)
            {
                if (lastOwner != null)
                {
                    lastOwner.Disarm(lastOwner);
                    revolver.position = new Vec2( -1000, -1000 );
                    lastOwner = null;
                }
                if (revolver != null)
                {
                    Level.Remove(revolver);
                }
                return;
            }
            if (revolver == null || equippedDuck.holdObject != revolver)
            {
                lastOwner = equippedDuck;
                if (equippedDuck.holdObject != null)
                {
                    equippedDuck.Disarm(equippedDuck);
                }
                if (revolver == null)
                {
                    revolver = new SexPistolsRevolver(position.x, position.y);
                }
                Level.Add(revolver);
                equippedDuck.GiveHoldable(revolver);
            }
        }
    }
}
