using Colossal.Serialization.Entities;
using Unity.Entities;

namespace ParkLife.Systems
{
  /// <summary>
  /// Persistent settings owned by one ParkLife district entity.
  /// The component lives on the game's district polygon, so it is saved and
  /// deleted with that polygon instead of maintaining a fragile external list.
  /// </summary>
  public struct ParkLife : IComponentData, IQueryTypeParameter, ISerializable
  {
    public ParkOption m_OptionMask;
    public int m_TicketPrice;

    public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
    {
      writer.Write((uint)m_OptionMask);
      writer.Write(m_TicketPrice);
    }

    public void Deserialize<TReader>(TReader reader) where TReader : IReader
    {
      reader.Read(out uint optionMask);
      m_OptionMask = (ParkOption)optionMask;
      reader.Read(out m_TicketPrice);
    }

  }
}
