import React, { useEffect, useMemo, useState } from 'react';
import { getResource, getResources } from '../services/apiClient';

function ResourcesPanel({ onResourcesLoaded }) {
  const [page, setPage] = useState(1);
  const [perPage, setPerPage] = useState(6);
  const [resourcePage, setResourcePage] = useState(null);
  const [selected, setSelected] = useState(null);
  const [status, setStatus] = useState('');

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
            </div>
            {resources.map((r) => (
              <button key={r.id} className={`table-row ${selected?.id === r.id ? 'active' : ''}`} style={{ borderLeftColor: r.color }} onClick={() => handleSelect(r.id)}>
                <div>#{r.id}</div>
                <div>{r.name}</div>
                <div>{r.color}</div>
              </button>
            ))}
          </div>
        </div>

        <div>
          {selected ? (
            <div className="detail-card">
              <div className="detail-row"><strong>ID</strong> {selected.id}</div>
              <div className="detail-row"><strong>Name</strong> {selected.name}</div>
              <div className="detail-row"><strong>Year</strong> {selected.year}</div>
              <div className="detail-row"><strong>Color</strong> <span className="pill" style={{ background: selected.color }}>{selected.color}</span></div>
              <div className="detail-row"><strong>Pantone</strong> {selected.pantone_value}</div>
            </div>
          ) : (
            <p>Select an item to view details.</p>
          )}
        </div>
      </div>

      {status && <div className="status error">{status}</div>}
    </div>
  );
}

export default ResourcesPanel;
