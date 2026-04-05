const App = (() => {
  function showPopup(message) {
    const popup = document.getElementById("arcanePopup");
    const msg = document.getElementById("popupMessage");
    const closeBtn = document.getElementById("popupClose");
    if (!popup || !msg || !closeBtn) return;

    msg.textContent = message;
    popup.style.display = "flex";

    closeBtn.onclick = () => (popup.style.display = "none");
    popup.onclick = (e) => {
      if (e.target === popup) popup.style.display = "none";
    };
  }

  function makeItArcane(role) {
    showPopup(`Arcane mode activated for ${role}!`);
  }

  return {
    makeItArcane,
  };
})();

window.App = App;
