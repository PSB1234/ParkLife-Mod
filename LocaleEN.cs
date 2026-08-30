using Colossal;
using System.Collections.Generic;

namespace ParkLife
{
  /// <summary>English text shown by the built-in Areas toolbar for ParkLife.</summary>
  public sealed class LocaleEN : IDictionarySource
  {
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(
      IList<IDictionaryEntryError> errors,
      Dictionary<string, int> indexCounts)
    {
      return new Dictionary<string, string>
      {
        { "Assets.NAME[Park Area]", "Park Area" },
        { "Assets.DESCRIPTION[Park Area]", "Draw a managed park area." }
      };
    }

    public void Unload()
    {
    }
  }
}
