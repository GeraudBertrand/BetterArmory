using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.IO;
using System.Reflection;
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
            CritCoeff = config.Bind<float>("Item: "+ItemName,"Critical coefficient per stack",2f,"How much crit coefficient should item apply");
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
            /*var c = new ILCursor(il);
            c.Index = 0;

            bool ILFound = c.TryGotoNext(
                x => x.MatchLdarg(1),
                x => x.MatchLdfld<DamageInfo>("crit"),
                x => x.MatchBrfalse(out _), 
                x => x.MatchLdloc(6),
                x => x.MatchLdcR4(2f),
                x => x.MatchMul(),
                x => x.MatchStloc(6)
                );
            if (ILFound)
            {
                c.Emit(OpCodes.Ldarg_0); // put health component on the stack
                c.Emit(OpCodes.Ldfld, typeof(HealthComponent).GetField("body", BindingFlags.Instance | BindingFlags.Public)); // Load the character body onto the stack

                c.EmitDelegate<Func<HealthComponent, float>>(hc =>
                {
                    var inv = hc.body.master.inventory;
                    if (inv)
                    {
                        var itemCount = GetCount(hc.body);
                        float val = 2f + (itemCount * CritCoeff.Value);
                        Debug.Log(itemCount+" : "+val);
                        return val;
                    }
                    return 2f;
                });
            }*/
        }
    }
}
