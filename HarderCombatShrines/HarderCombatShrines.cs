using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HarderCombatShrines
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [R2APISubmoduleDependency(nameof(DirectorAPI))]
	
    public class HarderCombatShrines : BaseUnityPlugin
	{

        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "TJT";
        public const string PluginName = "HarderCombatShrines";
        public const string PluginVersion = "1.0.0";

        public void Awake()
        {
            On.RoR2.ShrineCombatBehavior.AddShrineStack += ShrineCombatBehavior_AddShrineStack;
            On.RoR2.ShrineCombatBehavior.Start += ShrineCombatBehavior_Start;
        }

        private void ShrineCombatBehavior_Start(On.RoR2.ShrineCombatBehavior.orig_Start orig, ShrineCombatBehavior self)
        {
            orig(self);

            if (NetworkServer.active)
                self.maxPurchaseCount = int.MaxValue;
        }

        private void ShrineCombatBehavior_AddShrineStack(On.RoR2.ShrineCombatBehavior.orig_AddShrineStack orig, ShrineCombatBehavior self, Interactor interactor)
        {
            orig(self, interactor);

            if (NetworkServer.active)
            { 
                var combatDirector = self.GetFieldValue<CombatDirector>("combatDirector");
                var monsterCredit = self.GetPropertyValue<float>("monsterCredit");

                var newChoseDirectorCard = combatDirector.SelectMonsterCardForCombatShrine(monsterCredit);

                if (newChoseDirectorCard == null)
                {
                    Debug.Log("HarderCombatShrines: Could not find appropriate spawn card for Combat Shrine");
                }
                else
                {
                    self.SetFieldValue<DirectorCard>("chosenDirectorCard", newChoseDirectorCard);
                }
            }
        }

    }
}
