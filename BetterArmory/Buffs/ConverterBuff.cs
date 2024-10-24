using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static BetterArmory.Main;

namespace BetterArmory.Buffs
{
    public class ConverterBuff : BuffBase
    {
        public override string BuffName => "HealthShieldConvertion";

        public override bool CanStack => false;

        public override bool IsCooldown => false;

        public override bool IsDebuff => false;

        public override bool IsHidden => true;

        public override Color BuffColor => Color.white;

        public override Sprite IconSprite => MainAssets.LoadAsset<Sprite>("MyOrb.png");

        public override void Init(ConfigFile config)
        {
            CreateBuff();
        }
    }
}
