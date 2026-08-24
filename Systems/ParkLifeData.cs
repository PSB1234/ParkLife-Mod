using Colossal.Serialization.Entities;

using System.Runtime.InteropServices;

using Unity.Entities;

namespace ParkLife.Systems
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct ParkLifeData : IComponentData, IQueryTypeParameter, IEmptySerializable
  {
  }

}
