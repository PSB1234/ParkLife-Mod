using Colossal.Serialization.Entities;
using Unity.Entities;

namespace ParkLife.Systems
{
  /// <summary>
  /// Persistent settings owned by one ParkLife district entity.
  /// The component lives on the game's district polygon, so it is saved and
  /// deleted with that polygon instead of maintaining a fragile external list.
  /// </summary>
  public struct ParkLifeArea : IComponentData, IQueryTypeParameter, ISerializable
  {
    public ParkOption m_OptionMask;

    public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
    {
      writer.Write((uint)m_OptionMask);
    }

    public void Deserialize<TReader>(TReader reader) where TReader : IReader
    {
      reader.Read(out uint m_OptionMask);
    }
  }
}
