// Prefer an explicit base URL, then fall back to the current host (useful when the
// SPA is served by the ASP.NET site), and finally default to the HTTPS profile
// Visual Studio uses (https://localhost:7216).
const explicitBase = import.meta.env.VITE_API_BASE_URL;
const sameHostBase = typeof window !== 'undefined' ? `${window.location.origin}/api` : null;
const API_BASE = (explicitBase || sameHostBase || 'https://localhost:7216/api').replace(/\/$/, '');

function buildHeaders(extra = {}) {
  return {
    'Content-Type': 'application/json',
    ...extra
  };
}

async function handleResponse(response) {
  if (response.status === 204) {
    return null;
  }

  const text = await response.text();
  let payload = null;

  if (text) {
    try {
      payload = JSON.parse(text);
    } catch {
      payload = { message: text };
    }
  }

  if (!response.ok) {
    const message = payload?.message || payload?.error || response.statusText;
    throw new Error(message);
  }

  return payload;
}

async function request(path, options = {}) {
  const response = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers: buildHeaders(options.headers)
  });

  return handleResponse(response);
}

export async function getWorkforceSummary(page = 1, perPage = 6) {
  return request(`/warehouse/workforce-summary?page=${page}&per_page=${perPage}`);
}

export async function getWarehouseEmployees(page = 1, perPage = 6) {
  return request(`/warehouse/employees?page=${page}&per_page=${perPage}`);
}

export async function getEmployees(page = 1, perPage = 6) {
  return request(`/employees?page=${page}&per_page=${perPage}`);
}

export async function getEmployee(id) {
  return request(`/employees/${id}`);
}

export async function createEmployee(payload) {
  return request('/employees', {
    method: 'POST',
    body: JSON.stringify(payload)
  });
}

export async function updateEmployee(id, payload) {
  return request(`/employees/${id}`, {
    method: 'PUT',
    body: JSON.stringify(payload)
  });
}

export async function patchEmployee(id, payload) {
  return request(`/employees/${id}`, {
    method: 'PATCH',
    body: JSON.stringify(payload)
  });
}

export async function deleteEmployee(id) {
  return request(`/employees/${id}`, {
    method: 'DELETE'
  });
}

export async function getResources(page = 1, perPage = 6) {
  return request(`/items?page=${page}&per_page=${perPage}`);
}

export async function getResource(id) {
  return request(`/items/${id}`);
}

export async function getAssociates() {
  return request('/warehouse/assignments');
}

export async function promoteAssociate(userId, role = 'Associate') {
  return request(`/warehouse/assignments/promote/${userId}?role=${encodeURIComponent(role)}`, {
    method: 'POST'
  });
}

export async function assignItems(userId, itemIds) {
  return request(`/warehouse/assignments/${userId}/items`, {
    method: 'POST',
    body: JSON.stringify({ itemIds })
  });
}
