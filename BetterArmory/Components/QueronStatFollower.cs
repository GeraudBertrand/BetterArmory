using BepInEx.Configuration;
using BetterArmory.Items.Tier3;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Components
{
    public class QueronStatFollower : MonoBehaviour
    {
        public StatSelector[] listStat = Enum.GetValues(typeof(StatSelector)).Cast<StatSelector>().ToArray();

        public Dictionary<StatSelector, int> stats = new Dictionary<StatSelector, int>();

        public void Initialize()
        {
            foreach (var stat in listStat) {
                stats.Add(stat,0);
            }
        }

        internal void Increment(StatSelector ss)
        {
            if (stats.ContainsKey(ss))
            {
                stats[ss] += 1;
            }
            else
            {
                ChatMessage.Send("Cette stat n'existe pas : "+ss);
            }
        }

        // Apply bonus to stat 
        internal void ApplyBonusStat(StatHookEventArgs args, float b1, float b2, float b3, float b4, float b5, float b6, float b7, float b8, float b9, float b10, float b11)
        {
            foreach (var item in stats)
            {
                switch (item.Key)
                {
                    case StatSelector.HEALTH:
                        args.baseHealthAdd += b1 * item.Value;
                        break;
                    case StatSelector.SHIELD:
                        args.baseShieldAdd += b2 * item.Value;
                        break;
                    case StatSelector.ARMOR:
                        args.armorAdd += b3 * item.Value;
                        break;
                    case StatSelector.REGEN:
                        args.baseRegenAdd += b4 * item.Value;
                        break;
                    case StatSelector.MOVESPEED:
                        args.baseMoveSpeedAdd += b5 * item.Value;
                        break;
                    case StatSelector.SPRINTSPEED:
                        args.sprintSpeedAdd += b6 * item.Value;
                        break;
                    case StatSelector.DAMAGE:
                        args.baseDamageAdd += b7 * item.Value;
                        break;
                    case StatSelector.ATTACKSPEED:
                        args.baseAttackSpeedAdd += b8 * item.Value;
                        break;
                    case StatSelector.CRITCHANCE:
                        args.critAdd += b9 * item.Value;
                        break;
                    case StatSelector.CRITDAMAGE:
                        args.critDamageMultAdd += b10 * item.Value;
                        break;
                    case StatSelector.COOLDOWN:
                        args.cooldownMultAdd -= b11 * item.Value;
                        break;
                }
            }
        }

    }
}