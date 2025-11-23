import React, { useEffect, useState } from 'react';
import { assignItems, getAssociates, promoteAssociate } from '../services/apiClient';

function AssignmentsPanel({ employees = [], resources = [] }) {
  const [associates, setAssociates] = useState([]);
  const [selectedUserId, setSelectedUserId] = useState('');
  const [role, setRole] = useState('Associate');
  const [selectedAssociateId, setSelectedAssociateId] = useState(null);
  const [selectedItemIds, setSelectedItemIds] = useState([]);
  const [status, setStatus] = useState('');

  const loadAssociates = async (nextSelectedId) => {
    try {
      const data = await getAssociates();
      const list = data || [];
      setAssociates(list);
      const fallbackId = nextSelectedId ?? selectedAssociateId;
      if (!fallbackId && list.length > 0) {
        setSelectedAssociateId(list[0].userId);
      } else if (fallbackId) {
        setSelectedAssociateId(fallbackId);
      }
    } catch (error) {
      setStatus(error.message);
    }
  };

  useEffect(() => {
    loadAssociates();
  }, []);

  useEffect(() => {
    if (selectedAssociateId) {
      const associate = associates.find((a) => a.userId === selectedAssociateId);
      setSelectedItemIds(associate?.assignedItemIds || []);
    }
  }, [associates, selectedAssociateId]);

  const handlePromote = async () => {
    if (!selectedUserId) return;
    setStatus('');
    try {
      const id = Number(selectedUserId);
      await promoteAssociate(id, role);
      await loadAssociates(id);
      const promoted = associates.find((a) => a.userId === id);
      setSelectedItemIds(promoted?.assignedItemIds || []);
      setStatus('User promoted to warehouse associate.');
    } catch (error) {
      setStatus(error.message);
    }
  };

  const toggleItem = (id) => {
    setSelectedItemIds((current) => (
      current.includes(id) ? current.filter((x) => x !== id) : [...current, id]
    ));
  };

  const handleAssign = async () => {
    if (!selectedAssociateId) return;
    setStatus('');
    try {
      await assignItems(selectedAssociateId, selectedItemIds);
      await loadAssociates();
      setStatus('Items assigned to associate.');
    } catch (error) {
      setStatus(error.message);
    }
  };

  return (
    <div className="panel">
      <h2>Warehouse Assignments</h2>
      <p>Promote ReqRes users into associates and assign item IDs from the resources list. Everything is kept in memory by the API.</p>

      <div className="grid two-col">
        <div>
          <h3>Promote an Employee</h3>
          <label>Pick Employee
            <select value={selectedUserId} onChange={(e) => setSelectedUserId(e.target.value)}>
              <option value="">Choose…</option>
              {employees.map((u) => (
                <option key={u.id} value={u.id}>
                  #{u.id} {u.first_name} {u.last_name}
                </option>
              ))}
            </select>
          </label>
          <label>Role
            <input value={role} onChange={(e) => setRole(e.target.value)} />
          </label>
          <button type="button" className="primary" onClick={handlePromote}>Promote</button>
        </div>

        <div>
          <h3>Associates</h3>
          <div className="table">
            <div className="table-head">
              <div>ID</div>
              <div>Name</div>
              <div>Role</div>
            </div>
            {associates.map((a) => (
              <button key={a.userId} className={`table-row ${selectedAssociateId === a.userId ? 'active' : ''}`} onClick={() => setSelectedAssociateId(a.userId)}>
                <div>#{a.userId}</div>
                <div>{a.fullName}</div>
                <div>{a.role}</div>
              </button>
            ))}
          </div>
        </div>
      </div>

      <div className="divider" />

      <h3>Assign Items to Associate</h3>
      {selectedAssociateId ? (
        <div className="grid two-col">
          <div>
            <p>Check the items to associate with this worker. The server only keeps these assignments in memory.</p>
            <div className="table">
              <div className="table-head">
                <div>ID</div>
                <div>Name</div>
                <div>Assign</div>
              </div>
              {resources.map((item) => (
                <label key={item.id} className="table-row">
                  <div>#{item.id}</div>
                  <div>{item.name}</div>
                  <div>
                    <input
                      type="checkbox"
                      checked={selectedItemIds.includes(item.id)}
                      onChange={() => toggleItem(item.id)}
                    />
                  </div>
                </label>
              ))}
            </div>
          </div>

          <div>
            <div className="detail-card">
              <div className="detail-row"><strong>Associate</strong> #{selectedAssociateId}</div>
              <div className="detail-row"><strong>Items</strong> {selectedItemIds.length} selected</div>
              <button type="button" className="primary" onClick={handleAssign}>Save Assignments</button>
            </div>
          </div>
        </div>
      ) : (
        <p>Select an associate to manage assignments.</p>
      )}

      {status && <div className="status info">{status}</div>}
    </div>
  );
}

export default AssignmentsPanel;
