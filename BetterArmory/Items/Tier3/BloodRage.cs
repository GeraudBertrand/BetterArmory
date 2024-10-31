using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

using static BetterArmory.Main;
using static BetterArmory.Utils.ItemHelpers;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Items.Tier3
{
    public class BloodRage : ItemBase
    {
        public override string ItemName => "Blood Rage";
        public override string ItemLangTokenName => "BLOOD_RAGE";
        public override string ItemPickupDesc => "Make your bleeding your strength. The damage inflicted on you will make your punches be the end.";
        public override string ItemFullDescription => $"For each <style=cIsHealth> 25%</style> <style=cStack>(- 10% per stack)</style> health lost, you will gain <style=cIsDamage>{BaseDamageGranted.Value} base damage</style>.";
        public override string ItemLore => " LORE ";

        public override ItemTier Tier => ItemTier.Tier3;

        public ConfigEntry<float> BaseDamageGranted;
        public ConfigEntry<float> PercentToStack;

        public BuffDef DamageBuff { get; private set; }

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("BloodRage.png");
        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("Magatama.prefab");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateBuff();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            BaseDamageGranted = config.Bind("Item: " + ItemLangTokenName, "Base damage granted by Blood Rage", 10f, "How much damage should the first gave you");
            PercentToStack = config.Bind("Item: " + ItemLangTokenName, "Percent of health lost for stack", 0.2f, "How much percent of health do you need to lose to gain a stack");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        private void CreateBuff()
        {
            DamageBuff = ScriptableObject.CreateInstance<BuffDef>();
            DamageBuff.name = "BUFF_Blood_Rage";
            DamageBuff.canStack = true;
            DamageBuff.isDebuff = false;
            DamageBuff.isHidden = false;
            DamageBuff.isCooldown = false;
            DamageBuff.buffColor = Color.red;
            DamageBuff.iconSprite = MainAssets.LoadAsset<Sprite>("BloodRage.png");
            ContentAddition.AddBuffDef(DamageBuff);
        }

        public override void Hooks()
        {
            //Update stack of buff on update
            On.RoR2.CharacterBody.FixedUpdate += UpdateNbBuff;
            //Add bonus damage on buff count
            GetStatCoefficients += AddDamageReward;
        }


        private void UpdateNbBuff(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            orig(self);
            var healthComp = self.healthComponent;
            // Should make the nb of buff update want hp change.
            if (self && GetCount(self) > 0)
            {
                int countStack = StackOfPercentLost(healthComp.health, healthComp.fullHealth, GetCount(self));
                self.SetBuffCount(DamageBuff.buffIndex, countStack);
            }
        }

        private void AddDamageReward(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.HasBuff(DamageBuff))
            {
                args.baseDamageAdd += BaseDamageGranted.Value * sender.GetBuffCount(DamageBuff);
            }
        }

        private int StackOfPercentLost(float actual, float full, int itemcount)
        {
            var actualPercent = Mathf.Round(actual / full * 100);
            var lostPercent = 100 - actualPercent;
            var denom = 0.3f / (1 + PercentToStack.Value * itemcount) * 100; // Make denom be on 100%
            int stack = Mathf.FloorToInt(lostPercent / denom); // ex:   51.2% % 25% = 2
            return stack;
        }
    }
}
