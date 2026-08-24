using System;

namespace ParkLife.Systems
{ [Flags]
  public enum ParkOption
  {
    None = 0,
    TicketsEnabled = 1 << 0,
    DogsAllowed = 1 << 1,
    BicyclesAllowed = 1 << 2
  }
}
