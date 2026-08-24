using Colossal.UI.Binding;
using Game;
using Game.UI;
using Game.UI.InGame;
using Unity.Entities;
using UnityEngine;

namespace ParkLife.Systems
{
  /// <summary>Exposes the currently selected ParkLife area's settings to the mod UI.</summary>
  public partial class ParkLifeUISystem : UISystemBase
  {
    private const string kGroup = "parklife";
    private ParkLifeAreaSystem m_ParkLifeAreaSystem;
    private SelectedInfoUISystem m_SelectedInfoUISystem;

    protected override void OnCreate()
    {
      base.OnCreate();
      m_ParkLifeAreaSystem = World.GetOrCreateSystemManaged<ParkLifeAreaSystem>();
      m_SelectedInfoUISystem = World.GetOrCreateSystemManaged<SelectedInfoUISystem>();

      AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "selectedPark", HasSelectedPark));
      AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "ticketsEnabled", () => GetSelectedPark().m_TicketsEnabled));
      AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "dogsAllowed", () => GetSelectedPark().m_DogsAllowed));
      AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "bicyclesAllowed", () => GetSelectedPark().m_BicyclesAllowed));
      AddUpdateBinding(new GetterValueBinding<int>(kGroup, "ticketPrice", () => GetSelectedPark().m_TicketPrice));

      AddBinding(new TriggerBinding(kGroup, "drawPark", m_ParkLifeAreaSystem.StartDrawingPark));
      AddBinding(new TriggerBinding<bool>(kGroup, "setTicketsEnabled", value => SetSelectedPark(value, null, null, null)));
      AddBinding(new TriggerBinding<bool>(kGroup, "setDogsAllowed", value => SetSelectedPark(null, value, null, null)));
      AddBinding(new TriggerBinding<bool>(kGroup, "setBicyclesAllowed", value => SetSelectedPark(null, null, value, null)));
      AddBinding(new TriggerBinding<int>(kGroup, "setTicketPrice", value => SetSelectedPark(null, null, null, Mathf.Clamp(value, 0, 100))));
    }

    private bool HasSelectedPark()
    {
      return EntityManager.HasComponent<ParkLifeArea>(m_SelectedInfoUISystem.selectedEntity);
    }

    private ParkLifeArea GetSelectedPark()
    {
      return HasSelectedPark() ? EntityManager.GetComponentData<ParkLifeArea>(m_SelectedInfoUISystem.selectedEntity) : default;
    }

    private void SetSelectedPark(bool? ticketsEnabled, bool? dogsAllowed, bool? bicyclesAllowed, int? ticketPrice)
    {
      Entity selected = m_SelectedInfoUISystem.selectedEntity;
      if (!EntityManager.HasComponent<ParkLifeArea>(selected))
      {
        return;
      }
      ParkLifeArea park = EntityManager.GetComponentData<ParkLifeArea>(selected);
      if (ticketsEnabled.HasValue) park.m_TicketsEnabled = ticketsEnabled.Value;
      if (dogsAllowed.HasValue) park.m_DogsAllowed = dogsAllowed.Value;
      if (bicyclesAllowed.HasValue) park.m_BicyclesAllowed = bicyclesAllowed.Value;
      if (ticketPrice.HasValue) park.m_TicketPrice = ticketPrice.Value;
      EntityManager.SetComponentData(selected, park);
    }
  }
}
