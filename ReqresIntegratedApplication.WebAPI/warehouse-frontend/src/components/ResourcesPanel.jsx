import React, { useEffect, useMemo, useState } from 'react';
import { createResource, getResource, getResources, updateResource } from '../services/apiClient';

function ResourcesPanel({ onResourcesLoaded }) {
  const [page, setPage] = useState(1);
  const [perPage, setPerPage] = useState(6);
  const [resourcePage, setResourcePage] = useState(null);
  const [selected, setSelected] = useState(null);
  const [status, setStatus] = useState('');
  const [form, setForm] = useState({
    name: '',
    color: '#0099cc',
    year: new Date().getFullYear(),
    pantoneValue: '',
    quantity: 0
  });

  const resources = useMemo(() => resourcePage?.data || [], [resourcePage]);

  const loadResources = async () => {
    try {
      const data = await getResources(page, perPage);
      setResourcePage(data);
      if (onResourcesLoaded) {
        onResourcesLoaded(data);
      }
    } catch (error) {
      setStatus(error.message);
    }
  };

  useEffect(() => {
    loadResources();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, perPage]);

  const handleSelect = async (id) => {
    setStatus('');
    try {
      const detail = await getResource(id);
      setSelected(detail);
      setForm({
        name: detail.name || '',
        color: detail.color || '#0099cc',
        year: detail.year || new Date().getFullYear(),
        pantoneValue: detail.pantone_value || '',
        quantity: detail.quantity ?? 0
      });
    } catch (error) {
      setStatus(error.message);
    }
  };

  const handleFormChange = (field, value) => {
    setForm((prev) => ({ ...prev, [field]: value }));
  };

  const handleCreate = async () => {
    setStatus('');
    try {
      await createResource({
        name: form.name,
        color: form.color,
        year: Number(form.year) || 0,
        pantoneValue: form.pantoneValue,
        quantity: Number(form.quantity) || 0
      });
      setForm({
        name: '',
        color: '#0099cc',
        year: new Date().getFullYear(),
        pantoneValue: '',
        quantity: 0
      });
      await loadResources();
      setStatus('Resource added locally.');
    } catch (error) {
      setStatus(error.message);
    }
  };

  const handleUpdate = async () => {
    if (!selected) {
      setStatus('Select a resource to edit.');
      return;
    }

    setStatus('');
    try {
      const updated = await updateResource(selected.id, {
        name: form.name,
        color: form.color,
        year: Number(form.year) || 0,
        pantoneValue: form.pantoneValue,
        quantity: Number(form.quantity) || 0
      });
      setSelected(updated);
      await loadResources();
      setStatus('Resource updated.');
    } catch (error) {
      setStatus(error.message);
    }
  };

  return (
    <div className="panel">
      <h2>Resources &amp; Tools</h2>
      <p>Surfaced from the ReqRes <code>/unknown</code> endpoint and re-labeled as warehouse items.</p>

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
        <button type="button" onClick={loadResources}>Refresh</button>
      </div>

      <div className="grid two-col">
        <div>
          <div className="table">
            <div className="table-head">
              <div>ID</div>
              <div>Name</div>
              <div>Color</div>
              <div>Qty</div>
            </div>
            {resources.map((r) => (
              <button key={r.id} className={`table-row ${selected?.id === r.id ? 'active' : ''}`} style={{ borderLeftColor: r.color }} onClick={() => handleSelect(r.id)}>
                <div>#{r.id}</div>
                <div>{r.name}</div>
                <div>{r.color}</div>
                <div>{r.quantity ?? 0}</div>
              </button>
            ))}
          </div>
        </div>

        <div>
          <div className="detail-card">
            <div className="detail-row">
              <strong>ID</strong> {selected ? selected.id : 'New'}
            </div>
            <label className="detail-row">Name
              <input value={form.name} onChange={(e) => handleFormChange('name', e.target.value)} />
            </label>
            <label className="detail-row">Year
              <input type="number" value={form.year} onChange={(e) => handleFormChange('year', e.target.value)} />
            </label>
            <label className="detail-row">Color
              <input type="color" value={form.color} onChange={(e) => handleFormChange('color', e.target.value)} />
            </label>
            <label className="detail-row">Pantone
              <input value={form.pantoneValue} onChange={(e) => handleFormChange('pantoneValue', e.target.value)} placeholder="14-4121" />
            </label>
            <label className="detail-row">Quantity
              <input type="number" min="0" value={form.quantity} onChange={(e) => handleFormChange('quantity', e.target.value)} />
            </label>
            <div className="button-row">
              <button type="button" onClick={handleCreate}>Add Resource</button>
              <button type="button" onClick={handleUpdate} disabled={!selected}>Save Changes</button>
            </div>
          </div>
        </div>
      </div>

      {status && <div className="status error">{status}</div>}
    </div>
  );
}

export default ResourcesPanel;
