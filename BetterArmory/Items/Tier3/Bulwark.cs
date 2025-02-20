﻿using BepInEx.Configuration;
using BetterArmory.Utils;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

using static BetterArmory.Main;

namespace BetterArmory.Items.Tier3
{
    public class Bulwark : ItemBase
    {
        public override string ItemName => "Large Bulwark";
        public override string ItemLangTokenName => "LARGE_BULWARK";
        public override string ItemPickupDesc => "Forged in suffering, tempered in battle. A true bulwark never crumbles.";
        public override string ItemFullDescription => $"Resists damage by <style=cIsHealing>{baseReduction.Value * 100}%</style> <style=cStack>(+{stackReduction.Value * 100}% per stack).</style>";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier3;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("BulwarkDisplay.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("Bulwark.png");


        protected ConfigEntry<float> baseReduction;
        protected ConfigEntry<float> stackReduction;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            baseReduction = config.Bind("Item: " + ItemLangTokenName, "Base reduction for Bulwark", 0.11f, "How much reduction should the item give at first");
            stackReduction = config.Bind("Item: " + ItemLangTokenName, "Stack reduction for Bulwark", 0.2f, "How much reduction should the item gain by stack");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += ReduceDamage;
        }

        private void ReduceDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var body = self.body;
            if (body)
            {
                var itemCount = GetCount(body);
                if (itemCount > 0)
                {
                    if (damageInfo.damageType != DamageType.BypassArmor)
                    {
                        var reduction = Reduction(itemCount);
                        float bd = damageInfo.damage;
                        damageInfo.damage = Mathf.Max(1f, damageInfo.damage / (1 + reduction));
                    }
                }
            }
            orig(self, damageInfo);
        }

        private float Reduction(int count)
        {
            return MathForIt.RoundFloat(1 - 1 / (1 + baseReduction.Value + stackReduction.Value * (count - 1)), 2);
        }
    }
}
