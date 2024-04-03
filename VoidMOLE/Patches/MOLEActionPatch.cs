using HarmonyLib;
using Mirror;
using PropStreaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoidMOLE.Patches
{
    internal class MOLEActionPatch
    {
        [HarmonyPatch(typeof(MOLEAction), "PostTerrainManipulationHelper")]
        [HarmonyPrefix]
        static bool voidItems(MOLEAction __instance, NetworkConnectionToClient sender, int tick, bool madeChanges, ref PlayerDigger.DigResultInfo digResultInfo) {
            if (digResultInfo.resourceCounts != null && digResultInfo.resourceCounts.Count > 0) {
                InventoryWrapper inventoryForPlayer = NetworkMessageRelay.GetInventoryForPlayer(sender);
                List<NetworkInventorySlot> list = new List<NetworkInventorySlot>();
                foreach (KeyValuePair<ResourceInfo, int> pair in digResultInfo.resourceCounts) {
                    if(pair.Key.displayName == "Limestone" && VoidMOLEPlugin.voidLimestone.Value) continue;
                    if(pair.Key.displayName == "Plantmatter" && VoidMOLEPlugin.voidPlantmatter.Value) continue;
                    
                    if (inventoryForPlayer.AddResources(pair.Key, pair.Value)) {
                        list.Add(pair);
                    }
                }
                if(list.Count != 0) {
                    NetworkMessageRelay.instance.NotifyInventoryAdd(sender, list.ToArray());
                }
            }
            if (digResultInfo.hitProtectedVoxel) {
                NetworkMessageRelay.instance.NotifyProtectedArea(sender);
            }
            if (madeChanges) {
                List<InstanceLookup> list2 = new List<InstanceLookup>();
                List<Vector3> list3 = new List<Vector3>();
                float impactRadius = 1f;
                int maxValue = int.MaxValue;
                Vector3 item = Vector3.zero;
                if (digResultInfo.coordsModified != null && digResultInfo.coordsModified.Count > 0) {
                    VoxelManager instance = VoxelManager.instance;
                    foreach (Vector3Int vector3Int in digResultInfo.coordsModified) {
                        List<InstanceLookup> list4;
                        if (instance.GetAssociatedFrillsFromSurfaceVoxel(vector3Int, out list4)) {
                            item = vector3Int + 0.5f * Vector3.one;
                            foreach (InstanceLookup item2 in list4) {
                                list3.Add(item);
                                list2.Add(item2);
                            }
                        }
                    }
                }
                if (digResultInfo.propsModified != null && digResultInfo.propsModified.Count > 0) {
                    PropManager instance2 = PropManager.instance;
                    foreach (InstanceLookup item3 in digResultInfo.propsModified) {
                        SpawnData spawnData;
                        if (!list2.Contains(item3) && instance2.GetPropData(item3, out spawnData)) {
                            item = spawnData.matrix.GetPositionFast() + 0.5f * Vector3.one;
                            list3.Add(item);
                            list2.Add(item3);
                        }
                    }
                }
                if (list2.Count > 0) {
                    HitDestructibleAction hitDestructibleAction = new HitDestructibleAction();
                    HitDestructibleInfo hitDestructibleInfo = new HitDestructibleInfo {
                        tick = tick,
                        actionID = hitDestructibleAction.actionID,
                        actionType = hitDestructibleAction.actionType,
                        hitStrength = maxValue,
                        impactPos = list3.ToArray(),
                        impactRadius = impactRadius,
                        propLookups = list2.ToArray()
                    };
                    hitDestructibleAction.info = hitDestructibleInfo;
                    hitDestructibleAction.ProcessOnHost(sender);
                    return false;
                }
            }
            else {
                Debug.LogWarning("Attempted to use terrain manipulator with a valid action but nothing happened!");
            }

            return false;
        }
    }
}
