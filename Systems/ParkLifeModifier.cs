using Game;
using Colossal.Serialization.Entities;
using Unity.Entities;
using Unity.Mathematics;

namespace ParkLife.Systems
{
  [InternalBufferCapacity(0)]
  public struct ParkLifeModifier : IBufferElementData, ISerializable
  {
    public float2 m_Delta;

    public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
    {
      writer.Write(m_Delta);
    }

    public void Deserialize<TReader>(TReader reader) where TReader : IReader
    {
      if (reader.context.version >= Version.modifierRefactoring)
      {
        reader.Read(out m_Delta);
      }
      else
      {
        reader.Read(out m_Delta.y);
      }
    }
  }
}
