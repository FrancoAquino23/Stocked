// Contrato de la API de despensa
export interface PantryItem {
  externalId: number;
  name: string;
  imageUrl: string;
  usualQuantity: number | null;
  usualUnit: string | null;
}
