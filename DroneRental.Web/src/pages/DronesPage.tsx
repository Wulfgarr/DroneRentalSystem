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

    if (isLoading) return <p>Loading...</p>
    if (errorMessage) return <p className='error'>{errorMessage}</p>
    
    return (
        <ul className="drone-list">
            {drones.map(d => (
                <DroneCard key={d.id} drone={d}/>
            ))}
        </ul>
    );
}

