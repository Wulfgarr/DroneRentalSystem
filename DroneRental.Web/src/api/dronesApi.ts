import type { Drone } from "../types/drone";
import { apiGet } from "./client";

export function getDrones(): Promise<Drone[]> {
    return apiGet<Drone[]>(`/api/drones`);
}