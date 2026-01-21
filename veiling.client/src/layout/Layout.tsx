import { Outlet } from "react-router-dom";
import Header from "../components/Header";

export default function Layout() {
  return (
    <>
      <Header />
      <main style={{ paddingTop: "80px" }}>
        <Outlet />
      </main>
    </>
  );
}
