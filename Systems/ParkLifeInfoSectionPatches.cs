using System;
using System.Collections.Generic;
using System.Reflection;
using Colossal.UI.Binding;
using Game.UI.InGame;
using HarmonyLib;

namespace ParkLife.Systems
{
  /// <summary>
  /// Suppresses District-only selected-info sections for ParkLife areas.
  /// ParkLife retains District for the stable base-game drawing engine.
  /// </summary>
  [HarmonyPatch(typeof(InfoSectionBase), nameof(InfoSectionBase.Write))]
  internal static class ParkLifeInfoSectionPatches
  {
    private static readonly HashSet<string> kDistrictOnlySections = new HashSet<string>
    {
      typeof(PoliciesSection).FullName,
      typeof(LocalServicesSection).FullName,
      typeof(EmployeesSection).FullName,
      typeof(ResidentsSection).FullName,
      typeof(AverageHappinessSection).FullName,
      typeof(ProfitabilitySection).FullName
    };

    private static readonly FieldInfo kInfoSystemField = typeof(InfoSectionBase).GetField(
      "m_InfoUISystem",
      BindingFlags.Instance | BindingFlags.NonPublic);

    private static bool Prefix(InfoSectionBase __instance, IJsonWriter writer)
    {
      if (!kDistrictOnlySections.Contains(__instance.GetType().FullName))
      {
        return true;
      }

      SelectedInfoUISystem infoSystem = kInfoSystemField?.GetValue(__instance) as SelectedInfoUISystem;
      if (infoSystem == null || !__instance.EntityManager.HasComponent<ParkLife>(infoSystem.selectedEntity))
      {
        return true;
      }

      writer.WriteNull();
      return false;
    }
  }
}
