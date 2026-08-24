using Colossal.UI.Binding;
using Game;
using Game.Areas;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Game.UI.InGame;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace ParkLife.Systems
{
  /// <summary>Converts the district drawn with the ParkLife tool into a park.</summary>
  public partial class ParkLifeAreaSystem : InfoSectionBase
  {
    private ToolSystem m_ToolSystem;

    private AreaToolSystem m_AreaToolSystem;

    private DefaultToolSystem m_DefaultToolSystem;

    private SelectionToolSystem m_SelectionToolSystem;

    private EntityQuery m_ConfigQuery;

    private EntityQuery m_ParkQuery;

    private EntityQuery m_ParkPrefabQuery;

    private EntityQuery m_ParkModifiedQuery;

    private ValueBinding<bool> m_Selecting;
    protected override string group => "ParkSection";

    private NativeList<Entity> parks { get; set; }

    private bool districtMissing { get; set; }

    protected override void Reset()
    {
      parks.Clear();
    }

    public void StartDrawingPark()
    {
      if (m_ParkAreaPrefab == null)
      {
        return;
      }
      m_AreaToolSystem.prefab = m_ParkAreaPrefab;
      m_DistrictsBeforeDrawing.Clear();
      NativeArray<Entity> existingDistricts = m_DistrictQuery.ToEntityArray(Allocator.Temp);
      for (int i = 0; i < existingDistricts.Length; i += 1)
      {
        m_DistrictsBeforeDrawing.Add(existingDistricts[i]);
      }
      existingDistricts.Dispose();
      m_WaitingForParkDistrict = true;
      m_ToolSystem.activeTool = m_AreaToolSystem;
    }

    protected override void OnCreate()
    {
      base.OnCreate();
      m_ToolSystem = World.GetOrCreateSystemManaged<ToolSystem>();
      m_AreaToolSystem = World.GetOrCreateSystemManaged<AreaToolSystem>();
      m_DefaultToolSystem = World.GetOrCreateSystemManaged<DefaultToolSystem>();
      m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
      m_SelectionToolSystem = base.World.GetOrCreateSystemManaged<SelectionToolSystem>();
      ToolSystem toolSystem = m_ToolSystem;
      toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Combine(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
      parks = new NativeList<Entity>(Allocator.Persistent);
      m_ConfigQuery = GetEntityQuery(ComponentType.ReadOnly<AreasConfigurationData>());
      m_ParkQuery = GetEntityQuery(ComponentType.ReadOnly<ParkLifeArea>(), ComponentType.Exclude<Temp>());
      m_ParkPrefabQuery = GetEntityQuery(ComponentType.ReadOnly<DistrictData>(), ComponentType.Exclude<Locked>());
      m_ParkModifiedQuery = GetEntityQuery(new EntityQueryDesc
      {
        All = new ComponentType[1] { ComponentType.ReadOnly<District>() },
        Any = new ComponentType[2]
        {
        ComponentType.ReadOnly<Created>(),
        ComponentType.ReadOnly<Deleted>()
        },
        None = new ComponentType[1] { ComponentType.ReadOnly<Temp>() }
      });
      AddBinding(new TriggerBinding<Entity>(group, "removeDistrict", RemoveServiceDistrict));
      AddBinding(new TriggerBinding(group, "toggleSelectionTool", ToggleSelectionTool));
      AddBinding(new TriggerBinding(group, "toggleDistrictTool", ToggleDistrictTool));
      AddBinding(new TriggerBinding(group, "disableTool", DisableTool));
      AddBinding(m_Selecting = new ValueBinding<bool>(group, "selecting", initialValue: false));
    }

    protected override void OnUpdate()
    {
      InitializeParkAreaPrefab();
      if (m_DistrictQuery.IsEmptyIgnoreFilter)
      {
        return;
      }

      NativeArray<Entity> districts = m_DistrictQuery.ToEntityArray(Allocator.Temp);
      Entity newDistrict = Entity.Null;
      for (int i = 0; i < districts.Length; i += 1)
      {
        if (EntityManager.GetComponentData<PrefabRef>(districts[i]).m_Prefab == m_ParkAreaPrefabEntity || (m_WaitingForParkDistrict && !m_DistrictsBeforeDrawing.Contains(districts[i])))
        {
          newDistrict = districts[i];
          break;
        }
      }
      districts.Dispose();
      if (newDistrict == Entity.Null)
      {
        return;
      }

      EntityManager.AddComponentData(newDistrict, new ParkLifeArea
      {
        m_TicketsEnabled = false,
        m_DogsAllowed = true,
        m_BicyclesAllowed = true,
        m_TicketPrice = 0
      });
      // One activation creates one park. This prevents normal districts drawn
      // afterwards from accidentally inheriting the ParkLife designation.
      m_WaitingForParkDistrict = false;
      m_ToolSystem.activeTool = m_DefaultToolSystem;
    }

    private void InitializeParkAreaPrefab()
    {
      if (m_ParkAreaPrefab != null)
      {
        return;
      }

      AreasConfigurationPrefab configuration = m_PrefabSystem.GetPrefab<AreasConfigurationPrefab>(m_AreasConfigurationQuery.GetSingletonEntity());
      // Cloning the default district preserves its UIObject component, whose
      // group is the game's Areas tab. No new toolbar category is invented.
      m_ParkAreaPrefab = (AreaPrefab)m_PrefabSystem.DuplicatePrefab(configuration.m_DefaultDistrictPrefab, "ParkLife Park Area");
      m_ParkAreaPrefabEntity = m_PrefabSystem.GetEntity(m_ParkAreaPrefab);
      m_ParkAreaPrefab.Initialize(EntityManager, m_ParkAreaPrefabEntity);
      foreach (ComponentBase component in m_ParkAreaPrefab.components)
      {
        component.Initialize(EntityManager, m_ParkAreaPrefabEntity);
      }
      foreach (ComponentBase component in m_ParkAreaPrefab.components)
      {
        component.LateInitialize(EntityManager, m_ParkAreaPrefabEntity);
      }
      m_ParkAreaPrefab.LateInitialize(EntityManager, m_ParkAreaPrefabEntity);
      Mod.log.Info("Registered ParkLife Park Area in the Areas build tab.");
    }
  }
}
