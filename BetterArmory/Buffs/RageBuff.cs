using BepInEx.Configuration;
using System;
using UnityEngine;

using static BetterArmory.Main;

namespace BetterArmory.Buffs
{
    public class RageBuff : BuffBase
    {
        public override string BuffName => "BLOOD_RAGE";

        public override bool CanStack => true;

        public override bool IsCooldown => false;

        public override bool IsDebuff => false;

        public override bool IsHidden => false;

        public override Color BuffColor => Color.red;

        public override Sprite IconSprite => MainAssets.LoadAsset<Sprite>("BloodRage.png");

        public override void Init(ConfigFile config)
        {
            CreateBuff();
        }
    }
}
