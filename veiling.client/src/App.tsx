import "./App.css";
import AccountDropdown, { AccountDropdownLine } from "./components/account_dropdown";

function App() {
    return (
        <div style={{ padding: "2rem" }}>
            <AccountDropdown>
                <AccountDropdownLine>Naam</AccountDropdownLine>
                <AccountDropdownLine>E-mail</AccountDropdownLine>
                <AccountDropdownLine>Bedrijf</AccountDropdownLine>
                <AccountDropdownLine>Functie</AccountDropdownLine>
            </AccountDropdown>
        </div>
    );
}


export default App;