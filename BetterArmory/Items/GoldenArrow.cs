using BepInEx.Configuration;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.IO;
using UnityEngine;

using static BetterArmory.Main;

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
            IL.RoR2.HealthComponent.TakeDamage += MultiCrit;
        }

        private void MultiCrit(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdloc(6),
                x => x.MatchLdcR4(2f)
                );
            c.Index += 1;
            c.Next.Operand = 2f + CritCoeff.Value ;

            c.EmitDelegate<Func<CharacterBody, float>>((cb) => 
            {
                if (cb.master.inventory)
                {
                    int ItemCount = GetCount(cb);
                    if(ItemCount > 0)
                    {
                        return 2f + CritCoeff.Value * ItemCount;
                    }
                }
                return 2f;
            });
        }
    }
}
