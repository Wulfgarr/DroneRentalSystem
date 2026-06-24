import type { Drone } from '../types/drone';

type Props = {
    drone: Drone;
};

export function DroneCard({ drone }: Props) {
    return (
        <li key={drone.id} className="drone-card">
            <h3>{drone.brand} {drone.model}</h3>
            <p>Price per hour: {drone.pricePerHour}</p>
            <p>Available: {drone.isAvailable ? 'Yes' : 'No'}</p>
            <p>Battery life minutes: {drone.batteryLifeMinutes}</p>
            <p>Max range meters: {drone.maxRangeMeters}</p>
        </li>
    )
}