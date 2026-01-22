# Learnman - The Seductive AI Language Tutor 💋

**Learnman** is a next-generation language learning platform that combines **Blazor WebAssembly**, **Offline AI (Ollama)**, and a **Seductive Persona Engine** to make learning a new language... unforgettably engaging.

![App Screenshot](https://i.imgur.com/PLACEHOLDER.png)

## ✨ key Features

*   **Massive Teacher Roster**: Learn from **18 Italian teachers** (9 Men, 9 Women) plus international instructors, all aged 18-25.
*   **Seductive AI Personas**: Every teacher has a unique flirtatious personality.
*   **Offline AI Intelligence**: Powered by **Ollama (Llama 3.2)** locally on your machine.
*   **Premium Aesthetics**: Dark Mode UI with Glassmorphism and custom scrollbars.
*   **Smart Chat**: Emoji translation and accessibility-focused large fonts.
*   **Persistent Progress**: **SQLite Database** saves your user profile, language preferences, and chat history.

## 🛠️ Tech Stack

*   **Framework**: .NET 10 / Blazor Web App (Interactive Server)
*   **Database**: Entity Framework Core + SQLite
*   **AI Engine**:
    *   **Local**: Ollama (Llama 3.2)
    *   **Cloud**: OpenRouter logic (Gemini 2.0 Flash / Llama 3)
*   **Styling**: Vanilla CSS (Custom Design System)

## 🚀 Getting Started

### Prerequisites

1.  **.NET 10 SDK**: [Download Here](https://dotnet.microsoft.com/download/dotnet/10.0)
2.  **Ollama** (For Offline Mode): [Download Here](https://ollama.com)
    *   *After installing, run this in your terminal:*
        ```bash
        ollama run llama3.2
        ```

### Installation

1.  **Clone the repository**:
    ```bash
    git clone https://github.com/yourusername/learnman.git
    cd learnman
    ```

2.  **Build the project**:
    ```bash
    dotnet build
    ```

3.  **Run the application**:
    ```bash
    dotnet run
    ```
    *Open your browser to `http://localhost:5000`*

## 📖 How to Use

1.  **Register/Login**: Create an account (data is saved locally in `learnman.db`).
2.  **Onboarding**: Select your **Native Language** and **Target Language**.
3.  **Choose a Teacher**: Select the persona that appeals to you most.
4.  **Start Chatting**:
    *   Say "Hello" or "Ciao".
    *   The teacher will respond in character.
    *   If you type `*wink*`, the app will render it as 😉.

## 🤝 Contributing

Contributions are welcome! If you want to add more "spicy" personas or improve the AI prompting strategy, feel free to fork and submit a PR.

## 📄 License

MIT License. Created by [Your Name].
