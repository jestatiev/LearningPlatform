let allCategories = [];

// ---------- Navigacija ----------
function showView(name) {
  document.querySelectorAll(".view").forEach((v) => v.classList.remove("active"));
  document.getElementById(`view-${name}`).classList.add("active");
  document.querySelectorAll("#mainNav button[data-view]").forEach((b) => b.classList.remove("active"));
  const navBtn = document.querySelector(`#mainNav button[data-view="${name}"]`);
  if (navBtn) navBtn.classList.add("active");

  if (name === "courses") loadCourses();
  if (name === "myEnrollments") loadMyEnrollments();
  if (name === "admin") loadAdminData();
}

document.querySelectorAll("#mainNav button[data-view]").forEach((btn) => {
  btn.addEventListener("click", () => showView(btn.dataset.view));
});

document.getElementById("authNavBtn").addEventListener("click", () => showView("auth"));
document.getElementById("logoutBtn").addEventListener("click", () => {
  logout();
  refreshAuthUi();
  showView("courses");
});

document.querySelectorAll("[data-authtab]").forEach((btn) => {
  btn.addEventListener("click", () => {
    document.querySelectorAll("[data-authtab]").forEach((b) => b.classList.remove("active"));
    btn.classList.add("active");
    document.getElementById("loginCard").style.display = btn.dataset.authtab === "login" ? "block" : "none";
    document.getElementById("registerCard").style.display = btn.dataset.authtab === "register" ? "block" : "none";
  });
});

document.querySelectorAll("[data-admintab]").forEach((btn) => {
  btn.addEventListener("click", () => {
    document.querySelectorAll("[data-admintab]").forEach((b) => b.classList.remove("active"));
    btn.classList.add("active");
    ["categories", "courses", "lessons", "tests"].forEach((t) => {
      document.getElementById(`admin-${t}`).style.display = t === btn.dataset.admintab ? "block" : "none";
    });
  });
});

function refreshAuthUi() {
  const loggedIn = isLoggedIn();
  document.querySelectorAll(".authOnly").forEach((el) => (el.style.display = loggedIn ? "inline-block" : "none"));
  document.querySelectorAll(".adminOnly").forEach((el) => (el.style.display = loggedIn && isAdmin() ? "inline-block" : "none"));
  document.getElementById("authNavBtn").style.display = loggedIn ? "none" : "inline-block";
  document.getElementById("logoutBtn").style.display = loggedIn ? "inline-block" : "none";
  document.getElementById("usernameLabel").textContent = localStorage.getItem("username") || "";
}

// ---------- Auth ----------
async function handleLogin() {
  const username = document.getElementById("loginUsername").value;
  const password = document.getElementById("loginPassword").value;
  const msg = document.getElementById("loginMsg");
  try {
    const res = await Api.login({ username, password });
    localStorage.setItem("token", res.token);
    localStorage.setItem("roles", JSON.stringify(res.roles));
    localStorage.setItem("username", res.username);
    msg.innerHTML = `<p class="success">Uspješna prijava!</p>`;
    refreshAuthUi();
    showView("courses");
  } catch (e) {
    msg.innerHTML = `<p class="error">${e.message}</p>`;
  }
}

async function handleRegister() {
  const dto = {
    username: document.getElementById("regUsername").value,
    email: document.getElementById("regEmail").value,
    fullName: document.getElementById("regFullName").value,
    password: document.getElementById("regPassword").value,
  };
  const msg = document.getElementById("registerMsg");
  try {
    const res = await Api.register(dto);
    localStorage.setItem("token", res.token);
    localStorage.setItem("roles", JSON.stringify(res.roles));
    localStorage.setItem("username", res.username);
    msg.innerHTML = `<p class="success">Registracija uspješna, prijavljeni ste!</p>`;
    refreshAuthUi();
    showView("courses");
  } catch (e) {
    msg.innerHTML = `<p class="error">${e.message}</p>`;
  }
}

// ---------- Kategorije (za filter i formu) ----------
async function loadCategoriesIntoSelectors() {
  allCategories = await Api.getCategories();
  const filter = document.getElementById("categoryFilter");
  const courseCat = document.getElementById("newCourseCategory");
  filter.innerHTML = `<option value="">Sve kategorije</option>`;
  if (courseCat) courseCat.innerHTML = "";
  allCategories.forEach((c) => {
    filter.innerHTML += `<option value="${c.id}">${c.name}</option>`;
    if (courseCat) courseCat.innerHTML += `<option value="${c.id}">${c.name}</option>`;
  });
}

// ---------- Tečajevi ----------
async function loadCourses() {
  await loadCategoriesIntoSelectors();
  const categoryId = document.getElementById("categoryFilter").value;
  const courses = await Api.getCourses(categoryId || null);
  const grid = document.getElementById("coursesGrid");
  grid.innerHTML = courses
    .map(
      (c) => `
    <div class="card">
      <h3>${c.title}</h3>
      <p class="muted">${c.categoryName} • ${c.instructorName}</p>
      <p>⭐ ${c.averageRating || "N/A"} &nbsp; 💶 ${c.price.toFixed(2)} €</p>
      <button class="btn btn-primary btn-small" onclick="openCourse(${c.id})">Otvori</button>
    </div>`
    )
    .join("") || "<p class='muted'>Nema tečajeva.</p>";
}

async function openCourse(id) {
  const c = await Api.getCourse(id);
  let tests = [];
  if (isLoggedIn()) {
    try { tests = await Api.getTestsByCourse(id); } catch { tests = []; }
  }

  let enrolled = false;
  let myEnrollmentId = null;
  if (isLoggedIn() && !isAdmin()) {
    const mine = await Api.getMyEnrollments();
    const match = mine.find((e) => e.courseId === id);
    enrolled = !!match;
    myEnrollmentId = match?.id;
  }

  const html = `
    <div class="card">
      <h2>${c.title} <span class="badge">${c.categoryName}</span></h2>
      <p class="muted">Predavač: ${c.instructorName} • Cijena: ${c.price.toFixed(2)} € • ⭐ ${c.averageRating} (${c.reviews.length} recenzija) • ${c.enrollmentCount} prijavljenih</p>
      <p>${c.description}</p>
      ${
        isLoggedIn() && !isAdmin()
          ? enrolled
            ? `<p class="success">Prijavljeni ste na ovaj tečaj.</p>`
            : `<button class="btn btn-primary" onclick="enrollInCourse(${id})">Prijavi se na tečaj</button>`
          : !isLoggedIn()
          ? `<p class="muted">Prijavite se kako biste se upisali na tečaj.</p>`
          : ""
      }
    </div>

    <div class="card">
      <h3>Lekcije</h3>
      ${
        c.lessons.length
          ? "<ol>" + c.lessons.map((l) => `<li><b>${l.title}</b> - ${l.content}</li>`).join("") + "</ol>"
          : "<p class='muted'>Još nema lekcija.</p>"
      }
    </div>

    ${
      isLoggedIn()
        ? `<div class="card">
      <h3>Testovi</h3>
      ${
        tests.length
          ? tests.map((t) => `
              <div class="card">
                <b>${t.title}</b> (${t.questions.length} pitanja)
                ${!isAdmin() ? `<button class="btn btn-small btn-primary" onclick="openTest(${t.id})">Riješi test</button>` : ""}
                <div id="test-${t.id}-area"></div>
              </div>`).join("")
          : "<p class='muted'>Za ovaj tečaj još nema testova.</p>"
      }
    </div>`
        : ""
    }

    <div class="card">
      <h3>Recenzije</h3>
      ${
        c.reviews.length
          ? c.reviews.map((r) => `<p><b>${r.username}</b> - ⭐ ${r.rating}: ${r.comment || ""}</p>`).join("")
          : "<p class='muted'>Još nema recenzija.</p>"
      }
      ${
        isLoggedIn() && enrolled
          ? `<hr/>
            <label>Ocjena (1-5)</label><input id="reviewRating" type="number" min="1" max="5" value="5" />
            <label>Komentar</label><textarea id="reviewComment" rows="2"></textarea>
            <button class="btn btn-primary btn-small" onclick="submitReview(${id})">Pošalji recenziju</button>
            <div id="reviewMsg"></div>`
          : ""
      }
    </div>
  `;

  document.getElementById("courseDetailsContent").innerHTML = html;
  showView("courseDetails");
}

async function enrollInCourse(courseId) {
  try {
    await Api.enroll(courseId);
    openCourse(courseId);
  } catch (e) {
    alert(e.message);
  }
}

async function submitReview(courseId) {
  const rating = parseInt(document.getElementById("reviewRating").value, 10);
  const comment = document.getElementById("reviewComment").value;
  try {
    await Api.createReview({ rating, comment, courseId });
    openCourse(courseId);
  } catch (e) {
    document.getElementById("reviewMsg").innerHTML = `<p class="error">${e.message}</p>`;
  }
}

// ---------- Testovi (rješavanje) ----------
async function openTest(testId) {
  const test = await Api.getTest(testId);
  const area = document.getElementById(`test-${testId}-area`);
  area.innerHTML =
    test.questions
      .map(
        (q, i) => `
      <div>
        <p><b>${i + 1}. ${q.text}</b></p>
        <label><input type="radio" name="q${q.id}" value="A" /> A) ${q.optionA}</label><br/>
        <label><input type="radio" name="q${q.id}" value="B" /> B) ${q.optionB}</label><br/>
        <label><input type="radio" name="q${q.id}" value="C" /> C) ${q.optionC}</label><br/>
        <label><input type="radio" name="q${q.id}" value="D" /> D) ${q.optionD}</label>
      </div>`
      )
      .join("") +
    `<button class="btn btn-primary btn-small" onclick='submitTest(${testId}, ${JSON.stringify(test.questions.map((q) => q.id))})'>Predaj test</button>
     <div id="test-${testId}-result"></div>`;
}

async function submitTest(testId, questionIds) {
  const answers = questionIds.map((qid) => {
    const selected = document.querySelector(`input[name="q${qid}"]:checked`);
    return { questionId: qid, selectedOption: selected ? selected.value : "X" };
  });
  try {
    const result = await Api.submitTest(testId, { answers });
    document.getElementById(`test-${testId}-result`).innerHTML =
      `<p class="success">Rezultat: ${result.correctAnswers}/${result.totalQuestions} (${result.scorePercent}%)</p>`;
  } catch (e) {
    document.getElementById(`test-${testId}-result`).innerHTML = `<p class="error">${e.message}</p>`;
  }
}

// ---------- Moji tečajevi ----------
async function loadMyEnrollments() {
  const list = await Api.getMyEnrollments();
  const container = document.getElementById("myEnrollmentsList");
  container.innerHTML =
    list
      .map(
        (e) => `
    <div class="card">
      <h3>${e.courseTitle}</h3>
      <div class="progress-bar"><div style="width:${e.progressPercent}%"></div></div>
      <p class="muted">Napredak: ${e.progressPercent}% ${e.isCompleted ? "✅ Završeno" : ""}</p>
      <button class="btn btn-small" onclick="updateProgress(${e.id}, ${Math.min(e.progressPercent + 10, 100)})">+10% napretka</button>
      <button class="btn btn-small btn-danger" onclick="unenroll(${e.id})">Odjavi se</button>
    </div>`
      )
      .join("") || "<p class='muted'>Niste prijavljeni ni na jedan tečaj.</p>";
}

async function updateProgress(id, newProgress) {
  await Api.updateProgress(id, { progressPercent: newProgress, isCompleted: newProgress >= 100 });
  loadMyEnrollments();
}

async function unenroll(id) {
  await Api.unenroll(id);
  loadMyEnrollments();
}

// ---------- Admin ----------
async function loadAdminData() {
  await loadCategoriesIntoSelectors();

  const catList = document.getElementById("adminCategoriesList");
  catList.innerHTML = allCategories
    .map((c) => `<p>${c.name} (${c.courseCount} tečajeva) <button class="btn btn-small btn-danger" onclick="deleteCategory(${c.id})">Obriši</button></p>`)
    .join("");

  const courses = await Api.getCourses(null);
  const courseList = document.getElementById("adminCoursesList");
  courseList.innerHTML = courses
    .map((c) => `<p>${c.title} <button class="btn btn-small btn-danger" onclick="deleteCourse(${c.id})">Obriši</button></p>`)
    .join("");

  const lessonCourseSelect = document.getElementById("newLessonCourse");
  const testCourseSelect = document.getElementById("newTestCourse");
  const options = courses.map((c) => `<option value="${c.id}">${c.title}</option>`).join("");
  lessonCourseSelect.innerHTML = options;
  testCourseSelect.innerHTML = options;
}

async function createCategory() {
  const name = document.getElementById("newCategoryName").value;
  const description = document.getElementById("newCategoryDesc").value;
  await Api.createCategory({ name, description });
  document.getElementById("newCategoryName").value = "";
  document.getElementById("newCategoryDesc").value = "";
  loadAdminData();
}

async function deleteCategory(id) {
  await Api.deleteCategory(id);
  loadAdminData();
}

async function createCourse() {
  const dto = {
    title: document.getElementById("newCourseTitle").value,
    description: document.getElementById("newCourseDesc").value,
    instructorName: document.getElementById("newCourseInstructor").value,
    price: parseFloat(document.getElementById("newCoursePrice").value),
    categoryId: parseInt(document.getElementById("newCourseCategory").value, 10),
  };
  await Api.createCourse(dto);
  ["newCourseTitle", "newCourseDesc", "newCourseInstructor", "newCoursePrice"].forEach((id) => (document.getElementById(id).value = ""));
  loadAdminData();
}

async function deleteCourse(id) {
  await Api.deleteCourse(id);
  loadAdminData();
}

async function createLesson() {
  const dto = {
    courseId: parseInt(document.getElementById("newLessonCourse").value, 10),
    title: document.getElementById("newLessonTitle").value,
    content: document.getElementById("newLessonContent").value,
    videoUrl: null,
    orderIndex: parseInt(document.getElementById("newLessonOrder").value, 10),
  };
  await Api.createLesson(dto);
  document.getElementById("newLessonTitle").value = "";
  document.getElementById("newLessonContent").value = "";
  alert("Lekcija dodana.");
}

async function createTest() {
  const dto = {
    courseId: parseInt(document.getElementById("newTestCourse").value, 10),
    title: document.getElementById("newTestTitle").value,
  };
  const created = await Api.createTest(dto);
  document.getElementById("newTestTitle").value = "";
  alert(`Test kreiran, ID = ${created.id}. Koristi taj ID za dodavanje pitanja.`);
}

async function addQuestion() {
  const dto = {
    testId: parseInt(document.getElementById("newQuestionTestId").value, 10),
    text: document.getElementById("newQuestionText").value,
    optionA: document.getElementById("newQuestionA").value,
    optionB: document.getElementById("newQuestionB").value,
    optionC: document.getElementById("newQuestionC").value,
    optionD: document.getElementById("newQuestionD").value,
    correctOption: document.getElementById("newQuestionCorrect").value,
  };
  await Api.addQuestion(dto);
  ["newQuestionText", "newQuestionA", "newQuestionB", "newQuestionC", "newQuestionD"].forEach((id) => (document.getElementById(id).value = ""));
  alert("Pitanje dodano.");
}

// ---------- Init ----------
refreshAuthUi();
loadCourses();
