using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

using static R2API.RecalculateStatsAPI;
using static BetterArmory.Main;
using System;

namespace BetterArmory.Items
{
    class GoldenArrow : ItemBase
    {
        public override string ItemName => "Golden Arrow";
        public override string ItemLangTokenName => "GOLDEN_ARROW";
        public override string ItemPickupDesc => "Better arrow! So better damage!";
        public override string ItemFullDescription => "Let the gold imbue your weapon and become the edge of your bullet";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("assets/models/prefabs/item/firstitem/littleplate.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("assets/textures/icons/item/littleplate_icon.png");

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
            CritCoeff = config.Bind<float>("Item: "+ItemName,"Critical coefficient per stack",0.2f,"How much crit coefficient should item apply");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += doubleCrit;
        }

        private void doubleCrit(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if(damageInfo.rejected || damageInfo.procCoefficient <= 0)
            {
                orig(self, damageInfo, victim);
                return;
            }

            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var body = attacker.GetComponent<CharacterBody>();
                var victimBody = victim.GetComponent<CharacterBody>();
                if(body && victimBody)
                {
                    if (damageInfo.crit)
                    {
                        var inventoryCount = GetCount(body);
                        if(inventoryCount > 0)
                        {
                            damageInfo.damage = damageInfo.damage *  ( 1 + (CritCoeff.Value * inventoryCount));
                        }
                    }
                    
                }
            }
        }

    }
}
