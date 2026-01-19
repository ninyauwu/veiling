type LoggedInHeaderProps = {
  email: string;
  roles: string[];
};

export default function LoggedInHeader({ email, roles }: LoggedInHeaderProps) {
  return (
    <header
      style={{
        padding: "10px 20px",
        backgroundColor: "#4caf50",
        color: "#fff",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      }}
    >
      <div>
        <span>Welkom, {email}</span>
      </div>
      <div>
        <span>Rollen: {roles.join(", ")}</span>
      </div>
      <button
        onClick={() => {
          alert("Logout nog niet geïmplementeerd");
        }}
        style={{
          marginLeft: "20px",
          padding: "5px 10px",
          cursor: "pointer",
        }}
      >
        Uitloggen
      </button>
    </header>
  );
}
