import { ModRegistrar } from "cs2/modding";
import { ParkLifeInfoSection } from "./parklife-info-section";

const register: ModRegistrar = (moduleRegistry) => {
  const modulePath = "game-ui/game/components/selected-info-panel/selected-info-sections/selected-info-sections.tsx";
  const existingSections = moduleRegistry.get(modulePath, "selectedInfoSectionComponents");

  moduleRegistry.override(modulePath, "selectedInfoSectionComponents", {
    ...existingSections,
    "ParkLife.Systems.ParkLifeSection": ParkLifeInfoSection,
  });
};

export default register;
