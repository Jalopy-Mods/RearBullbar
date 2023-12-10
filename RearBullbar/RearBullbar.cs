using JaLoader;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using HarmonyLib;
using Object = UnityEngine.Object;

namespace RearBullbar
{
    public class RearBullbar : Mod
    {
        public override string ModID => "RearBullbar";
        public override string ModName => "Rear Bullbar";
        public override string ModAuthor => "Leaxx";
        public override string ModDescription => "Adds an optional rear bullbar that decreases damage!";
        public override string ModVersion => "1.0";
        public override string GitHubLink => "https://github.com/Jalopy-Mods/RearBullbar";
        public override WhenToInit WhenToInit => WhenToInit.InGame;
        public override List<(string, string, string)> Dependencies => new List<(string, string, string)>()
        {
            ("JaLoader", "Leaxx", "2.0.1")
        };

        public override bool UseAssets => true;

        private GameObject bullbarObject;

        private static Harmony harmony;

        public override void EventsDeclaration()
        {
            base.EventsDeclaration();
        }

        public override void SettingsDeclaration()
        {
            base.SettingsDeclaration();
        }

        public override void CustomObjectsRegistration()
        {
            base.CustomObjectsRegistration();

            bullbarObject = LoadAsset<GameObject>("rearbullbar", "rearBullbar", "", ".prefab");
            bullbarObject = Instantiate(bullbarObject, ModHelper.Instance.laika.transform.Find("TweenHolder/Frame"));
            bullbarObject.transform.localPosition = new Vector3(0, -3.45f, 0);
            bullbarObject.GetComponent<MeshRenderer>().material = ModHelper.Instance.defaultEngineMaterial;

            ModHelper.Instance.CreateIconForExtra(bullbarObject, new Vector3(-1f, 0, 0), new Vector3(0.25f, 0.25f, 0.25f), new Vector3(28, 235, 0), "RearBullbar");

            CustomObjectsManager.Instance.RegisterObject(ModHelper.Instance.CreateExtraObject(bullbarObject, BoxSizes.Big, "Rear Bullbar", "Halves damage incurred from crashing the car.", 100, 15, "RearBullbar", AttachExtraTo.Body), "RearBullbar");

            if (harmony == null)
            {
                harmony = new Harmony($"{ModAuthor}_{ModID}");
                harmony.PatchAll();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }
    }

    [HarmonyPatch(typeof(CarCollisionsC), "OnCollisionEnter")]
    public static class CarCollisionsC_OnCollisionEnter_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(CarCollisionsC __instance, Collision col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Ground") || col.gameObject.layer == LayerMask.NameToLayer("Grass") || col.gameObject.layer == LayerMask.NameToLayer("CarMesh") || col.gameObject.layer == LayerMask.NameToLayer("Untagged") || col.gameObject.layer == LayerMask.NameToLayer("Player") || col.gameObject.layer == LayerMask.NameToLayer("Uncle") || col.gameObject.layer == LayerMask.NameToLayer("TrueObstacle") || col.gameObject.layer == LayerMask.NameToLayer("AIObstacles") || col.gameObject.layer == LayerMask.NameToLayer("AICollider"))
            {
                return;
            }

            if (col.gameObject.tag == "AICar" && col.relativeVelocity.magnitude > (float)__instance.fineVelocity)
            {
                __instance.carLogic.GetComponent<CarLogicC>().TrafficLightPenalty();
            }

            if (col.relativeVelocity.magnitude > 20f)
            {
                float num = col.relativeVelocity.magnitude / 25f;

                if (__instance.carLogic.transform.Find("BodyExtrasHolder/Extra_RearBullbar_rearBullbar_RearBullbar_Leaxx/Mesh").gameObject.activeSelf)
                {
                    num = col.relativeVelocity.magnitude / 50f;
                }

                if (__instance.carLogic.GetComponent<ExtraUpgradesC>().bullBarInstalled)
                {
                    num = col.relativeVelocity.magnitude / 50f;
                }

                __instance.carLogic.GetComponent<CarPerformanceC>().carCondition -= num;
                __instance.carLogic.GetComponent<CarLogicC>().CarDamageUp();
                float num2 = Random.Range(0f, num);
                __instance.damageShuffle[0] = num2;
                num -= num2;
                float num3 = Random.Range(0f, num);
                __instance.damageShuffle[1] = num3;
                num -= num3;
                float num4 = Random.Range(0f, num);
                __instance.damageShuffle[2] = num4;
                num -= num4;
                float num5 = Random.Range(0f, num);
                __instance.damageShuffle[3] = num5;
                num -= num5;
                float num6 = Random.Range(0f, num);
                __instance.damageShuffle[4] = num6;
                num -= num6;
                float num7 = Random.Range(0f, num);
                __instance.damageShuffle[5] = num7;
                num -= num7;
                float num8 = Random.Range(0f, num);
                __instance.damageShuffle[6] = num8;
                num -= num8;
                for (int i = 0; i < __instance.damageShuffle.Count; i++)
                {
                    float value = __instance.damageShuffle[i];
                    int index = Random.Range(i, __instance.damageShuffle.Count);
                    __instance.damageShuffle[i] = __instance.damageShuffle[index];
                    __instance.damageShuffle[index] = value;
                }

                __instance.ApplyDamage();
            }

            if (col.gameObject.tag != "Player" && col.contacts.Length > 0)
            {
                ContactPoint val = col.contacts[0];
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, val.normal);
                Vector3 point = val.point;
                GameObject obj = Object.Instantiate(__instance.crashAudio, point, rotation);
                Object.Destroy(obj, 2f);
            }

            return;
        }
    }
}
