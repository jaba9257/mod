using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    [EditorGroup("Stands")]
    internal class SexPistolsHat : Hat
    {
        int redirect_cooldown = 0;
        SexPistolsRevolver revolver = null;
        Duck lastOwner = null;
        bool giveNextFrame = false;
        

        public SexPistolsHat(float xpos, float ypos) : base(xpos, ypos)
        {
            _editorName = "Sex Pistols";
            graphic = pickupSprite = new Sprite(GetPath("mista"));
            _sprite = new SpriteMap(GetPath("mista_sex_pistols"), 32, 32);
        }

        public bool Check(Vec2 a, Vec2 b)
        {
            bool is_all_good = true;
            foreach (MaterialThing obj in Level.CheckLineAll<MaterialThing>(a, b))
            {
                if (obj is Duck && ((Duck)obj) == this.owner)
                {
                    is_all_good = false;
                    break;
                }
                if (obj.thickness <= 2f)
                {
                    continue;
                }
                is_all_good = false;
                break;
            }
            return is_all_good;
        }

        public bool RedirectBullet(SexPistolsHat u)
        {
            bool redirect = false;
            foreach(Bullet bul in Level.CheckCircleAll<Bullet>(u.position, 300f))
            {

                if(redirect)
                {
                    return redirect;
                }
                if (bul.owner == u.owner && !redirect)
                {
                    Vec2 pos = Basic_geometry.GetPoint(bul.start, bul._angle, bul.travelTime);
                    if(Basic_geometry.Dist(bul.travelStart, pos) >= Basic_geometry.Dist(bul.travelStart, bul.travelEnd) || !Check(pos, bul.travelStart))
                    {
                        continue;
                    }
                    foreach (Duck duck in Level.CheckCircleAll<Duck>(pos, 300f))
                    {
                        if(duck == u.owner)
                        {
                            continue;
                        }
                        if(redirect)
                        {
                            return redirect;
                        }
                        float per_need = 0f;
                        foreach (MaterialThing obj in Level.CheckLineAll<MaterialThing>(pos, duck.position))
                        {
                            if(obj is Duck && ((Duck)obj) == this.owner)
                            {
                                per_need = 10f;
                                break;
                            }
                            if(obj.thickness <= bul.ammo.penetration)
                            {
                                continue;
                            }
                            per_need += 1f;
                            break;
                        }
                        if(per_need == 0f)
                        {
                            redirect = true;
                            float angl_by_2p = Basic_geometry.CalculateAngle(pos, duck.position);
                            Bullet redirectional_bullet = new Bullet(pos.x, pos.y, bul.ammo, angl_by_2p);
                            Level.Remove(bul);
                            Level.Add(redirectional_bullet);
                        }
                    }
                }
            }
            return redirect;
        }
        
        public override void Update()
        {
            base.Update();
            redirect_cooldown--;
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
            if(redirect_cooldown < 0 && this.owner != null && (owner as Duck).inputProfile.Pressed("QUACK"))
            {
                bool wasredirected = RedirectBullet(this);
                if(wasredirected)
                {
                    redirect_cooldown = 600;
                }
            }
        }
    }
}
