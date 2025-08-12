import React from 'react';
import { useAuth } from '../hooks/useAuth';

const Login: React.FC = () => {
  const { login } = useAuth();
  return (
    <div className="flex flex-col items-center justify-center h-screen">
      <button
        onClick={login}
        className="bg-blue-600 text-white px-6 py-3 rounded shadow"
      >
        Login with Cortside
      </button>
    </div>
  );
};

export default Login;
