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
    private ParkLifeSection m_ParkLifeAreaSystem;
    private SelectedInfoUISystem m_SelectedInfoUISystem;

    protected override void OnCreate()
    {
      base.OnCreate();
      m_ParkLifeAreaSystem = World.GetOrCreateSystemManaged<ParkLifeSection>();
      m_SelectedInfoUISystem = World.GetOrCreateSystemManaged<SelectedInfoUISystem>();

      AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "selectedPark", HasSelectedPark));
      AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "ticketsEnabled", () => HasOption(ParkOption.TicketsEnabled)));
      AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "dogsAllowed", () => HasOption(ParkOption.DogsAllowed)));
      AddUpdateBinding(new GetterValueBinding<bool>(kGroup, "bicyclesAllowed", () => HasOption(ParkOption.BicyclesAllowed)));
      AddUpdateBinding(new GetterValueBinding<int>(kGroup, "ticketPrice", () => GetSelectedPark().m_TicketPrice));

      AddBinding(new TriggerBinding<bool>(kGroup, "setTicketsEnabled", value => SetSelectedPark(value, null, null, null)));
      AddBinding(new TriggerBinding<bool>(kGroup, "setDogsAllowed", value => SetSelectedPark(null, value, null, null)));
      AddBinding(new TriggerBinding<bool>(kGroup, "setBicyclesAllowed", value => SetSelectedPark(null, null, value, null)));
      AddBinding(new TriggerBinding<int>(kGroup, "setTicketPrice", value => SetSelectedPark(null, null, null, Mathf.Clamp(value, 0, 100))));
    }

    private bool HasSelectedPark()
    {
      return EntityManager.HasComponent<ParkLife>(m_SelectedInfoUISystem.selectedEntity);
    }

    private ParkLife GetSelectedPark()
    {
      return HasSelectedPark() ? EntityManager.GetComponentData<ParkLife>(m_SelectedInfoUISystem.selectedEntity) : default;
    }

    private void SetSelectedPark(bool? ticketsEnabled, bool? dogsAllowed, bool? bicyclesAllowed, int? ticketPrice)
    {
      Entity selected = m_SelectedInfoUISystem.selectedEntity;
      if (!EntityManager.HasComponent<ParkLife>(selected))
      {
        return;
      }
      ParkLife park = EntityManager.GetComponentData<ParkLife>(selected);
      if (ticketsEnabled.HasValue) SetOption(ref park, ParkOption.TicketsEnabled, ticketsEnabled.Value);
      if (dogsAllowed.HasValue) SetOption(ref park, ParkOption.DogsAllowed, dogsAllowed.Value);
      if (bicyclesAllowed.HasValue) SetOption(ref park, ParkOption.BicyclesAllowed, bicyclesAllowed.Value);
      if (ticketPrice.HasValue) park.m_TicketPrice = ticketPrice.Value;
      EntityManager.SetComponentData(selected, park);
    }

    private bool HasOption(ParkOption option)
    {
      if (!HasSelectedPark()) return false;
      return (GetSelectedPark().m_OptionMask & option) == option;
    }

    private static void SetOption(ref ParkLife park, ParkOption option, bool enabled)
    {
      if (enabled) park.m_OptionMask |= option;
      else park.m_OptionMask &= ~option;
    }
  }
}
