using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BetterArmory.Buffs
{
    public abstract class BuffBase
    {
        public abstract string BuffName {  get; }
        public abstract bool CanStack {  get; }
        public abstract bool IsCooldown {  get; }
        public abstract bool IsDebuff {  get; }
        public abstract bool IsHidden {  get; }

        public abstract Color BuffColor { get; }

        public abstract Sprite IconSprite { get; }

        public BuffDef BuffDef;

        protected void CreateBuff()
        {
            BuffDef = ScriptableObject.CreateInstance<BuffDef>();
            BuffDef.name = "BUFF_"+BuffName;
            BuffDef.canStack = CanStack;
            BuffDef.isCooldown = IsCooldown;
            BuffDef.isDebuff = IsDebuff;
            BuffDef.isHidden = IsHidden;
            BuffDef.buffColor = BuffColor;
            BuffDef.iconSprite = IconSprite;

            ContentAddition.AddBuffDef(BuffDef);
        }
        public abstract void Init(ConfigFile config);
    }
}
