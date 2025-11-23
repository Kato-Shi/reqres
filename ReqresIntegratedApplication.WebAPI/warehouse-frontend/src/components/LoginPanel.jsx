import React, { useState } from 'react';

const DEMO_EMAIL = 'eve.holt@reqres.in';
const DEMO_PASSWORD = 'cityslicka';

/**
 * Lightweight login form that gates the SPA without calling ReqRes.
 * It simply matches the well-known demo credentials and returns a
 * faux token so the rest of the experience can stay unchanged.
 */
function LoginPanel({ onLogin }) {
  const [email, setEmail] = useState(DEMO_EMAIL);
  const [password, setPassword] = useState(DEMO_PASSWORD);
  const [status, setStatus] = useState('');

  const handleSubmit = (event) => {
    event.preventDefault();
    setStatus('');

    if (email.trim().toLowerCase() === DEMO_EMAIL && password === DEMO_PASSWORD) {
      onLogin({ email, token: 'demo-local-token' });
    } else {
      setStatus('Invalid credentials. Use eve.holt@reqres.in / cityslicka to continue.');
    }
  };

  return (
    <div className="login-shell">
      <div className="login-card panel">
        <h1>TeamShift Lite</h1>
        <p className="muted">Enter the demo credentials to open the warehouse dashboard.</p>
        <form className="form" onSubmit={handleSubmit}>
          <label>
            Email
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              autoComplete="username"
            />
          </label>
          <label>
            Password
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              autoComplete="current-password"
            />
          </label>
          <button type="submit" className="primary">Login</button>
        </form>
        {status && <div className="status error">{status}</div>}
        <div className="helper">
          <strong>Need the credentials?</strong> Use <code>{DEMO_EMAIL}</code> with password <code>{DEMO_PASSWORD}</code>.
        </div>
      </div>
    </div>
  );
}

export default LoginPanel;
