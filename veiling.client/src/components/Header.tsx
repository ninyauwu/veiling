import { useEffect, useState } from "react";
import HeaderLoggedIn from "./HeaderLoggedin.tsx";
import HeaderLoggedOut from "./HeaderLoggedout.tsx";
import { authCheckFetch } from "../utils/AuthCheckFetch.tsx";

interface HeaderProps {
  showLocationDropdown?: boolean;
}

interface MeResponse {
  email: string;
  isEmailConfirmed: boolean;
  roles: string[];
}

function Header({ showLocationDropdown = true }: HeaderProps) {
  const [user, setUser] = useState<MeResponse | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const checkAuth = async () => {
      const me = await authCheckFetch("/me");
      setUser(me);
      setLoading(false);
    };

    checkAuth();
  }, []);

  if (loading) return null;

  if (user) {
  return (
    <HeaderLoggedIn
    email={user.email}
    roles={user.roles}
    />
  );
}

  return <HeaderLoggedOut showLocationDropdown={showLocationDropdown} />;
}

export default Header;
