import { useEffect, useState, type JSX } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { authFetch } from "../utils/AuthFetch";

interface Props {
    children: JSX.Element;
}

export default function ProtectedRoute({ children }: Props) {
    const [allowed, setAllowed] = useState<boolean | null>(null);
    const location = useLocation();

    useEffect(() => {
        authFetch("/api/me")
            .then(res => {
                if (!res.ok) throw new Error();
                setAllowed(true);
            })
            .catch(() => setAllowed(false));
    }, []);

    if (allowed === null) {
        return (
            <div style={{ padding: "40px", textAlign: "center" }}>
                Checking authentication...
            </div>
        );
    }

    if (!allowed) {
        return (
            <Navigate
                to="/login"
                replace
                state={{ from: location.pathname }}
            />
        );
    }

    return children;
}
