import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ApiError } from '../api/client';
import { getDrone } from '../api/dronesApi';
import type { Drone } from '../types/drone'

type DroneDetailsResult = {
    id: string;
    drone: Drone | null;
    error: string | null;
};



export function DroneDetailsPage() {
    const { id } = useParams<{ id: string }>();

    const [result, setResult] = useState<DroneDetailsResult | null>(null);

    useEffect(() => {
        if (!id) {
            return;
        }

        let ignore = false;

        async function loadDrone(id: string) {
            try {
                const drone = await getDrone(id);

                if (!ignore) {
                    setResult({ id, drone, error: null });
                }
            } catch (error: unknown) {
                let message = 'Could not load the drone. Check your connection and try again.';

                if (error instanceof ApiError) {
                    message = error.status === 404
                        ? 'Drone not found.'
                        : 'The server could not load the drone. Please try again later';
                }

                if (!ignore) {
                    setResult({ id, drone: null, error: message });
                }
            }
        }

        void loadDrone(id);

        return () => {
            ignore = true;
        };
    }, [id]);

    const currentResult = result?.id === id ? result : null;
    const drone = currentResult?.drone;
    return (
        <section>
            <h1>Drone details</h1>

            {!id && (
                <p className="page-message error">Invalid drone address.</p>
            )}

            {id &&  !currentResult && (
                <p className="page-message" role="status">
                    Loading drone...
                </p>
            )}

            {currentResult?.error && (
                <p className="page-message error" role="alert">
                    {currentResult.error}
                </p>
            )}

            {drone && (
                <div>
                    <h2>{drone.brand} {drone.model}</h2>

                    <p>
                        <span className={`availability ${drone.isAvailable
                            ? 'availability--available'
                            : 'availability--unavailable'
                            }`}
                        >
                            {drone.isAvailable ? 'Available' : 'Unavailable'}
                        </span>
                    </p>

                    <dl>
                        <dt>Price per hour</dt>
                        <dd>{drone.pricePerHour.toFixed(2)} / h</dd>

                        <dt>Battery life</dt>
                        <dd>{drone.batteryLifeMinutes} min</dd>

                        <dt>Maximum range</dt>
                        <dd>{drone.maxRangeMeters.toLocaleString('en-US')} m</dd>
                    </dl>
                </div>
            )}

            <Link to="/drones">Back to drones</Link>
        </section>
    )
}