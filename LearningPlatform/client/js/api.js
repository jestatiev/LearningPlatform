// Osnovna adresa backend API-ja - promijeni ako backend radi na drugom portu
const API_BASE = "http://localhost:5080/api";

function getToken() {
  return localStorage.getItem("token");
}

function getRoles() {
  return JSON.parse(localStorage.getItem("roles") || "[]");
}

function isLoggedIn() {
  return !!getToken();
}

function isAdmin() {
  return getRoles().includes("Admin");
}

function logout() {
  localStorage.removeItem("token");
  localStorage.removeItem("roles");
  localStorage.removeItem("username");
}

async function apiFetch(path, { method = "GET", body = null, auth = true } = {}) {
  const headers = { "Content-Type": "application/json" };
  if (auth && getToken()) {
    headers["Authorization"] = `Bearer ${getToken()}`;
  }

  const response = await fetch(`${API_BASE}${path}`, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  });

  if (response.status === 204) return null;

  const text = await response.text();
  const data = text ? JSON.parse(text) : null;

  if (!response.ok) {
    const message = data?.message || `Greška ${response.status}`;
    throw new Error(message);
  }
  return data;
}

const Api = {
  register: (dto) => apiFetch("/auth/register", { method: "POST", body: dto, auth: false }),
  login: (dto) => apiFetch("/auth/login", { method: "POST", body: dto, auth: false }),

  getCategories: () => apiFetch("/categories", { auth: false }),
  createCategory: (dto) => apiFetch("/categories", { method: "POST", body: dto }),
  deleteCategory: (id) => apiFetch(`/categories/${id}`, { method: "DELETE" }),

  getCourses: (categoryId) => apiFetch(`/courses${categoryId ? `?categoryId=${categoryId}` : ""}`, { auth: false }),
  getCourse: (id) => apiFetch(`/courses/${id}`, { auth: false }),
  createCourse: (dto) => apiFetch("/courses", { method: "POST", body: dto }),
  updateCourse: (id, dto) => apiFetch(`/courses/${id}`, { method: "PUT", body: dto }),
  deleteCourse: (id) => apiFetch(`/courses/${id}`, { method: "DELETE" }),

  getLessonsByCourse: (courseId) => apiFetch(`/lessons/by-course/${courseId}`, { auth: false }),
  createLesson: (dto) => apiFetch("/lessons", { method: "POST", body: dto }),
  deleteLesson: (id) => apiFetch(`/lessons/${id}`, { method: "DELETE" }),

  getMyEnrollments: () => apiFetch("/enrollments/my"),
  enroll: (courseId) => apiFetch("/enrollments", { method: "POST", body: { courseId } }),
  updateProgress: (id, dto) => apiFetch(`/enrollments/${id}/progress`, { method: "PUT", body: dto }),
  unenroll: (id) => apiFetch(`/enrollments/${id}`, { method: "DELETE" }),

  getTestsByCourse: (courseId) => apiFetch(`/tests/by-course/${courseId}`),
  getTest: (id) => apiFetch(`/tests/${id}`),
  createTest: (dto) => apiFetch("/tests", { method: "POST", body: dto }),
  addQuestion: (dto) => apiFetch("/tests/questions", { method: "POST", body: dto }),
  submitTest: (id, dto) => apiFetch(`/tests/${id}/submit`, { method: "POST", body: dto }),

  getReviewsByCourse: (courseId) => apiFetch(`/reviews/by-course/${courseId}`, { auth: false }),
  createReview: (dto) => apiFetch("/reviews", { method: "POST", body: dto }),
  deleteReview: (id) => apiFetch(`/reviews/${id}`, { method: "DELETE" }),

  getMe: () => apiFetch("/users/me"),
  getAllUsers: () => apiFetch("/users"),
};
