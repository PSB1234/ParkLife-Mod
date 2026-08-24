using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using ParkLife.Systems;

namespace ParkLife
{
  public class Mod : IMod
  {
    public static ILog log = LogManager.GetLogger($"{nameof(ParkLife)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
    public void OnLoad(UpdateSystem updateSystem)
    {
      log.Info(nameof(OnLoad));

      if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
        log.Info($"Current mod asset at {asset.path}");

      updateSystem.UpdateAt<ParkLifePrefabSystem>(SystemUpdatePhase.PrefabUpdate);
      updateSystem.UpdateAt<ParkLifeAreaSystem>(SystemUpdatePhase.PostSimulation);
      updateSystem.UpdateAt<ParkLifeSection>(SystemUpdatePhase.PostSimulation);
      updateSystem.UpdateAt<ParkLifeUISystem>(SystemUpdatePhase.UIUpdate);
    }

    public void OnDispose()
    {
      log.Info(nameof(OnDispose));
    }
  }
}
