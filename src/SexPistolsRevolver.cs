using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.Magic_Wand
{
    internal class SexPistolsRevolver : Gun
    {
        bool shootEverReleased = false;
        public float rise = 0;
        public float _angleOffset = 0;
        public enum LoadState
        {
            NOT_LOADING,
            START_SWIPING_UP,
            SWIPING_UP,
            RELOADING,
            START_SWIPING_DOWN,
            SWIPING_DOWN
        };
        LoadState _loadState;

        public SexPistolsRevolver(float xval, float yval) : base(xval, yval) 
        {
            _manualLoad = true;
            _ammoType = new ATMagnum();
            ammoType.bulletSpeed = 16f;
            _type = "gun";
            graphic = new Sprite("magnum");
            center = new Vec2(16, 16);
            collisionOffset = new Vec2(-8f, -6f);
            collisionSize = new Vec2(16f, 10f);
            _barrelOffsetTL = new Vec2(25f, 12f);
            _fireSound = "magnum";
            _kickForce = 3f;
            _fireWait = 2f;
            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(1f, 2f);
            handOffset = new Vec2(0.0f, 1f);
            ammo = 6;
        }

        public override void Update()
        {
            base.Update();
            if (owner == null)
            {
                shootEverReleased = false;
            }
            if (owner != null && (owner is Duck) && !((Duck) owner).inputProfile.Pressed("SHOOT"))
            {
                shootEverReleased = true;
                if (_loadState == LoadState.NOT_LOADING)
                {
                    this._angleOffset = -(this.owner == null ? 0.0f : (this.offDir >= (sbyte)0 ? -Maths.DegToRad(this.rise * 65f) : -Maths.DegToRad((float)(-(double)this.rise * 65.0))));
                    if ((double)this.rise > 0.0)
                        this.rise -= 0.013f;
                    else
                        this.rise = 0.0f;
                    if (!this._raised)
                        return;
                    this._angleOffset = 0.0f;
                }
                UpdateLoadState();
            }
        }

        public void UpdateLoadState()
        {
            if (_loadState == LoadState.NOT_LOADING)
                return;
            if (this.owner == null)
            {
                if (this._loadState == LoadState.RELOADING)
                    this.loaded = true;
                this._loadState = LoadState.NOT_LOADING;
                this._angleOffset = 0.0f;
                this.handOffset = Vec2.Zero;
            }
            if (this._loadState == LoadState.START_SWIPING_UP)
            {
                if (Network.isActive)
                {
                    if (this.isServerForObject)
                        NetSoundEffect.Play("oldPistolSwipe");
                }
                else
                    SFX.Play("swipe", 0.6f, -0.3f);
                ++this._loadState;
            }
            else if (this._loadState == LoadState.SWIPING_UP)
            {
                if ((double)this._angleOffset < 0.15999999642372131)
                    this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.2f, 0.08f);
                else
                    ++this._loadState;
            }
            else if (this._loadState == LoadState.RELOADING)
            {
                this.handOffset.y -= 0.28f;
                if ((double)this.handOffset.y >= -4.0)
                    return;
                ++this._loadState;
                if (Network.isActive)
                {
                    if (!this.isServerForObject)
                        return;
                    NetSoundEffect.Play("oldPistolLoad");
                }
                else
                    SFX.Play("shotgunLoad");
            }
            else if (this._loadState == LoadState.START_SWIPING_DOWN)
            {
                this.handOffset.y += 0.15f;
                if ((double)this.handOffset.y < 0.0)
                    return;
                ++this._loadState;
                this.handOffset.y = 0.0f;
                if (Network.isActive)
                {
                    if (!this.isServerForObject)
                        return;
                    NetSoundEffect.Play("oldPistolSwipe2");
                }
                else
                    SFX.Play("swipe", 0.7f);
            }
            else
            {
                if (this._loadState != LoadState.SWIPING_DOWN)
                    return;
                if ((double)this._angleOffset > 0.039999999105930328)
                {
                    this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.0f, 0.08f);
                }
                else
                {
                    this._loadState = LoadState.NOT_LOADING;
                    this.loaded = true;
                    this._angleOffset = 0.0f;
                    if (this.isServerForObject && this.duck != null && this.duck.profile != null)
                        RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
                    if (Network.isActive)
                    {
                        if (!this.isServerForObject)
                            return;
                        SFX.PlaySynchronized("click", 1f, 0.5f, 0.0f, false, true);
                    }
                    else
                        SFX.Play("click", pitch: 0.5f);
                }
            }
        }

        public override void OnPressAction()
        {
            if (shootEverReleased)
            {
                if (loaded && ammo > 0)
                {
                    this._angleOffset = -(this.owner == null ? 0.0f : (this.offDir >= (sbyte)0 ? -Maths.DegToRad(this.rise * 65f) : -Maths.DegToRad((float)(-(double)this.rise * 65.0))));
                    angle -= _angleOffset;
                    base.OnPressAction();
                    angle += _angleOffset;
                    _angleOffset = 0;
                    if (rise < 1.0)
                    {
                        rise += 0.25f;
                    }
                    if (!loaded)
                    {
                        loaded = true;
                        --ammo;
                    }
                }
                else
                {
                    ammo = 6;
                    loaded = false;
                    if (_loadState == LoadState.NOT_LOADING)
                    {
                        _loadState = LoadState.START_SWIPING_UP;
                        _angleOffset = 0;
                        rise = 0;
                    }
                }
            }
        }

        public override void Draw()
        {
            float angle = this.angle;
            if (this.offDir > (sbyte)0)
                this.angle -= this._angleOffset;
            else
                this.angle -= this._angleOffset;
            base.Draw();
            this.angle = angle;
        }
    }
}
