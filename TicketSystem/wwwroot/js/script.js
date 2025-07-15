// Java Script für Klickgeräusche

document.querySelectorAll(".click-btn").forEach(button => {
    button.addEventListener("click", () => {
        const audio = new Audio("/audio/click.mp3");
        audio.play();
    })
});