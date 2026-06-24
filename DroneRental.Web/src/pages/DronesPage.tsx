import { useEffect, useState } from 'react';
import type { Drone } from '../types/drone';
import { getDrones } from '../api/dronesApi';
import { DroneCard } from '../components/DroneCard';

export function DronesPage() {
    const [drones, setDrones] = useState<Drone[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        async function load() {
            const data = await getDrones();
            setDrones(data);
            setIsLoading(false);
        }
        
        load();
    }, []);

    if (isLoading) return <p>Loading...</p>
    
    return (
        <ul className="drone-list">
            {drones.map(d => (
                <DroneCard key={d.id} drone={d}/>
            ))}
        </ul>
    );
}

