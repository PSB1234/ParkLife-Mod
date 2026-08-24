using Game;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine;

namespace ParkLife.Systems
{
  /// <summary>
  /// Registers one ParkLife area prefab and places it beside the built-in
  /// district tool in the Areas menu.
  /// </summary>
  public partial class ParkLifePrefabSystem : GameSystemBase
  {
    private PrefabSystem m_PrefabSystem;
    private EntityQuery m_AreasConfigurationQuery;
    private bool m_Registered;

    protected override void OnCreate()
    {
      base.OnCreate();
      m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
      m_AreasConfigurationQuery = GetEntityQuery(ComponentType.ReadOnly<AreasConfigurationData>());
    }

    protected override void OnUpdate()
    {
      if (m_Registered || m_AreasConfigurationQuery.IsEmptyIgnoreFilter)
      {
        return;
      }

      AreasConfigurationData areas = EntityManager.GetComponentData<AreasConfigurationData>(
        m_AreasConfigurationQuery.GetSingletonEntity());

      if (!m_PrefabSystem.TryGetPrefab<AreaPrefab>(areas.m_DefaultDistrictPrefab, out AreaPrefab districtPrefab))
      {
        return;
      }

      UIObject districtUI = districtPrefab.GetComponent<UIObject>();
      if (districtUI == null || districtUI.m_Group == null)
      {
        return;
      }

      ParkLifePrefab parkPrefab = ScriptableObject.CreateInstance<ParkLifePrefab>();
      parkPrefab.name = "ParkLife Park Area";
      parkPrefab.m_Color = new Color(0.35f, 0.72f, 0.40f, 1f);
      parkPrefab.m_EdgeColor = new Color(0.55f, 0.90f, 0.56f, 1f);
      parkPrefab.m_SelectionColor = new Color(0.42f, 0.86f, 0.47f, 1f);
      parkPrefab.m_SelectionEdgeColor = Color.white;

      ParkLifeUI parkUI = parkPrefab.AddComponent<ParkLifeUI>();
      parkUI.m_Group = districtUI.m_Group;
      parkUI.m_Priority = districtUI.m_Priority + 1;
            parkUI.m_Icon = "coui://ui-mods/images/park-area.svg";
      parkPrefab.m_UIObject = parkUI;

      m_Registered = m_PrefabSystem.AddPrefab(parkPrefab);
      if (!m_Registered)
      {
        return;
      }

      // Runtime-created prefabs are added after the game's normal prefab
      // initialization pass. Complete UIObject's LateInitialize work here so
      // the existing Areas toolbar receives our fourth entry.
      Entity parkPrefabEntity = m_PrefabSystem.GetEntity(parkPrefab);
      Entity areasGroupEntity = m_PrefabSystem.GetEntity(districtUI.m_Group);
      EntityManager.SetComponentData(parkPrefabEntity, new UIObjectData
      {
        m_Group = areasGroupEntity,
        m_Priority = parkUI.m_Priority
      });
      EntityManager.GetBuffer<UIGroupElement>(areasGroupEntity).Add(
        new UIGroupElement(parkPrefabEntity));
    }
  }
}
