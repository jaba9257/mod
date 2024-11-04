using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    internal class Stand : Thing
    {
        public Duck own = null;
        private bool startfight = false;
        private Duck enemy = null;
        private Stand enemystand = null;
        private int fightpoints = 0;
        private int framesforbattle = -1;
        public int lifetime = 300;
        public Duck lastownn = null;
        private int cantfight = 0;
        
        public Stand(float xval, float yval) : base(xval, yval)
        {

        }

        public void Hit(Thing obj)
        {
            if (obj == null) return;
            if(this.own != null && this.own.moveLock == true)
            {
                return;
            }
            if(obj is Stand && !startfight && cantfight < 0)
            {
                startfight = true;
                framesforbattle = 300;
                own.moveLock = true;
                enemystand = ((Stand)obj);
                enemy = ((Stand)obj).own;
                lifetime += 300;
                own._hSpeed = 0;
                own._vSpeed = 0;
                own.gravMultiplier = 0f;
                return;
            }
            if(obj is Bullet)
            {
                
            }
            Duck duck = null;
            if(obj is Duck && ((Duck)obj) != this.own)
            {
                if (this.x <= obj.x)
                {
                    obj.ApplyForce(new Vec2(1f, -0.7f));
                }
                else
                {
                    obj.ApplyForce(new Vec2(-1f, -0.7f));
                }
                duck = (Duck)obj;
                ((Duck)obj).GoRagdoll();
            }
            if(obj is RagdollPart)
            {
                if (this.x <= obj.x)
                {
                    obj.ApplyForce(new Vec2(1f, -0.7f));
                }
                else
                {
                    obj.ApplyForce(new Vec2(-1f, -0.7f));
                }
                duck = ((RagdollPart)obj).duck;
            }
            if (duck != null)
            {
                QuackUtility.DoQuack(ref duck);
            }
        }

        public override void Update()
        {
            base.Update();
            lifetime--;
            framesforbattle--;
            cantfight--;
            if (this.own == null)
            {
                if(framesforbattle > 0)
                {
                    lastownn.moveLock = false;
                }
                startfight = false;
                enemy = null;
                enemystand = null;
                return;
            }
            lastownn = own;
            if (startfight && framesforbattle > 0 && own.inputProfile.Pressed("QUACK"))
            {
                fightpoints++;
            }
            if (framesforbattle <= 0 && enemy != null)
            {
                startfight = false;
                if(enemystand.fightpoints > fightpoints)
                {
                    if(this.x < enemy.x)
                    {
                        own.ApplyForce(new Vec2(-4f, 0f));
                    }
                    else
                    {
                        own.ApplyForce(new Vec2(4f, 0f));
                    }
                    fightpoints = 0;
                    own.GoRagdoll();
                }
                else
                {
                    enemystand.fightpoints = 0;
                }
                enemy = null;
                enemystand = null;
                own.moveLock = false;
                own.gravMultiplier = 1f;
                lifetime = 0;
                //cantfight = 5;
            }
        }
    }
}
