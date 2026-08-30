using Game;
using Game.Common;
using Game.Tools;
using Game.UI;
using System;
using Unity.Collections;
using Unity.Entities;

namespace ParkLife.Systems
{
  /// <summary>Applies ParkLife's default rules to each newly drawn park.</summary>
  public partial class ParkLifeAreaSystem : GameSystemBase
  {
    private EntityQuery m_NewParkQuery;
    private EntityQuery m_ParkQuery;
    private NameSystem m_NameSystem;

    protected override void OnCreate()
    {
      base.OnCreate();
      m_NewParkQuery = GetEntityQuery(new EntityQueryDesc
      {
        All = new[] { ComponentType.ReadWrite<ParkLife>(), ComponentType.ReadOnly<Created>() },
        None = new[] { ComponentType.ReadOnly<Temp>() }
      });
      m_ParkQuery = GetEntityQuery(
        ComponentType.ReadOnly<ParkLife>(),
        ComponentType.Exclude<Temp>());
      m_NameSystem = World.GetOrCreateSystemManaged<NameSystem>();
    }

    protected override void OnUpdate()
    {
      int nextParkNumber = GetNextParkNumber();
      using NativeArray<Entity> parks = m_NewParkQuery.ToEntityArray(Allocator.Temp);
      for (int i = 0; i < parks.Length; i++)
      {
        EntityManager.SetComponentData(parks[i], new ParkLife
        {
          m_OptionMask = ParkOption.DogsAllowed | ParkOption.BicyclesAllowed,
          m_TicketPrice = 0
        });
        m_NameSystem.SetCustomName(parks[i], $"Park {nextParkNumber}");
        nextParkNumber++;
      }
    }

    private int GetNextParkNumber()
    {
      int highestNumber = 0;
      using NativeArray<Entity> parks = m_ParkQuery.ToEntityArray(Allocator.Temp);
      for (int i = 0; i < parks.Length; i++)
      {
        if (!m_NameSystem.TryGetCustomName(parks[i], out string name) ||
            !name.StartsWith("Park ", StringComparison.Ordinal) ||
            !int.TryParse(name.Substring("Park ".Length), out int number))
        {
          continue;
        }

        highestNumber = Math.Max(highestNumber, number);
      }

      return highestNumber + 1;
    }
  }
}
