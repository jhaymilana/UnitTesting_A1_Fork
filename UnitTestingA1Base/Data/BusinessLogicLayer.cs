using System.Xml.Linq;
using UnitTestingA1Base.Data;
using UnitTestingA1Base.Models;

namespace UnitTestingA1Base.Data
{
    public class BusinessLogicLayer
    {
        private AppStorage _appStorage;

        public BusinessLogicLayer(AppStorage appStorage) {
            _appStorage = appStorage;
        }
        public HashSet<Recipe>? GetRecipesByIngredient(int? id, string? name)
        {
            if (id == null && name == null)
            {
                return null;
            }

            if (name != null)
            {
                int? ingredientId = _appStorage.Ingredients.FirstOrDefault(i => i.Name == name)?.Id;
                if (ingredientId == null)
                {
                    return null;
                }

                HashSet<Recipe> recipes = _appStorage.Recipes
                    .Where(r => _appStorage.RecipeIngredients
                    .Any(ri => ri.RecipeId == r.Id && ri.IngredientId == ingredientId))
                    .ToHashSet();

                return recipes;
            }
            else if (id != null)
            {
                bool ingredientExists = _appStorage.Ingredients.Any(i => i.Id == id);
                if (!ingredientExists)
                {
                    return null;
                }

                HashSet<Recipe> recipes = _appStorage.Recipes
                    .Where(r => _appStorage.RecipeIngredients
                    .Any(ri => ri.RecipeId == r.Id && ri.IngredientId == id))
                    .ToHashSet();

                return recipes;
            }

            return null;
        }

        public HashSet<Recipe>? GetRecipesByDiet(int? id, string? name)
        {
            HashSet<Recipe> recipes = new HashSet<Recipe>();

            if (id == null && name == null)
            {
                return null;
            }

            if (name != null)
            {
                DietaryRestriction? diet = _appStorage.DietaryRestrictions.FirstOrDefault(d => d.Name == name);

                if (diet == null)
                {
                    return null;
                }

                if (diet != null)
                {
                    HashSet<IngredientRestriction> dietaryRestrictions = _appStorage.IngredientRestrictions.Where(dR => dR.DietaryRestrictionId == diet.Id).ToHashSet();

                    HashSet<int> allowedIngredientIds = dietaryRestrictions.Select(dR => dR.IngredientId).ToHashSet();

                    recipes = _appStorage.Recipes
                        .Where(r => _appStorage.RecipeIngredients
                            .Where(ri => ri.RecipeId == r.Id)
                            .All(ri => allowedIngredientIds.Contains(ri.IngredientId)))
                        .ToHashSet();

                    return recipes;
                }
                else
                {
                    return null;
                }
            }
            else if (id != null)
            {
                DietaryRestriction? diet = _appStorage.DietaryRestrictions.FirstOrDefault(d => d.Id == id);

                if (diet != null)
                {
                    HashSet<IngredientRestriction> dietaryRestrictions = _appStorage.IngredientRestrictions.Where(dR => dR.DietaryRestrictionId == diet.Id).ToHashSet();

                    HashSet<int> allowedIngredientIds = dietaryRestrictions.Select(dR => dR.IngredientId).ToHashSet();

                    recipes = _appStorage.Recipes
                        .Where(r => _appStorage.RecipeIngredients
                            .Where(ri => ri.RecipeId == r.Id)
                            .All(ri => allowedIngredientIds.Contains(ri.IngredientId)))
                        .ToHashSet();

                    return recipes;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public HashSet<Recipe>? GetRecipes(int? id, string? name)
        {
            if (id == null && name == null)
            {
                return null;
            }

            if (name != null)
            {
                int? ingredientId = _appStorage.Recipes.FirstOrDefault(i => i.Name == name)?.Id;
                if (ingredientId == null)
                {
                    return null;
                }

                HashSet<Recipe> recipes = _appStorage.Recipes
                    .Where(r => r.Id == ingredientId)
                    .ToHashSet();

                return recipes;
            }
            if (id != null)
            {
                bool recipeExists = _appStorage.Recipes.Any(i => i.Id == id);
                if (!recipeExists)
                {
                    return null;
                }

                HashSet<Recipe> recipes = _appStorage.Recipes
                    .Where(r => r.Id == id)
                    .ToHashSet();

                return recipes;
            }

            return null;
        }

        public Recipe CreateNewRecipe(RecipeInputModel inputModel)
        {
            if (_appStorage.Recipes.Any(r => r.Name == inputModel.Recipe.Name))
            {
                throw new InvalidOperationException("Recipe with the same name already exists.");
            }

            foreach (Ingredient ingredient in inputModel.Ingredients)
            {
                if (!_appStorage.Ingredients.Any(i => i.Name == ingredient.Name))
                {
                    ingredient.Id = _appStorage.GeneratePrimaryKey();
                    _appStorage.Ingredients.Add(ingredient);
                }
            }

            inputModel.Recipe.Id = _appStorage.GeneratePrimaryKey();
            foreach (Ingredient ingredient in inputModel.Ingredients)
            {
                ingredient.Id = _appStorage.Ingredients.First(i => i.Name == ingredient.Name).Id;
            }

            _appStorage.Recipes.Add(inputModel.Recipe);

            foreach (Ingredient ingredient in inputModel.Ingredients)
            {
                RecipeIngredient recipeIngredient = new RecipeIngredient
                {
                    RecipeId = inputModel.Recipe.Id,
                    IngredientId = ingredient.Id
                };
                _appStorage.RecipeIngredients.Add(recipeIngredient);
            }

            return inputModel.Recipe;
        }

        public Ingredient? DeleteIngredient(int? id, string? name)
        {
            Ingredient? ingredientToDelete = _appStorage.Ingredients.FirstOrDefault(i => i.Id == id || i.Name == name);

            if (ingredientToDelete == null)
            {
                throw new InvalidOperationException("No ingredient found.");
            }

            int recipeCount = _appStorage.RecipeIngredients.Count(ri => ri.IngredientId == ingredientToDelete.Id);

            if (recipeCount == 1)
            {
                RecipeIngredient recipeIngredientToDelete = _appStorage.RecipeIngredients.First(ri => ri.IngredientId == ingredientToDelete.Id);
                Recipe recipeToDelete = _appStorage.Recipes.First(r => r.Id == recipeIngredientToDelete.RecipeId);

                _appStorage.Recipes.Remove(recipeToDelete);
                _appStorage.RecipeIngredients.Remove(recipeIngredientToDelete);
            }
            else if (recipeCount > 1)
            {
                throw new InvalidOperationException("Ingredient is used in multiple recipes. Cannot delete.");
            }

            _appStorage.Ingredients.Remove(ingredientToDelete);

            return ingredientToDelete;
        }

        public Recipe? DeleteRecipe(int? id, string? name)
        {
            Recipe? recipeToDelete = _appStorage.Recipes.FirstOrDefault(r => r.Id == id || r.Name == name);

            if (recipeToDelete != null)
            {
                // Find all RecipeIngredients objects associated with the recipe
                List<RecipeIngredient> recipeIngredientsToDelete = _appStorage.RecipeIngredients
                    .Where(ri => ri.RecipeId == recipeToDelete.Id)
                    .ToList();

                foreach (RecipeIngredient recipeIngredient in recipeIngredientsToDelete)
                {
                    _appStorage.RecipeIngredients.Remove(recipeIngredient);
                }

                _appStorage.Recipes.Remove(recipeToDelete);

                return recipeToDelete;
            }
            else
            {
                throw new InvalidOperationException("Recipe not found.");
            }
        }
    }
}
