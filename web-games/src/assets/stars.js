document.addEventListener("DOMContentLoaded", () => {
    const starContainer = document.createElement("div");
    starContainer.id = "stars";
    document.body.appendChild(starContainer);
  
    const numStars = 300;
    const stars = [];
  
    for (let i = 0; i < numStars; i++) {
      const star = document.createElement("div");
      star.className = "star";
      star.style.top = `${Math.random() * 100}vh`;
      star.style.left = `${Math.random() * 100}vw`;

      const randomDelay = Math.random() * 5;
      const randomDuration = 2+ Math.random() * 3;
      star.style.animationDelay = `${randomDelay}s`;
      star.style.animationDuration = `${randomDuration}s`;
     
      starContainer.appendChild(star);
      stars.push(star);
    }
  
    document.addEventListener("mousemove", (e) => {
      const { clientX, clientY } = e;
      stars.forEach((star, index) => {
        const speed = (index % 10) + 1; // Different speeds for parallax effect
        const offsetX = (clientX / window.innerWidth - 0.5) * speed * 10;
        const offsetY = (clientY / window.innerHeight - 0.5) * speed * 10;
        star.style.transform = `translate(${offsetX}px, ${offsetY}px)`;
      });
    });
  });