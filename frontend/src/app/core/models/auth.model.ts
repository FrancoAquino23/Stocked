// Contrato de la API de autenticación
export interface AuthResponse {
  token: string;
  username: string;
  email: string;
  pantryDayOfWeek: string | null;
  pantryTime: string | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface UpdatePantryScheduleRequest {
  dayOfWeek: string;
  time: string;
}
