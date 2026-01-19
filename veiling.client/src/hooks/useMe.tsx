import { useEffect, useState } from "react";
import { authFetch } from "../utils/AuthFetch";

export type Me = {
  id: string;
  name: string;
  role: string;
  bedrijfId?: number;
};

export function useMe() {
  const [me, setMe] = useState<Me | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchMe = async () => {
      try {
        const response = await authFetch("/me");

        if (!response.ok) {
          setMe(null);
          return;
        }

        const data = await response.json();
        setMe(data);
      } catch {
        setMe(null);
      } finally {
        setLoading(false);
      }
    };

    fetchMe();
  }, []);

  return { me, loading, isLoggedIn: !!me };
}
