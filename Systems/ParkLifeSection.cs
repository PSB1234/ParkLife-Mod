using Colossal.UI.Binding;

using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Game.UI.InGame;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace ParkLife.Systems
{
  /// <summary>Converts the district drawn with the ParkLife tool into a park.</summary>
  public partial class ParkLifeSection : InfoSectionBase
  {
    private ToolSystem m_ToolSystem;

    private AreaToolSystem m_AreaToolSystem;

    private DefaultToolSystem m_DefaultToolSystem;

    private SelectionToolSystem m_SelectionToolSystem;

    private bool m_ParkLifeSelecting;
    private EntityQuery m_ParkQuery;

    private EntityQuery m_ParkPrefabQuery;

    private EntityQuery m_ParkModifiedQuery;

    private ValueBinding<bool> m_Selecting;
    protected override string group => "ParkSection";

    private NativeList<Entity> parks { get; set; }

    private bool parksMissing { get; set; }
    private ParkLifePrefab m_ParkAreaPrefab;
    protected override void Reset()
    {
      parks.Clear();
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
      m_ParkQuery = GetEntityQuery(ComponentType.ReadOnly<ParkLife>(), ComponentType.Exclude<Temp>());
      m_ParkPrefabQuery = GetEntityQuery(ComponentType.ReadOnly<ParkLifeData>(), ComponentType.Exclude<Locked>());
      m_ParkModifiedQuery = GetEntityQuery(new EntityQueryDesc
      {
        All = new ComponentType[1] { ComponentType.ReadOnly<ParkLife>() },
        Any = new ComponentType[2]
        {
        ComponentType.ReadOnly<Created>(),
        ComponentType.ReadOnly<Deleted>()
        },
        None = new ComponentType[1] { ComponentType.ReadOnly<Temp>() }
      });
      AddBinding(new TriggerBinding(group, "toggleSelectionTool", ToggleSelectionTool));
      AddBinding(new TriggerBinding(group, "toggleParkTool", ToggleParkTool));
      AddBinding(new TriggerBinding(group, "disableTool", DisableTool));
      AddBinding(m_Selecting = new ValueBinding<bool>(group, "selecting", initialValue: false));
    }
    protected override void OnUpdate()
    {
      base.visible = Visible();
      parksMissing = m_ParkQuery.IsEmptyIgnoreFilter;

      parks.Clear();
      using NativeArray<Entity> parkEntities = m_ParkQuery.ToEntityArray(Allocator.Temp);
      parks.AddRange(parkEntities);

      if (m_ParkAreaPrefab == null &&
          !m_ParkPrefabQuery.IsEmptyIgnoreFilter)
      {
        Entity prefabEntity = m_ParkPrefabQuery.GetSingletonEntity();

        m_ParkAreaPrefab =
            m_PrefabSystem.GetPrefab<ParkLifePrefab>(prefabEntity);
      }

    }

    private void OnToolChanged(ToolBaseSystem tool)
    {
      bool parkLifeSelectionActive =
          tool == m_SelectionToolSystem &&
          m_ParkLifeSelecting;

      if (m_Selecting.value && !parkLifeSelectionActive)
      {
        m_SelectionToolSystem.selectionOwner = Entity.Null;
        m_SelectionToolSystem.selectionType = SelectionType.None;
      }

      m_Selecting.Update(parkLifeSelectionActive);
    }
    [Preserve]
    protected override void OnDestroy()
    {
      parks.Dispose();
      base.OnDestroy();
    }
    private bool Visible()
    {
      return EntityManager.HasComponent<ParkLife>(selectedEntity);
    }
    protected override void OnPreUpdate()
    {
      base.OnPreUpdate();
      if (!m_ParkModifiedQuery.IsEmptyIgnoreFilter)
      {
        RequestUpdate();
      }
    }
    public override void OnWriteProperties(IJsonWriter writer)
    {
      writer.PropertyName("ParkLifeAreaMissing");
      writer.Write(parksMissing);
      writer.PropertyName("parkLifearea");
      writer.ArrayBegin(parks.Length);
      for (int i = 0; i < parks.Length; i++)
      {
        Entity entity = parks[i];
        writer.TypeBegin("selectedInfo.ParkLifeArea");
        writer.PropertyName("name");
        m_NameSystem.BindName(writer, entity);
        writer.PropertyName("entity");
        writer.Write(entity);
        writer.TypeEnd();
      }
      writer.ArrayEnd();
    }
    private void ToggleSelectionTool()
    {
      if (m_ToolSystem.activeTool == m_SelectionToolSystem)
      {
        m_ParkLifeSelecting = false;
        m_ToolSystem.activeTool = m_DefaultToolSystem;
        return;
      }
      m_ParkLifeSelecting = true;
      m_SelectionToolSystem.selectionType = SelectionType.ServiceDistrict;
      m_SelectionToolSystem.selectionOwner = selectedEntity;
      m_ToolSystem.activeTool = m_SelectionToolSystem;
    }
    private void ToggleParkTool()
    {
      if (m_ToolSystem.activeTool == m_AreaToolSystem)
      {
        m_ToolSystem.activeTool = m_DefaultToolSystem;
        return;
      }
      if (m_ParkAreaPrefab == null)
      {
        return;
      }

      m_AreaToolSystem.prefab = m_ParkAreaPrefab;
      m_ToolSystem.activeTool = m_AreaToolSystem;
    }

    private void DisableTool()
    {
      m_ToolSystem.activeTool = m_DefaultToolSystem;
    }

    // Required by InfoSectionBase. ParkLife does not use ServiceDistrict links,
    // so this section has no selected-entity relationship buffer to read.
    protected override void OnProcess()
    {
    }

    [Preserve]
    public ParkLifeSection()
    {
    }
  }
}
