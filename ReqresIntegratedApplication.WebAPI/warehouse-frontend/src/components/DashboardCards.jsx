import React from 'react';

function Stat({ label, value, helper }) {
  return (
    <div className="stat-card">
      <div className="stat-label">{label}</div>
      <div className="stat-value">{value}</div>
      {helper && <div className="stat-helper">{helper}</div>}
    </div>
  );
}

function DashboardCards({ summary, roster }) {
  const memberCount = roster?.members?.length ?? roster?.data?.length ?? 0;
  return (
    <div className="panel">
      <h2>Warehouse Snapshot</h2>
      <p>Quick metrics pulled from the ReqRes-powered API so you can see team size at a glance.</p>
      <div className="stat-grid">
        <Stat label="Members on page" value={summary?.countOnPage ?? memberCount} />
        <Stat label="Total members" value={summary?.totalMembers ?? roster?.total ?? 0} />
        <Stat label="Page" value={`${summary?.page ?? roster?.page ?? 1} of ${summary?.totalPages ?? roster?.totalPages ?? 1}`} />
        <Stat label="Per page" value={summary?.perPage ?? roster?.perPage ?? 6} helper="Adjust query string to test pagination." />
      </div>
    </div>
  );
}

export default DashboardCards;
