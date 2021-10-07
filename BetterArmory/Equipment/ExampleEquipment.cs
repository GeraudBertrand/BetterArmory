using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static BetterArmory.Main;

namespace BetterArmory.Equipment
{
    public class ExampleEquipment : EquipmentBase
    {
        public override string EquipmentName => "Deprecate Me Equipment";

        public override string EquipmentLangTokenName => "DEPRECATE_ME_EQUIPMENT";

        public override string EquipmentPickupDesc => "";

        public override string EquipmentFullDescription => "";

        public override string EquipmentLore => "";

        public override GameObject EquipmentModel => MainAssets.LoadAsset<GameObject>("assets/models/prefabs/item/firstitem/littleplate.prefab");

        public override Sprite EquipmentIcon => MainAssets.LoadAsset<Sprite>("assets/textures/icons/item/littleplate_icon.png");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            return false;
        }


    }
}
