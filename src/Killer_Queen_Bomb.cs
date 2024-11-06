﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    internal class Killer_Queen_Bomb : Thing
    {
        public Thing connectedto;
        public Killer_Queen_Bomb(float xval, float yval, Thing con = null) : base(xval, yval)
        {
            connectedto = con;
        }

        public void Detonate()
        {
            if(connectedto == null)
            {
                Level.Remove(this);
            }
            else
            {
                Level.Remove(connectedto);
                Level.Add(new GrenadeExplosion(this.x, this.y));
                Level.Remove(this);
            }
        }

        public override void Update()
        {
            base.Update();
            if(connectedto == null)
            {
                return;
            }
            this.position = connectedto.position;
        }
    }
}