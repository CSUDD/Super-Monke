using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using System.Reflection;
using UnityEngine.XR;
using Photon.Pun;
using Photon;

namespace SuperMonkeV2
{
    [BepInPlugin("org.jeydevv.monkeytag.supermonke", "Super Monke!", "1.0.0.0")]
    public class MyPatcher : BaseUnityPlugin
    {
        public void Awake()
        {
            var harmony = new Harmony("com.jeydevv.monkeytag.supermonke");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(GorillaLocomotion.Player))]
    [HarmonyPatch("Update", MethodType.Normal)]
    class PowerPatch
    {
        static bool gravityToggled = false;
        static bool flying = false;
        static void Prefix(GorillaLocomotion.Player __instance)
        {
            if (!PhotonNetworkController.instance.isPrivate) return;

            bool triggerDown = false;
            bool primaryDown = false;
            bool secondaryDown = false;
            List<InputDevice> list = new List<InputDevice>();
            InputDevices.GetDevices(list);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].TryGetFeatureValue(CommonUsages.triggerButton, out triggerDown);
                list[i].TryGetFeatureValue(CommonUsages.primaryButton, out primaryDown);
                list[i].TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryDown);
            }

            if (triggerDown)
            {
                __instance.jumpMultiplier = 5f;
            }
            else
            {
               __instance.jumpMultiplier = 1.25f;
            }

            if (__instance.maxJumpSpeed < 999f)
            {
                __instance.maxJumpSpeed = 999f;
            }

            if (primaryDown)
            {
                __instance.transform.position += (__instance.headCollider.transform.forward * Time.deltaTime) * 12f;
                __instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                if (!flying)
                {
                    flying = true;
                }
            }
            else if (flying)
            {
                __instance.GetComponent<Rigidbody>().velocity = (__instance.headCollider.transform.forward * Time.deltaTime) * 12f;
                flying = false;
            }

            if (secondaryDown)
            {
                if (!gravityToggled && __instance.bodyCollider.attachedRigidbody.useGravity == true)
                {
                    __instance.bodyCollider.attachedRigidbody.useGravity = false;
                    gravityToggled = true;
                }
                else if (!gravityToggled && __instance.bodyCollider.attachedRigidbody.useGravity == false)
                {
                    __instance.bodyCollider.attachedRigidbody.useGravity = true;
                    gravityToggled = true;
                }
            }
            else
            {
                gravityToggled = false;
            }
        }
    }
}
