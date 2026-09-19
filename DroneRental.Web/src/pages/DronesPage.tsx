import { useEffect, useState } from 'react';
import type { Drone } from '../types/drone';
import { getDrones } from '../api/dronesApi';
import { DroneCard } from '../components/DroneCard';

export function DronesPage() {
    const [drones, setDrones] = useState<Drone[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    useEffect(() => {
        async function load() {
            try{
                const data = await getDrones();
                setDrones(data);
            } catch {
                setErrorMessage('Could not load drones.')
            } finally {
                setIsLoading(false);
            }
        }
        
        load();
    }, []);

    if (isLoading) {
            return (
            <p className="page-message">
                Loading drones...
            </p>
            );
    }
    if (errorMessage) {
        return (
        <p className="page-message error">
            {errorMessage}
        </p>
        );
    }
    if (drones.length === 0) {
        return <p className="page-message">No drones found.</p>;
    }
    
    return (
        <ul className="drone-list">
            {drones.map(drone => (
                <DroneCard
                key={drone.id}
                drone={drone}
                />
            ))}
        </ul>
    );
}

