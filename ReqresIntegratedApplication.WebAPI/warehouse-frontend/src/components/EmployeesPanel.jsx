import React, { useEffect, useMemo, useState } from 'react';
  import {
    createEmployee,
    getEmployee,
    getEmployees,
    patchEmployee,
    updateEmployee,
    deleteEmployee
  } from '../services/apiClient';

function EmployeesPanel({ onUsersLoaded }) {
  const [page, setPage] = useState(1);
  const [perPage, setPerPage] = useState(6);
  const [roster, setRoster] = useState(null);
  const [selectedUser, setSelectedUser] = useState(null);
  const [createForm, setCreateForm] = useState({ name: 'New Hire', job: 'Warehouse Operator' });
  const [updateForm, setUpdateForm] = useState({ name: '', job: '' });
  const [status, setStatus] = useState('');

  const users = useMemo(() => roster?.data || [], [roster]);

  const loadUsers = async () => {
    try {
      const data = await getEmployees(page, perPage);
      setRoster(data);
      if (onUsersLoaded) {
        onUsersLoaded(data);
      }
    } catch (error) {
      setStatus(error.message);
    }
  };

  useEffect(() => {
    loadUsers();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, perPage]);

  const handleSelect = async (id) => {
    setSelectedUser(null);
    setStatus('');
    try {
      const detail = await getEmployee(id);
      setSelectedUser(detail);
      setUpdateForm({
        name: `${detail.first_name} ${detail.last_name}`.trim(),
        job: detail.last_name || 'Updated Job'
      });
    } catch (error) {
      setStatus(error.message);
    }
  };

  const handleCreate = async (evt) => {
    evt.preventDefault();
    setStatus('');
    try {
      const created = await createEmployee(createForm);
      setStatus(`Created user ${created.id} at ${created.createdAt}`);
      await loadUsers();
    } catch (error) {
      setStatus(error.message);
    }
  };

  const handlePut = async () => {
    if (!selectedUser) return;
    setStatus('');
    try {
      const updated = await updateEmployee(selectedUser.id, updateForm);
      setStatus(`PUT saved at ${updated.updatedAt}`);
      await loadUsers();
      await handleSelect(selectedUser.id);
    } catch (error) {
      setStatus(error.message);
    }
  };

  const handlePatch = async () => {
    if (!selectedUser) return;
    setStatus('');
    try {
      const updated = await patchEmployee(selectedUser.id, { job: updateForm.job });
      setStatus(`PATCH saved at ${updated.updatedAt}`);
      await loadUsers();
      await handleSelect(selectedUser.id);
    } catch (error) {
      setStatus(error.message);
    }
  };

  const handleDelete = async () => {
    if (!selectedUser) return;
    setStatus('');
    try {
      await deleteEmployee(selectedUser.id);
      setStatus(`Deleted user #${selectedUser.id}`);
      setSelectedUser(null);
      await loadUsers();
    } catch (error) {
      setStatus(error.message);
    }
  };

  return (
    <div className="panel">
      <h2>Employees</h2>
      <p>Lists and edits warehouse employees through the ReqRes <code>/users</code> endpoints.</p>

      <div className="toolbar">
        <div className="toolbar-item">
          <label>Page
            <input type="number" value={page} min="1" onChange={(e) => setPage(Number(e.target.value) || 1)} />
          </label>
        </div>
        <div className="toolbar-item">
          <label>Per Page
            <input type="number" value={perPage} min="1" onChange={(e) => setPerPage(Number(e.target.value) || 1)} />
          </label>
        </div>
        <button type="button" onClick={loadUsers}>Refresh List</button>
      </div>

      <div className="grid two-col">
        <div>
          <h3>Roster</h3>
          <div className="table">
            <div className="table-head">
              <div>ID</div>
              <div>Name</div>
              <div>Email</div>
            </div>
            {users.map((user) => (
              <button key={user.id} className={`table-row ${selectedUser?.id === user.id ? 'active' : ''}`} onClick={() => handleSelect(user.id)}>
                <div>#{user.id}</div>
                <div>{user.first_name} {user.last_name}</div>
                <div>{user.email}</div>
              </button>
            ))}
          </div>
        </div>

        <div>
          <h3>Details & Updates</h3>
          {selectedUser ? (
            <div className="detail-card">
              <div className="detail-row"><strong>ID</strong> {selectedUser.id}</div>
              <div className="detail-row"><strong>Name</strong> {selectedUser.first_name} {selectedUser.last_name}</div>
              <div className="detail-row"><strong>Email</strong> {selectedUser.email}</div>
              {selectedUser.avatar && <img src={selectedUser.avatar} alt="avatar" className="avatar" />}

              <label>
                Full Name
                <input value={updateForm.name} onChange={(e) => setUpdateForm({ ...updateForm, name: e.target.value })} />
              </label>
              <label>
                Job Title
                <input value={updateForm.job} onChange={(e) => setUpdateForm({ ...updateForm, job: e.target.value })} />
              </label>

              <div className="button-row">
                <button type="button" className="primary" onClick={handlePut}>Save (PUT)</button>
                <button type="button" onClick={handlePatch}>Quick Update (PATCH)</button>
                <button type="button" className="danger" onClick={handleDelete}>Delete</button>
              </div>
            </div>
          ) : (
            <p>Select a user to see details and enable PUT/PATCH actions.</p>
          )}
        </div>
      </div>

      <div className="divider" />

      <h3>Create Employee (POST)</h3>
      <form className="form inline" onSubmit={handleCreate}>
        <label>Name
          <input value={createForm.name} onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })} required />
        </label>
        <label>Job Title
          <input value={createForm.job} onChange={(e) => setCreateForm({ ...createForm, job: e.target.value })} required />
        </label>
        <button type="submit" className="primary">Create (POST)</button>
      </form>

      {status && <div className="status info">{status}</div>}
    </div>
  );
}

export default EmployeesPanel;
