import { Navigate, Route, Routes } from 'react-router-dom';
import './App.css';
import { DronesPage } from './pages/DronesPage';
import { NotFoundPage} from './pages/NotFoundPage';


function App() {
    return (
      <main className="app">
      <h1>DroneRental</h1>
      <Routes>
        <Route path="/" element={<Navigate to="/drones" replace />} />
        <Route path="/drones" element={<DronesPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </main>
  );
}

export default App
