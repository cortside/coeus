# chat-frontend

This is the frontend React application for the chat service. It provides a modern chat UI, authenticates users via OIDC, and streams chat messages from the backend.

## Features
- Vite + React 19 + TypeScript + Tailwind CSS + shadcn/ui
- OIDC login (PKCE) with JWT stored in memory
- Chat page with streaming messages via WebSocket
- API client for backend

## Requirements
- Node.js 20+
- npm

## Setup

1. Install dependencies:
   ```powershell
   npm install
   ```
2. Copy `.env.example` to `.env` and adjust as needed.

## Running the Service

- To start the frontend:
  ```powershell
  npm run dev
  ```
- Or use the provided script:
  ```powershell
  ./run.ps1
  ```

## Development
- Tailwind CSS and shadcn/ui for styling.
- Edit `src/` for components and logic.

## License
MIT
