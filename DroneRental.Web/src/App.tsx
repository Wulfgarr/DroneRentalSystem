import { Navigate, Route, Routes } from 'react-router-dom';
import './App.css';
import { AppLayout } from './components/AppLayout';
import { DronesPage } from './pages/DronesPage';
import { NotFoundPage } from './pages/NotFoundPage';
import { DroneDetailsPage } from "./pages/DroneDetailsPage";
import { LoginPage } from "./pages/LoginPage";
import { RegisterPage } from './pages/RegisterPage';


function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route path="/" element={<Navigate to="/drones" replace />} />
        <Route path="/drones" element={<DronesPage />} />
        <Route path="/drones/:id" element={<DroneDetailsPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}

export default App;
