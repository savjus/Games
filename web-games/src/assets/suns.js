document.addEventListener("DOMContentLoaded", () => {
  const sunsContainer = document.createElement("div");
  sunsContainer.id = "suns";
  document.body.appendChild(sunsContainer);

  const numSuns = 5;

  for (let i = 0; i < numSuns; i++) {
    const sun = document.createElement("button");
    sun.className = "sun";
    sun.style.top = `${Math.random() * 100}vh`;
    sun.style.left = `${Math.random() * 100}vw`;

    sun.addEventListener("click", () => {
      createExplosion(sun);
      sun.remove();
    });

    sunsContainer.appendChild(sun);
  }

  function createExplosion(sun) {
    const numParticles = 30;
    const sunRect = sun.getBoundingClientRect();
    const explosionContainer = document.createElement("div");
    explosionContainer.className = "explosion";
    explosionContainer.style.top = `${sunRect.top + sunRect.height / 2}px`;
    explosionContainer.style.left = `${sunRect.left + sunRect.width / 2}px`;
    document.body.appendChild(explosionContainer);

    for (let i = 0; i < numParticles; i++) {
      const particle = document.createElement("div");
      particle.className = "particle";
      const angle = Math.random() * 2 * Math.PI; // Random direction
      const speed = Math.random() * 500 + 100; // Random speed
      const velocityX = Math.cos(angle) * speed;
      const velocityY = Math.sin(angle) * speed;

      particle.style.setProperty("--velocity-x", `${velocityX}px`);
      particle.style.setProperty("--velocity-y", `${velocityY}px`);
      particle.style.setProperty("--animation-duration", `${Math.random() * 1 + 0.5}s`);

      explosionContainer.appendChild(particle);
    }

    setTimeout(() => explosionContainer.remove(), 1000); // Remove explosion after animation
  }
});