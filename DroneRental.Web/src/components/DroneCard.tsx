import type { Drone } from '../types/drone';

type Props = {
    drone: Drone;
};

export function DroneCard({ drone }: Props) {

    const formattedPrice = new Intl.NumberFormat('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    }).format(drone.pricePerHour);

    const formattedRange = new Intl.NumberFormat('en-US').format(
        drone.maxRangeMeters
    );

    return (
        <li className="drone-card">
            <h3>{drone.brand} {drone.model}</h3>
            <p>Price: {formattedPrice} / h</p>

            <span
                className={
                    drone.isAvailable
                        ? 'availability availability--available'
                        : 'availability availability--unavailable'
                }
            >
                {drone.isAvailable ? 'Available' : 'Unavailable'}
            </span>

            <p>Battery life: {drone.batteryLifeMinutes} min</p>
            <p>Max range: {formattedRange} m</p>
        </li>
    );
}