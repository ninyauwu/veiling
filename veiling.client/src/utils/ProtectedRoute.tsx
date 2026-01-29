import { useEffect, useState, type JSX } from "react";
import { Navigate } from "react-router-dom";
import { authFetch } from "../utils/AuthFetch";

interface Props {
    children: JSX.Element;
}

export default function ProtectedRoute({ children }: Props) {
    const [allowed, setAllowed] = useState<boolean | null>(null);

    useEffect(() => {
        authFetch("/api/auth/me")
            .then(res => {
                if (!res.ok) throw new Error();
                setAllowed(true);
            })
            .catch(() => setAllowed(false));
    }, []);

    if (allowed === null) {
        return <div>Checking authentication...</div>;
    }

    if (!allowed) {
        return <Navigate to="/login" replace />;
    }

    return children;
}
