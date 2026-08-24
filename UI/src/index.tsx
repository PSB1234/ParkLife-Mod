import { ModRegistrar } from "cs2/modding";
import { ParkLifePanel } from "mods/parklife-panel";

const register: ModRegistrar = (moduleRegistry) => {

    moduleRegistry.append('GameBottomRight', ParkLifePanel);
}

export default register;
