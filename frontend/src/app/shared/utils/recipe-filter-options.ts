// Opciones de cocina/dieta/tipo de comida, compartidas entre búsqueda y favoritas
export const CUISINE_OPTIONS = [
  'American', 'Chinese', 'French', 'Greek', 'Indian',
  'Italian', 'Japanese', 'Korean', 'Mediterranean', 'Mexican', 'Thai', 'Vietnamese',
];

export const DIET_OPTIONS = ['Gluten Free', 'Ketogenic', 'Vegetarian', 'Vegan', 'Pescetarian', 'Paleo'];

export const TYPE_OPTIONS = [
  'Main Course', 'Side Dish', 'Dessert', 'Appetizer', 'Salad', 'Breakfast', 'Soup', 'Snack',
];

// Vocabulario fijo de Spoonacular para intolerancias — búsqueda solo (server-side)
export const INTOLERANCE_OPTIONS = [
  'Dairy', 'Egg', 'Gluten', 'Grain', 'Peanut', 'Seafood', 'Sesame', 'Shellfish', 'Soy', 'Sulfite', 'Tree Nut', 'Wheat',
];
