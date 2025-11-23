import React, { useState } from 'react';
import { login } from '../services/apiClient';

function LoginPanel({ onAuthenticated }) {
  const [email, setEmail] = useState('eve.holt@reqres.in');
  const [password, setPassword] = useState('cityslicka');
  const [status, setStatus] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (evt) => {
    evt.preventDefault();
    setIsLoading(true);
    setStatus('');
    try {
      const token = await login(email, password);
      if (token && onAuthenticated) {
        onAuthenticated(token);
      }
    } catch (error) {
      setStatus(error.message || 'Login failed.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="panel">
      <h2>Warehouse Login</h2>
      <p>
        Sign in with the built-in demo account (no external call is made). This sidesteps
        ReqRes 401s while keeping the rest of the API live against reqres.in.
      </p>
      <form onSubmit={handleSubmit} className="form">
        <label>
          Email
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </label>
        <label>
          Password
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </label>
        <button type="submit" className="primary" disabled={isLoading}>
          {isLoading ? 'Signing in…' : 'Login'}
        </button>
        {status && <div className="status error">{status}</div>}
      </form>
      <div className="helper">
        <p>
          <strong>Why login?</strong> The API validates the credentials locally and returns a demo token so
          subsequent requests stay consistent. No data is stored beyond this session.
        </p>
      </div>
    </div>
  );
}

export default LoginPanel;
