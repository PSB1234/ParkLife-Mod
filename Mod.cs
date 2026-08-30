using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using HarmonyLib;
using ParkLife.Systems;

namespace ParkLife
{
  public class Mod : IMod
  {
    public static ILog log = LogManager.GetLogger($"{nameof(ParkLife)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
    private Harmony m_Harmony;

  public void OnLoad(UpdateSystem updateSystem)
  {
    log.Info(nameof(OnLoad));
    GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN());
    m_Harmony = new Harmony("ParkLife.SelectedInfo");
    m_Harmony.PatchAll();

      if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
        log.Info($"Current mod asset at {asset.path}");

      updateSystem.UpdateAt<ParkLifePrefabSystem>(SystemUpdatePhase.PrefabUpdate);
      updateSystem.UpdateAt<ParkLifeAreaSystem>(SystemUpdatePhase.PostSimulation);
      updateSystem.UpdateAt<ParkLifeSection>(SystemUpdatePhase.PostSimulation);
      updateSystem.UpdateAt<ParkLifeUISystem>(SystemUpdatePhase.UIUpdate);
    }

    public void OnDispose()
    {
      m_Harmony?.UnpatchAll("ParkLife.SelectedInfo");
      log.Info(nameof(OnDispose));
    }
  }
}
