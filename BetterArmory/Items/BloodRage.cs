using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

using static BetterArmory.Main;
using static BetterArmory.Utils.ItemHelpers;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Items
{
    public class BloodRage : ItemBase
    {
        public override string ItemName => "Blood Rage";
        public override string ItemLangTokenName => "BLOOD_RAGE";
        public override string ItemPickupDesc => " PICKUP DESCRIPTION";
        public override string ItemFullDescription => " FULL DESCRIPTION";
        public override string ItemLore => " LORE ";

        public override ItemTier Tier => ItemTier.Tier3;

        public ConfigEntry<float> BaseDamageGranted;
        public ConfigEntry<float> BasePercentToStack;
        public ConfigEntry<float> StackPercentToStack;

        public BuffDef DamageBuff { get; private set; }

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MyOrb.png");
        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("MyOrbDisplay.prefab");

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
            BaseDamageGranted = config.Bind<float>("Item: " + ItemLangTokenName, "Base damage granted by Blood Rage", 10f, "How much damage should the first gave you");
            BasePercentToStack = config.Bind<float>("Item: " + ItemLangTokenName, "Percent of health lost for stack", 0.2f, "How much percent of health do you need to lose to gain a stack");
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
            ContentAddition.AddBuffDef(DamageBuff);
        }

        public override void Hooks()
        {
            //Update buff on health
            On.RoR2.CharacterBody.FixedUpdate += CalculateDamage;
            //Update time buff on damage take
            On.RoR2.HealthComponent.TakeDamage += ResetTimer;
            //Add bonus damage on buff count
            GetStatCoefficients += AddDamageReward;
        }

        

        private void CalculateDamage(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
        {
            orig(self);
            var rageComponent = self.GetComponent<RageComponent>();
            if (!rageComponent) { 
                rageComponent = self.gameObject.AddComponent<RageComponent>();
                rageComponent.cachedHealth = self.healthComponent.health;
            }
            var newInventoryCount = GetCount(self);
            var actualLife = self.healthComponent.health;
            if(rageComponent.cachedInventoryCount != newInventoryCount)
            {
                rageComponent.cachedInventoryCount = newInventoryCount;
            }
            if (rageComponent.cachedInventoryCount > 0)
            {
                var stack = StackOfPercentLost(actualLife, self.healthComponent.fullHealth,rageComponent.cachedInventoryCount);
                rageComponent.cachedStack = stack;
                if (!self.HasBuff(DamageBuff.buffIndex))
                {
                    self.AddTimedBuffAuthority(DamageBuff.buffIndex,20f);
                }
                //self.SetBuffCount(DamageBuff.buffIndex, rageComponent.cachedStack);
                Chat.AddMessage(self.GetBuffCount(DamageBuff.buffIndex).ToString());
            }
            rageComponent.cachedHealth = actualLife;
        }

        private void ResetTimer(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self,damageInfo);

            var body = self.body;
            if (body)
            {
                if (body.HasBuff(DamageBuff.buffIndex))
                {
                    RefreshTimedBuffs(body,DamageBuff,20f);
                }
            }
        }

        private void AddDamageReward(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.HasBuff(DamageBuff)) { args.baseDamageAdd += BaseDamageGranted.Value * sender.GetBuffCount(DamageBuff); }
        }

        private int StackOfPercentLost(float actual, float full, int itemcount)
        {
            var actualPercent = Mathf.Round((actual*100)/ full);
            var lostPercent = 100 - actualPercent;
            var denom = 0.3f/(1+BasePercentToStack.Value*itemcount);
            int stack = (int)(lostPercent / denom);
            return stack;
        }

        public class RageComponent : MonoBehaviour
        {
            public int cachedInventoryCount = 0;
            public int cachedStack = 0;
            public float cachedHealth = 0;
        }
    }
}
