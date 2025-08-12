import { useState } from 'react';

export function useAuth() {
  // Placeholder: implement OIDC PKCE login and JWT storage in memory
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [token, setToken] = useState<string | null>(null);

  function login() {
    // TODO: OIDC PKCE login flow
    setIsAuthenticated(true);
    setToken('demo-token');
  }

  function logout() {
    setIsAuthenticated(false);
    setToken(null);
  }

  return { isAuthenticated, token, login, logout };
}
