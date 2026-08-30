import React from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { InfoRow, InfoSection } from "cs2/ui";

const selectedPark$ = bindValue<boolean>("parklife", "selectedPark", false);
const ticketsEnabled$ = bindValue<boolean>("parklife", "ticketsEnabled", false);
const dogsAllowed$ = bindValue<boolean>("parklife", "dogsAllowed", false);
const bicyclesAllowed$ = bindValue<boolean>("parklife", "bicyclesAllowed", false);
const ticketPrice$ = bindValue<number>("parklife", "ticketPrice", 0);

const rowStyle: React.CSSProperties = {
  display: "flex",
  alignItems: "center",
  justifyContent: "space-between",
  gap: "0.6rem",
  width: "100%",
};

const toggleStyle: React.CSSProperties = {
  minWidth: "4.4rem",
  padding: "0.25rem 0.55rem",
  border: "1px solid rgba(146, 209, 125, 0.65)",
  borderRadius: "0.2rem",
  background: "rgba(47, 102, 56, 0.72)",
  color: "#e9ffe5",
};

function RuleToggle({ label, value, binding }: { label: string; value: boolean; binding: string }) {
  return (
    <InfoRow
      left={label}
      right={
        <button style={toggleStyle} onClick={() => trigger("parklife", binding, !value)}>
          {value ? "Allowed" : "Not allowed"}
        </button>
      }
    />
  );
}

export const ParkLifeInfoSection = () => {
  const selectedPark = useValue(selectedPark$);
  const ticketsEnabled = useValue(ticketsEnabled$);
  const dogsAllowed = useValue(dogsAllowed$);
  const bicyclesAllowed = useValue(bicyclesAllowed$);
  const ticketPrice = useValue(ticketPrice$);

  if (!selectedPark) return null;

  return (
    <InfoSection>
      <InfoRow left="Park rules" uppercase />
      <RuleToggle label="Entry tickets" value={ticketsEnabled} binding="setTicketsEnabled" />
      <InfoRow
        left="Ticket price"
        right={
          <div style={rowStyle}>
            <input
              aria-label="Ticket price"
              type="range"
              min={0}
              max={100}
              value={ticketPrice}
              disabled={!ticketsEnabled}
              onChange={(event) => trigger("parklife", "setTicketPrice", Number(event.currentTarget.value))}
            />
            <span>{ticketPrice}</span>
          </div>
        }
      />
      <RuleToggle label="Dogs" value={dogsAllowed} binding="setDogsAllowed" />
      <RuleToggle label="Bicycles" value={bicyclesAllowed} binding="setBicyclesAllowed" />
    </InfoSection>
  );
};
