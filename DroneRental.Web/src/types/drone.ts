export type Drone = {
    id: string;
    brand: string;
    model: string;
    pricePerHour: number;
    isAvailable: boolean;
    batteryLifeMinutes: number;
    maxRangeMeters: number;
}