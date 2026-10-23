// Contrato de la API de preparaciones de receta
export type RecipePreparationStatus = 'Pending' | 'InProgress' | 'Done';

export interface RecipePreparation {
  id: number;
  externalId: number;
  recipeName: string;
  multiplier: number;
  status: RecipePreparationStatus;
}

export interface RecipeAffordability {
  requestedMultiplier: number;
  affordableMultiplier: number;
  fullyCovered: boolean;
  uncertainIngredients: string[];
}
