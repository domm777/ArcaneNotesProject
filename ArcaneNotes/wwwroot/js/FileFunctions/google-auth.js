function parseJwt(jwt) {
  const base64Url = jwt.split(".")[1];
  const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
  const jsonPayload = decodeURIComponent(
    atob(base64)
      .split("")
      .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
      .join(""),
  );
  return JSON.parse(jsonPayload);
}

function getCurrentUser() {
  const raw = localStorage.getItem("google_user");
  return raw ? JSON.parse(raw) : null;
}

function getUserNoteKey(email) {
  return `arcane_notes_${email}`;
}

function showLoggedInUI() {
  const loginSection = document.getElementById("google-login-section");
  const userProfile = document.getElementById("user-profile");

  if (loginSection) loginSection.style.display = "none";
  if (userProfile) userProfile.style.display = "block";
}

function showLoggedOutUI() {
  const loginSection = document.getElementById("google-login-section");
  const userProfile = document.getElementById("user-profile");

  if (loginSection) loginSection.style.display = "block";
  if (userProfile) userProfile.style.display = "none";
}

function setUserProfile(user) {
  const pic = document.getElementById("user-pic");
  const name = document.getElementById("user-name");
  const email = document.getElementById("user-email");

  if (name) name.textContent = user.name || "";
  if (email) email.textContent = user.email || "";

  if (pic) {
    if (user.picture) {
      pic.style.display = "block";
      pic.src = user.picture;
      pic.onerror = function () {
        this.style.display = "none";
      };
    } else {
      pic.style.display = "none";
    }
  }
}

function loadUserNotes() {
  const user = getCurrentUser();
  if (!user || !user.email) return;

  const saved = localStorage.getItem(getUserNoteKey(user.email));
  if (!saved) return;

  try {
    const data = JSON.parse(saved);
    if (
      window.ExportedFunctions &&
      typeof window.ExportedFunctions.LoadFromJsonData === "function"
    ) {
      window.ExportedFunctions.LoadFromJsonData(data);
    }
  } catch (err) {
    console.error("Failed to load saved notes:", err);
  }
}

function saveUserNotes() {
  const user = getCurrentUser();
  if (!user || !user.email) return;

  if (
    !window.ExportedFunctions ||
    typeof window.ExportedFunctions.GetCurrentNoteData !== "function"
  ) {
    return;
  }

  try {
    const data = window.ExportedFunctions.GetCurrentNoteData();
    localStorage.setItem(getUserNoteKey(user.email), JSON.stringify(data));
  } catch (err) {
    console.error("Failed to save notes:", err);
  }
}

window.handleGoogleCredential = function (response) {
  try {
    const user = parseJwt(response.credential);
    localStorage.setItem("google_user", JSON.stringify(user));

    showLoggedInUI();
    setUserProfile(user);

    setTimeout(loadUserNotes, 100);
  } catch (err) {
    console.error("Google login failed:", err);
  }
};

function loadGoogleUser() {
  const raw = localStorage.getItem("google_user");
  if (!raw) {
    showLoggedOutUI();
    return;
  }

  try {
    const user = JSON.parse(raw);
    showLoggedInUI();
    setUserProfile(user);

    setTimeout(loadUserNotes, 100);
  } catch (err) {
    console.error("Failed to load Google user:", err);
  }
}

function logoutGoogle() {
  localStorage.removeItem("google_user");
  showLoggedOutUI();
}

document.addEventListener("DOMContentLoaded", () => {
  loadGoogleUser();

  const logoutBtn = document.getElementById("google-logout-btn");
  if (logoutBtn) {
    logoutBtn.addEventListener("click", logoutGoogle);
  }

  document.addEventListener("input", () => {
    saveUserNotes();
  });

  document.addEventListener("change", () => {
    saveUserNotes();
  });

  document.addEventListener("click", () => {
    setTimeout(saveUserNotes, 200);
  });
});
