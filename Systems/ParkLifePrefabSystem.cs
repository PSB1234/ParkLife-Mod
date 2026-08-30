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
      parkPrefab.name = "Park Area";
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

      // AddPrefab only creates the prefab entity. Because this prefab is
      // registered after the normal prefab initialization pass, run the same
      // initialization hooks here. AreaPrefab.LateInitialize creates the
      // area-entity archetype that AreaToolSystem needs when it draws a
      // polygon; without it the game crashes as soon as the tool is used.
      Entity parkPrefabEntity = m_PrefabSystem.GetEntity(parkPrefab);
      parkPrefab.Initialize(EntityManager, parkPrefabEntity);
      parkUI.Initialize(EntityManager, parkPrefabEntity);
      parkPrefab.LateInitialize(EntityManager, parkPrefabEntity);
      parkUI.LateInitialize(EntityManager, parkPrefabEntity);

      // AreaToolSystem reads AreaGeometryData from the prefab while it builds
      // the dotted polygon preview. Runtime-created prefabs start as lots, so
      // copy the District prefab's geometry settings for district-style area
      // drawing and editing.
      Entity districtPrefabEntity = m_PrefabSystem.GetEntity(districtPrefab);
      if (EntityManager.HasComponent<AreaGeometryData>(districtPrefabEntity) &&
          EntityManager.HasComponent<AreaGeometryData>(parkPrefabEntity))
      {
        EntityManager.SetComponentData(
          parkPrefabEntity,
          EntityManager.GetComponentData<AreaGeometryData>(districtPrefabEntity));
      }

    }
  }
}
