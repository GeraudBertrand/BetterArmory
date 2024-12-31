using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using static BetterArmory.Main;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Items.Tier2
{
    public class GiantGloves : ItemBase
    {
        public override string ItemName => "Giants Gloves";
        public override string ItemLangTokenName => "GIANTS_GLOVES";
        public override string ItemPickupDesc => "Giants always are stronger than you!";
        public override string ItemFullDescription => $"Increase your health by <style=cIsHealth>{GrantedHealth.Value}%</style> <style=cStack>(+{GrantedHealth.Value}% per stack)</style>. Boost your damage by <style=cIsHealth>3% of your <style=cIsHealth>max health.";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("GiantGlovesIcon.png");
        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("GiantGlovesDisplay.prefab");

        protected ConfigEntry<float> GrantedHealth;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            GrantedHealth = config.Bind("Item: " + ItemLangTokenName, "Percent of health granted by stack", 0.05f, "How much percent of health is granted");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            GetStatCoefficients += BonusHealthGloves;
            GetStatCoefficients += BonusDamageGloves;
        }

        private void BonusHealthGloves(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender && GetCount(sender) > 0)
            {
                args.healthMultAdd += GrantedHealth.Value * GetCount(sender);
            }
        }

        private void BonusDamageGloves(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender && GetCount(sender) > 0)
            {
                float mh = sender.healthComponent.fullHealth;
                args.baseDamageAdd += mh * 0.03f;
            }
        }
    }
}
