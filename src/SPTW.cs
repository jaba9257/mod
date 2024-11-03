using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    [EditorGroup("Stands")]
    internal class SPTW : Hat
    {
        Duck LastEquipped = null;
        private float direct = 1f;
        private int cooldown = 0;
        private int ts_timer = 0;
        private bool unfreez = false;
        private bool first = true;
        private int punch_cooldown = 0;
        private int punch_time = 0;
        public StarPlatinum us = null;
        Dictionary<PhysicsObject, float[]> recover = new Dictionary<PhysicsObject, float[]>();
        List<Bullet> recbul = new List<Bullet>();
        public SPTW(float xpos, float ypos) : base(xpos, ypos)
        {
            _editorName = "SPTW";
            graphic = new Sprite(GetPath("jotaro2"));
            _pickupSprite = new Sprite(GetPath("jotaro2"));
            _sprite = new SpriteMap(GetPath("jotaro3"), 32, 32);
        }

        private static void freeztime(SPTW u, ref Dictionary<PhysicsObject, float[]> recover, ref List<Bullet> recbul)
        {
            foreach (PhysicsObject obj in Level.CheckCircleAll<PhysicsObject>(u.position, 10000000000f))
            {
                if (obj == u || (obj is Duck && u.equippedDuck == (Duck)obj))
                {
                    continue;
                }
                if (obj is Grenade)
                {
                    ((Grenade)obj)._timer += 0.01f;
                    ((Grenade)obj)._timer = Math.Min(1.2f, ((Grenade)obj)._timer);
                }
                if (obj is GoodBook)
                {
                    ((GoodBook)obj)._timer += 0.01f;
                    ((GoodBook)obj)._timer = Math.Min(1.2f, ((GoodBook)obj)._timer);
                }
                if (obj is GrenadeCannon)
                {
                    ((GrenadeCannon)obj)._timer += 0.01f;
                    ((GrenadeCannon)obj)._timer = Math.Min(1.2f, ((GrenadeCannon)obj)._timer);
                }
                if (obj is Mine)
                {
                    ((Mine)obj)._timer += 0.01f;
                    ((Mine)obj)._armed = false;
                    ((Mine)obj)._timer = Math.Min(1.2f, ((Mine)obj)._timer);
                }
                float[] val;
                if ((obj._vSpeed != 0 || obj._hSpeed != 0))
                {
                    if(obj is CannonGrenade)
                    {
                        if(obj._hSpeed != 0 || Math.Abs(obj._vSpeed) > 0.21f)
                        {
                            float[] zxc = new float[2];
                            zxc[0] = obj._hSpeed;
                            zxc[1] = obj._vSpeed;
                            if (recover.TryGetValue((obj), out val))
                            {
                                recover[obj] = zxc;
                            }
                            else
                            {
                                recover.Add(obj, zxc);
                            }
                        }
                        obj._hSpeed = 0;
                        obj._vSpeed = -0.2f;
                    }
                    else
                    {
                        float[] zxc = new float[2];
                        zxc[0] = obj._hSpeed;
                        zxc[1] = obj._vSpeed;
                        if (recover.TryGetValue((obj), out val))
                        {
                            recover[obj] = zxc;
                        }
                        else
                        {
                            recover.Add(obj, zxc);
                        }
                        obj._hSpeed = 0;
                        obj._vSpeed = 0;
                    }
                }
                obj.gravMultiplier = 0f;
                if (obj is Duck)
                {
                    ((Duck)obj).moveLock = true;
                }
            }
            foreach(Bullet bul in Level.CheckCircleAll<Bullet>(u.position, 1000000000f))
            {
                recbul.Add(new Bullet(bul.x, bul.y, bul.ammo, bul.angle, u.equippedDuck));
                recbul[recbul.Count - 1] = bul;
                Level.Remove(bul);
            }
            u.unfreez = true;
        }
        private static void unfreezt(SPTW u, ref Dictionary<PhysicsObject, float[]> recover, ref List<Bullet> recbul)
        {
            Layer.Blocks.colorMul = new Vec3(1f, 1f, 1f);
            Layer.Background.colorMul = new Vec3(1f, 1f, 1f);
            foreach (PhysicsObject obj in Level.CheckCircleAll<PhysicsObject>(u.position, 10000000000f))
            {
                if (u.equippedDuck != null && u.equippedDuck.moveLock)
                {
                    continue;
                }
                obj.gravMultiplier = 1f;
                float[] val;
                if (recover.TryGetValue((obj), out val))
                {
                    obj._vSpeed = val[1];
                    obj._hSpeed = val[0];
                }
                if (obj is Duck)
                {
                    ((Duck)obj).moveLock = false;
                }
            }
            foreach(Bullet val in recbul)
            {
                val.ammo.deadly = true;
                Level.Add(val);
            }
            recbul.Clear();
            recover.Clear();
            u.unfreez = false;
        }

        private static void CreateSPTW(SPTW u, out StarPlatinum us)
        {
            us = new StarPlatinum(u.x + u.direct * 8f, u.y, u.direct);
            us.own = u.equippedDuck;
            Level.Add(us);
        }

        public override void Update()
        {
            base.Update();
            ts_timer--;
            cooldown--;
            punch_time--;
            punch_cooldown--;
            if (LastEquipped == null && equippedDuck == null)
            {
                first = true;
                return;
            }
            if (equippedDuck == null)
            {
                if (us != null)
                {
                    us.own = null;
                    Level.Remove(us);
                }
                first = true;
                ts_timer = 0;
                if (ts_timer <= 0 && unfreez)
                {
                    SFX.Play(GetPath("Sounds/unfreez"), 10f);
                    unfreezt(this, ref recover, ref recbul);
                    unfreez = false;
                    LastEquipped = null;
                }
                if (!unfreez || ts_timer <= 0)
                {
                    LastEquipped = null;
                };
                return;
            }
            if (first && equippedDuck != null)
            {
                //recover.Clear();
                SFX.Play(GetPath("Sounds/SPTW_spawn"), 10f);
                first = false;
                if (cooldown < 100)
                {
                    cooldown = 100;
                }
                return;
            }
            if (!equippedDuck.moveLock && cooldown < 0 && equippedDuck.inputProfile.Pressed("QUACK"))
            {
                Layer.Blocks.colorMul = new Vec3(0.8f, 0f, 1f);
                Layer.Background.colorMul = new Vec3(0.8f, 0f, 1f);
                SFX.Play(GetPath("sounds/SPTW_TS"), 10f);
                cooldown = 60 * 30;
                ts_timer = 60 * 6;
            }
            if (!equippedDuck.moveLock && punch_cooldown < 0 && equippedDuck.inputProfile.Pressed("STRAFE"))
            {
                SFX.Play(GetPath("sounds/SPTW_ora"), 10f);
                sprite = new SpriteMap(GetPath("jotaro2"), 32, 32);
                CreateSPTW(this, out us);
                punch_cooldown = 60 * 10;
                punch_time = 60 * 5;
            }

            if (!equippedDuck.moveLock && equippedDuck.inputProfile.Pressed("RIGHT"))
            {
                direct = 1f;
            }

            if (!equippedDuck.moveLock && equippedDuck.inputProfile.Pressed("LEFT"))
            {
                direct = -1f;
            }

            if (us != null && us.lifetime < 0)
            {
                Level.Remove(us);
                us.own = null;
                _sprite = new SpriteMap(GetPath("jotaro3"), 32, 32);
            }

            if (ts_timer > 0)
            {
                freeztime(this, ref recover, ref recbul);
            }
            else if (ts_timer <= 0 && unfreez)
            {
                SFX.Play(GetPath("Sounds/unfreez"), 10f);
                unfreezt(this, ref recover, ref recbul);
                unfreez = false;
            }
        }
    }
}
