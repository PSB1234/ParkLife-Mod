import { bindTrigger, bindTriggerWithArgs, bindValue, useValue } from "cs2/api";

const selectedPark$ = bindValue<boolean>("parklife", "selectedPark", false);
const ticketsEnabled$ = bindValue<boolean>("parklife", "ticketsEnabled", false);
const dogsAllowed$ = bindValue<boolean>("parklife", "dogsAllowed", true);
const bicyclesAllowed$ = bindValue<boolean>("parklife", "bicyclesAllowed", true);
const ticketPrice$ = bindValue<number>("parklife", "ticketPrice", 0);

const drawPark = bindTrigger("parklife", "drawPark");
const setTicketsEnabled = bindTriggerWithArgs<[boolean]>("parklife", "setTicketsEnabled");
const setDogsAllowed = bindTriggerWithArgs<[boolean]>("parklife", "setDogsAllowed");
const setBicyclesAllowed = bindTriggerWithArgs<[boolean]>("parklife", "setBicyclesAllowed");
const setTicketPrice = bindTriggerWithArgs<[number]>("parklife", "setTicketPrice");

const panelStyle = {
  width: "270rem",
  margin: "0 12rem 12rem 0",
  padding: "14rem",
  color: "#f5f4ee",
  background: "rgba(20, 32, 25, 0.96)",
  border: "1rem solid #6ea96f",
  boxShadow: "0 8rem 28rem rgba(0, 0, 0, 0.42)",
  fontFamily: "var(--font-family-base, sans-serif)",
} as const;

export const ParkLifePanel = () => {
  const selectedPark = useValue(selectedPark$);
  const ticketsEnabled = useValue(ticketsEnabled$);
  const dogsAllowed = useValue(dogsAllowed$);
  const bicyclesAllowed = useValue(bicyclesAllowed$);
  const ticketPrice = useValue(ticketPrice$);

  return <div style={panelStyle}>
    <div style={{ color: "#9fdb8b", fontSize: "12rem", fontWeight: 700, letterSpacing: "1rem", textTransform: "uppercase" }}>ParkLife</div>
    <div style={{ fontSize: "17rem", fontWeight: 700, margin: "3rem 0 10rem" }}>Park area</div>
    {!selectedPark ? <div style={{ marginTop: "12rem", color: "#c2cbc0", lineHeight: 1.35 }}>Open Areas and select ParkLife Park Area to draw a park, then select it here to manage its rules.</div> : <div style={{ marginTop: "12rem" }}>
      <div style={{ borderTop: "1rem solid #426246", paddingTop: "9rem", marginBottom: "8rem", color: "#9fdb8b", fontSize: "12rem", fontWeight: 700 }}>Park rules</div>
      <label><input type="checkbox" checked={ticketsEnabled} onChange={event => setTicketsEnabled(event.currentTarget.checked)} /> Charge entry ticket</label>
      <label style={{ display: "block", marginTop: "8rem", opacity: ticketsEnabled ? 1 : 0.55 }}>Ticket price: {ticketPrice}
        <input type="range" min="0" max="100" step="1" value={ticketPrice} disabled={!ticketsEnabled} onChange={event => setTicketPrice(Number(event.currentTarget.value))} style={{ display: "block", width: "100%" }} />
      </label>
      <label style={{ display: "block", marginTop: "8rem" }}><input type="checkbox" checked={dogsAllowed} onChange={event => setDogsAllowed(event.currentTarget.checked)} /> Dogs allowed</label>
      <label style={{ display: "block", marginTop: "8rem" }}><input type="checkbox" checked={bicyclesAllowed} onChange={event => setBicyclesAllowed(event.currentTarget.checked)} /> Bicycles allowed</label>
      <div style={{ marginTop: "10rem", color: "#c2cbc0", fontSize: "11rem", lineHeight: 1.35 }}>Entry fees and dog rules are saved now. Visitor charging and dog routing are the next simulation feature.</div>
    </div>}
  </div>;
};
