// Contrato de la API de recetas
export interface RecipeSearchResult {
  externalId: number;
  name: string;
  imageUrl: string;
  readyInMinutes: number | null;
  dishTypes: string[];
  cuisines: string[];
}

export interface RecipeSearchFilters {
  cuisine: string;
  diet: string;
  type: string;
}

export interface RecipeWithMatch {
  externalId: number;
  name: string;
  imageUrl: string;
  canMake: boolean;
  missingIngredientsCount: number;
  readyInMinutes: number | null;
  dishTypes: string[];
  diets: string[];
  cuisines: string[];
}

export interface RecipeStep {
  number: number;
  description: string;
}

export interface RecipeIngredient {
  name: string;
  imageUrl: string;
  quantity: number;
  unit: string;
}

export interface RecipeDetail {
  externalId: number;
  name: string;
  summary: string;
  imageUrl: string;
  readyInMinutes: number | null;
  dishTypes: string[];
  diets: string[];
  cuisines: string[];
  isFavorite: boolean;
  steps: RecipeStep[];
  ingredients: RecipeIngredient[];
}
