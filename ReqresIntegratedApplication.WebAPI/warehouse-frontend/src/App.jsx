import React, { useEffect, useState } from 'react';
import AssignmentsPanel from './components/AssignmentsPanel.jsx';
import DashboardCards from './components/DashboardCards.jsx';
import EmployeesPanel from './components/EmployeesPanel.jsx';
import LoginPanel from './components/LoginPanel.jsx';
import ResourcesPanel from './components/ResourcesPanel.jsx';
import * as api from './services/apiClient';
import { getToken as loadToken, setToken as storeToken } from './services/session';

function App() {
  const [token, setToken] = useState(loadToken());
  const [activeTab, setActiveTab] = useState('dashboard');
  const [summary, setSummary] = useState(null);
  const [warehouseRoster, setWarehouseRoster] = useState(null);
  const [employeePage, setEmployeePage] = useState(null);
  const [resourcePage, setResourcePage] = useState(null);
  const [status, setStatus] = useState('');

  useEffect(() => {
    api.setToken(token);
    storeToken(token);
    if (token) {
      hydrateDashboard();
    }
  }, [token]);

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

  const handleLogout = async () => {
    await api.logout();
    setToken(null);
    setActiveTab('dashboard');
    setSummary(null);
    setWarehouseRoster(null);
    setEmployeePage(null);
    setResourcePage(null);
  };

  if (!token) {
    return (
      <div className="app">
        <header className="app-header">
          <div>
            <h1>TeamShift Lite: Warehouse Dashboard</h1>
            <p>Sign in with the local demo credentials to unlock the workforce and inventory screens.</p>
          </div>
        </header>
        <main>
          <LoginPanel onAuthenticated={setToken} />
        </main>
      </div>
    );
  }

  const employees = employeePage?.data ?? [];
  const resources = resourcePage?.data ?? [];

  return (
    <div className="app">
      <header className="app-header">
        <div>
          <h1>TeamShift Lite: Warehouse Dashboard</h1>
          <p>
            ReqRes-backed workforce and item management. Login is validated locally to avoid external 401s;
            the token is stored in-memory only.
          </p>
        </div>
        <div className="nav-actions">
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
