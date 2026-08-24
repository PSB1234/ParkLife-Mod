using Game;
using Game.Common;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace ParkLife.Systems
{
  /// <summary>Applies ParkLife's default rules to each newly drawn park.</summary>
  public partial class ParkLifeAreaSystem : GameSystemBase
  {
    private EntityQuery m_NewParkQuery;

    protected override void OnCreate()
    {
      base.OnCreate();
      m_NewParkQuery = GetEntityQuery(new EntityQueryDesc
      {
        All = new[] { ComponentType.ReadWrite<ParkLife>(), ComponentType.ReadOnly<Created>() },
        None = new[] { ComponentType.ReadOnly<Temp>() }
      });
    }

    protected override void OnUpdate()
    {
      using NativeArray<Entity> parks = m_NewParkQuery.ToEntityArray(Allocator.Temp);
      for (int i = 0; i < parks.Length; i++)
      {
        EntityManager.SetComponentData(parks[i], new ParkLife
        {
          m_OptionMask = ParkOption.DogsAllowed | ParkOption.BicyclesAllowed,
          m_TicketPrice = 0
        });
      }
    }
  }
}
