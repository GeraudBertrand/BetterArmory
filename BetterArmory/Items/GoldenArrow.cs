using BepInEx.Configuration;
using IL.RoR2.Skills;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

using static BetterArmory.Main;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Items
{
    public class GoldenArrow : ItemBase
    {
        public override string ItemName => "Golden Arrow";
        public override string ItemLangTokenName => "GOLDEN_ARROW";
        public override string ItemPickupDesc => "Better arrows! So better damage!";
        public override string ItemFullDescription => "Let the gold imbue your weapon and become the edge of your bullet";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("assets/models/prefabs/item/puppet/puppet.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MyOrb.png");
        public ConfigEntry<float> CritCoeff;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            CritCoeff = config.Bind<float>("Item: "+ ItemLangTokenName, "Critical coefficient per stack",0.1f,"How much crit coefficient should item apply");
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
            if (sender)
            {
                var count = GetCount(sender);
                if(count > 0)
                {
                    args.critDamageMultAdd += CritCoeff.Value * (count - 1);
                }
            }
            
            
        }
    }
}
