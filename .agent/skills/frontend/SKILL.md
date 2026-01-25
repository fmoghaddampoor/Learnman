---
name: Frontend Web Development
description: Guidelines and best practices for building high-quality, modern web applications.
---

# Frontend Web Development Skill

This skill outlines the standards and workflows for creating web applications in this workspace.

## Technology Stack

1.  **Core**: HTML5, JavaScript (ES6+), CSS3.
2.  **Styling**: Vanilla CSS is preferred for flexibility. Use TailwindCSS only if explicitly requested (confirm version first).
3.  **Frameworks**:
    *   For complex web apps: Use **Next.js** or **Vite**.
    *   **New Project Creation**:
        *   Use `npx -y` (e.g., `npx -y create-vite@latest ./`).
        *   Run with `--help` to see options.
        *   Run non-interactively.
4.  **Local Development**: Use `npm run dev`. Build for production only when requested or validating.

## Design Aesthetics (CRITICAL)

The user expects **premium, wow-factor designs**.

1.  **Visual Excellence**:
    *   Avoid generic colors. Use curated palettes (HSL, variables).
    *   Use modern typography (Inter, Roboto, Outfit, etc.).
    *   Implement smooth gradients and glassmorphism where appropriate.
2.  **Dynamic & Alive**:
    *   Hover effects, transitions, and micro-animations are mandatory.
    *   The interface should feel responsive to user interaction.
3.  **No Placeholders**: Use `generate_image` tool for assets if needed.

## Implementation Workflow

1.  **Plan**: Understand requirements, sketch features, choose aesthetic direction.
2.  **Foundation**: Set up `index.css` (variables, reset, base styles). Define the design system.
3.  **Components**: Build reusable, styled components. Avoid ad-hoc styling in logic files.
4.  **Assembly**: Compose pages with proper routing and layout.
5.  **Polish**: Review UX, smooth out animations, optimize performance.

## SEO & Best Practices

*   **Title Tags**: Unique and descriptive for every page.
*   **Meta Descriptions**: Summarize content accurately.
*   **Heading Structure**: Single `<h1>` per page, logical hierarchy (`<h2>`, `<h3>`).
*   **Semantic HTML**: Use `<nav>`, `<main>`, `<article>`, `<footer>`, etc.
*   **Accessibility**: ARIA labels where needed, good contrast, keyboard navigation.
*   **Unique IDs**: For testing and anchoring.
