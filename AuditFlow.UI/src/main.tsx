import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
// import './index.css' // Temporarily commented out to avoid PostCSS errors - 暂时注释掉，避免 PostCSS 错误
import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
