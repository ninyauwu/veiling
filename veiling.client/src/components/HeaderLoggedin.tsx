import AccountDropdown, { AccountDropdownLine } from "./AccountDropdown";

type LoggedInHeaderProps = {
    email: string;
    roles: string[];
};

export default function LoggedInHeader({ email, roles }: LoggedInHeaderProps) {
    function handleLogout() {
        localStorage.removeItem("access_token");
        localStorage.removeItem("refresh_token");
        window.location.href = "/login";
    }

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
                <strong>Welkom,</strong> {email}
            </div>

            <AccountDropdown align="right" onLogout={handleLogout}>
                <AccountDropdownLine>
                    <strong>{email}</strong>
                </AccountDropdownLine>

                <AccountDropdownLine>
                    Rollen: {roles.join(", ")}
                </AccountDropdownLine>
            </AccountDropdown>
        </header>
    );
}