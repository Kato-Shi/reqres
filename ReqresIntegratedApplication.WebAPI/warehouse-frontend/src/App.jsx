import React, { useEffect, useMemo, useState } from 'react';
import AssignmentsPanel from './components/AssignmentsPanel.jsx';
import DashboardCards from './components/DashboardCards.jsx';
import EmployeesPanel from './components/EmployeesPanel.jsx';
import ResourcesPanel from './components/ResourcesPanel.jsx';
import LoginPanel from './components/LoginPanel.jsx';
import * as api from './services/apiClient';

const SESSION_KEY = 'teamsift-lite-session';

function App() {
  const [activeTab, setActiveTab] = useState('dashboard');
  const [summary, setSummary] = useState(null);
  const [warehouseRoster, setWarehouseRoster] = useState(null);
  const [employeePage, setEmployeePage] = useState(null);
  const [resourcePage, setResourcePage] = useState(null);
  const [status, setStatus] = useState('');
  const [session, setSession] = useState(() => {
    const raw = localStorage.getItem(SESSION_KEY);
    return raw ? JSON.parse(raw) : { isAuthenticated: false, email: null, token: null };
  });

  const isAuthenticated = session?.isAuthenticated;

  useEffect(() => {
    if (isAuthenticated) {
      hydrateDashboard();
    }
  }, [isAuthenticated]);

  useEffect(() => {
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
  }, [session]);

  const hydrateDashboard = async () => {
    setStatus('');
    try {
      const [summaryResult, rosterResult] = await Promise.all([
        api.getWorkforceSummary(),
        api.getWarehouseEmployees()
      ]);
      setSummary(summaryResult);
      setWarehouseRoster(rosterResult);
    } catch (error) {
      setStatus(error.message);
    }
  };

  const handleLogin = ({ email, token }) => {
    setSession({ isAuthenticated: true, email, token });
    setActiveTab('dashboard');
  };

  const handleLogout = () => {
    setSession({ isAuthenticated: false, email: null, token: null });
    setSummary(null);
    setWarehouseRoster(null);
    setEmployeePage(null);
    setResourcePage(null);
    setStatus('');
  };

  const employees = employeePage?.data ?? [];
  const resources = resourcePage?.data ?? [];

  const userLabel = useMemo(() => {
    return session?.email ? `Signed in as ${session.email}` : '';
  }, [session]);

  if (!isAuthenticated) {
    return <LoginPanel onLogin={handleLogin} />;
  }

  return (
    <div className="app">
      <header className="app-header">
        <div>
          <h1>TeamShift Lite: Warehouse Dashboard</h1>
          <p>ReqRes-backed workforce and item management with simple in-app login.</p>
        </div>
        <div className="nav-actions">
          {userLabel && <span className="muted">{userLabel}</span>}
          <button onClick={hydrateDashboard}>Refresh Metrics</button>
          <button onClick={handleLogout}>Logout</button>
        </div>
      </header>

      <nav className="tabs">
        {['dashboard', 'employees', 'resources', 'assignments'].map((tab) => (
          <button
            key={tab}
            className={activeTab === tab ? 'active' : ''}
            onClick={() => setActiveTab(tab)}
          >
            {tab.charAt(0).toUpperCase() + tab.slice(1)}
          </button>
        ))}
      </nav>

      <main>
        {activeTab === 'dashboard' && (
          <DashboardCards summary={summary} roster={warehouseRoster} />
        )}
        {activeTab === 'employees' && (
          <EmployeesPanel onUsersLoaded={setEmployeePage} />
        )}
        {activeTab === 'resources' && (
          <ResourcesPanel onResourcesLoaded={setResourcePage} />
        )}
        {activeTab === 'assignments' && (
          <AssignmentsPanel employees={employees} resources={resources} />
        )}

        {status && <div className="status error">{status}</div>}
      </main>
    </div>
  );
}

export default App;
