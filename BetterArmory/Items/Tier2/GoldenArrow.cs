using BepInEx.Configuration;
using IL.RoR2.Skills;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

using static BetterArmory.Main;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Items.Tier2
{
    public class GoldenArrow : ItemBase
    {
        public override string ItemName => "Golden Arrow";
        public override string ItemLangTokenName => "GOLDEN_ARROW";
        public override string ItemPickupDesc => "It never dulls, never bends, never fails. A symbol of absolute precision, it enhances the lethality of every critical strike.";
        public override string ItemFullDescription => $"Increase your critical damage by <style=cIsDamage>{CritCoeff.Value *100}%</style> <style=cStack>(+ {CritCoeff.Value *100}% per Stack)</style>";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("GoldenArrowDisplay.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("GoldenArrowIcon.png");

        protected ConfigEntry<float> CritCoeff;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            CritCoeff = config.Bind("Item: " + ItemLangTokenName, "Critical coefficient per stack", 0.2f, "How much crit coefficient should item apply");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            GetStatCoefficients += MultiCrit;
        }

        private void MultiCrit(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender && GetCount(sender) > 0)
            {
                var count = GetCount(sender);
                args.critDamageMultAdd += CritCoeff.Value * count;
            }


        }
    }
}
