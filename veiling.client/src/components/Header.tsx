import React from "react";
import { type Me } from "../types";

type HeaderProps = {
  me: Me | null;
};

const Header: React.FC<HeaderProps> = ({ me }) => {
  return (
    <header style={{ padding: "16px", backgroundColor: "#eee" }}>
      {me ? (
        <div>
          <span>Welkom, {me.email}</span>
          <span style={{ marginLeft: "16px" }}>
            Rollen: {me.roles.join(", ")}
          </span>
        </div>
      ) : (
        <div>
          <span>Niet ingelogd</span>
        </div>
      )}
    </header>
  );
};

export default Header;
