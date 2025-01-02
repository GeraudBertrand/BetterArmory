using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using UnityEngine;

using static R2API.RecalculateStatsAPI;
using static BetterArmory.Main;
using R2API.Utils;
using BepInEx.Logging;
using BetterArmory.Utils;

namespace BetterArmory.Items.Tier2
{
    public class MidasContract : ItemBase
    {
        public override string ItemName => "Contract : Midas";
        public override string ItemLangTokenName => "MIDAS_CONTRACT";
        public override string ItemPickupDesc => "pickup";
        public override string ItemFullDescription => $"Killing spree increase gold gain multiplied by <style=cIsUtility>{GoldMultBase.Value*100}%</style> <style=cStack>(+{GoldMultStack.Value*100}% per stack)</style> for killing time";
        public override string ItemLore => "LORE";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("MidasContractDisplay.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MidasContractIcon.png");


        protected ConfigEntry<float> GoldMultBase;
        protected ConfigEntry<float> GoldMultStack;
        protected ConfigEntry<float> BuffTime;
        protected ConfigEntry<int> KillThreshold;

        public static BuffDef GoldBuff;


        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateBuffs();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            GoldMultBase = config.Bind<float>("Item: " + ItemLangTokenName, "Base gold multiplier for Midas Contract", 0.4f,"How much gold multiplier should the first give you ?");
            GoldMultStack = config.Bind<float>("Item: " + ItemLangTokenName, "Stack gold multiplier for Midas Contract", 0.3f, "How much gold multiplier should each stack give you ?");
            BuffTime = config.Bind<float>("Item: " + ItemLangTokenName, "Lifetime for Gold Buff", 15.0f,"How much time should the buff live on ?");
            KillThreshold = config.Bind<int>("Item: " + ItemLangTokenName, "Threshold to give Gold Buff", 5, "How much kill should player do to get Gold Buff ?");
        }
        public override void CreateBuffs()
        {
            GoldBuff = Buffs.AddNewBuff("GOLDMULT",MainAssets.LoadAsset<Sprite>("BloodRage.png"),Color.yellow);
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }


        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnCharacterDeath += GiveBuffOnThreshold;
            On.RoR2.CharacterMaster.GiveMoney += CharacterMaster_GiveMoney;
        }


        private void GiveBuffOnThreshold(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            CharacterBody body = damageReport.attackerBody;
            if (body != null && body.isPlayerControlled && GetCount(body) > 0)
            {
                if (body.multiKillCount >= KillThreshold.Value)
                {
                    if (!body.HasBuff(GoldBuff))
                    {
                        body.AddTimedBuff(GoldBuff,BuffTime.Value);
                    }
                }
            }
        }

        private void CharacterMaster_GiveMoney(On.RoR2.CharacterMaster.orig_GiveMoney orig, CharacterMaster self, uint amount)
        {
            if (self)
            {
                if (self.gameObject)
                {
                    CharacterBody body = self.GetBody();
                    if (body && GetCount(body) > 0) {
                        if (body.HasBuff(GoldBuff))
                        {
                            amount = (uint)(amount * (1 + GoldMultBase.Value + (GoldMultStack.Value * (GetCount(body) - 1))));
                        }
                    }
                }
            }
            orig(self, amount);
        }



    }
}
