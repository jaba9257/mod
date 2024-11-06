using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    internal class StarPlatinum : Stand
    {
        private SpriteMap sprite;
        private float direct = 0;
        public StarPlatinum(float xpos, float ypos, float dir) : base(xpos, ypos)
        {
            this.sprite = new SpriteMap(GetPath("SPTW_punch2"), 32, 32);
            this.sprite.AddAnimation("SPTW_punch", 0.5f, true, new int[] { 0, 1, 2, 3, 4 });
            this.sprite.SetAnimation("SPTW_punch");
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
            if(lifetime == 0)
            {
                Level.Remove(this);
                base.Update();

                return;
            }
            float minn = 0f, pluss = 0f;
            if(direct == 1f)
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
                if(obj == this) {continue;}
                this.Hit(obj);
            }

            if(own.offDir > 0)
            {
                this.sprite.flipH = false;
                direct = 1f;
            }
            else if(own.offDir < 0)
            {
                this.sprite.flipH = true;
                direct = -1f;
            }

            if(direct == -1f)
            {
                this.collisionOffset = new Vec2(-30f, 0);
            }
            this.x = this.own.x + 12 * direct;
            this.y = this.own.y - 15;
            base.Update();
        }
    }
}
