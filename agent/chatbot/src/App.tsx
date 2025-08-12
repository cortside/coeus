import React from 'react';
import Chat from './components/Chat';
import Login from './components/Login';
import { useAuth } from './hooks/useAuth';

function App() {
  const { isAuthenticated } = useAuth();
  return (
    <div className="min-h-screen bg-gray-100">
      {isAuthenticated ? <Chat /> : <Login />}
    </div>
  );
}

export default App;
