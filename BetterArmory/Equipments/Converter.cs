using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using static BetterArmory.Main;

namespace BetterArmory.Equipment
{
    public class Converter : EquipmentBase
    {
        public override string EquipmentName => "Converter";
        public override string EquipmentLangTokenName => "CONVERTER";
        public override string EquipmentPickupDesc => "";
        public override string EquipmentFullDescription => "";
        public override string EquipmentLore => "";

        public override GameObject EquipmentModel => MainAssets.LoadAsset<GameObject>("assets/models/prefabs/item/firstitem/littleplate.prefab");
        public override Sprite EquipmentIcon => MainAssets.LoadAsset<Sprite>("assets/textures/icons/item/littleplate_icon.png");

        public ConfigEntry<float> ConverterRate;
        public ConfigEntry<float> ConverterCooldown;
        public override float Cooldown => ConverterCooldown.Value;

        public float exchange { get; private set; }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {
            ConverterCooldown = config.Bind<float>("Equipment : Converter","cooldown",50f,"how much cooldown");
            ConverterRate = config.Bind<float>("Equipment : Converter","rate",.2f,"how much rate");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (!slot.characterBody || !slot.characterBody.teamComponent) return false;
            var health = slot.characterBody.healthComponent;
            ChatMessage.Send("Converting...");
            if (health.fullHealth > 1f)
            {

                exchange = health.fullHealth * ConverterRate.Value;
                exchange = Mathf.Floor(exchange);

                RecalculateStatsAPI.GetStatCoefficients += ModifyHealthAndShield;
                return true;
            }
            else
            {
                ChatMessage.SendColored("Not enough health !",Color.red);
            }

            return false;
        }

        private void ModifyHealthAndShield(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            args.baseShieldAdd += exchange;
            args.baseHealthAdd -= exchange;
            ChatMessage.SendColored("It's done !!",Color.green);
        }
    }
}
