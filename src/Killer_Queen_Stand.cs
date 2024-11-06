using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    internal class Killer_Queen_Stand : Stand
    {
        private SpriteMap sprite;
        float direct = 1f;
        public Killer_Queen_Stand(float xval, float yval, float dir) : base(xval, yval)
        {
            this.sprite = new SpriteMap(GetPath("yoshikage2"), 32, 32);
            this.sprite.AddAnimation("yoshikage2", 0.5f, true, new int[] { 0, 1, 2, 3, 4 });
            this.sprite.SetAnimation("yoshikage2");
            base.graphic = this.sprite;
            direct = dir;
            if (dir == -1f)
            {
                sprite.flipH = true;

            }
            base.collisionCenter = this.position;
            base.collisionSize = new Vec2(30f, 20f);
        }

        public override void Update()
        {
            if (this.own == null)
            {
                Level.Remove(this);
                base.Update();
                return;
            }
            if (lifetime == 0)
            {
                Level.Remove(this);
                base.Update();
                return;
            }
            float minn = 0f, pluss = 0f;
            if (direct == 1f)
            {
                minn = -0f;
                pluss = 29f;
            }
            else if (direct == -1f)
            {
                minn = -29f;
                pluss = 0f;
            }
            foreach (Thing obj in Level.CheckRectAll<Thing>(new Vec2(x + minn, y), new Vec2(x + pluss, y + 20f)))
            {
                if (obj == this) { continue; }
                this.Hit(obj);
            }

            if (own.offDir > 0)
            {
                this.sprite.flipH = false;
                direct = 1f;
            }
            else if (own.offDir < 0)
            {
                this.sprite.flipH = true;
                direct = -1f;
            }

            if (direct == -1f)
            {
                this.collisionOffset = new Vec2(-30f, 0);
            }
            this.x = this.own.x + 12 * direct;
            this.y = this.own.y - 15;
            base.Update();
        }
    }
}
