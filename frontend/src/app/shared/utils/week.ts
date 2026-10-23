// Lunes de la semana actual. Formato (YYYY-MM-DD)
export function getCurrentWeekStart(): string {
  const today = new Date();
  const isoDayOfWeek = today.getDay() === 0 ? 7 : today.getDay();
  const monday = new Date(today);
  monday.setDate(today.getDate() - (isoDayOfWeek - 1));

  const year = monday.getFullYear();
  const month = String(monday.getMonth() + 1).padStart(2, '0');
  const day = String(monday.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}
