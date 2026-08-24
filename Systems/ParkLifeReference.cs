
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace ParkLife.Systems
{
  public struct ParkReference : IBufferElementData, ISerializable
  {
    public Entity m_Park;

    public void Serialize<TWriter>(TWriter writer)
        where TWriter : IWriter
    {
      writer.Write(m_Park);
    }

    public void Deserialize<TReader>(TReader reader)
        where TReader : IReader
    {
      reader.Read(out m_Park);
    }
  }
}
