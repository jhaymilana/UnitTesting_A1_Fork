using System.Security.Cryptography;
using UnitTestingA1Base.Data;
using UnitTestingA1Base.Models;

namespace RecipeUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private BusinessLogicLayer _initializeBusinessLogic()
        {
            return new BusinessLogicLayer(new AppStorage());
        }

        // GetRecipeByIngredient()

        [TestMethod]
        public void GetRecipesByIngredient_ValidId_ReturnsRecipesWithIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredientId = 6;
            int recipeCount = 2;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByIngredient(ingredientId, null);

            Assert.AreEqual(recipeCount, recipes?.Count);
        }

        [TestMethod]
        public void GetRecipesByIngredient_ValidName_ReturnsRecipesWithIngredient()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string ingredientName = "Salmon";
            int recipeCount = 2;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByIngredient(null, ingredientName);

            Assert.AreEqual(recipeCount, recipes?.Count);
        }

        [TestMethod]
        public void GetRecipesByIngredient_InvalidId_ReturnsNull()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredientId = 999;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByIngredient(ingredientId, null);

            Assert.IsNull(recipes);
        }

        [TestMethod]
        public void GetRecipesByIngredient_InvalidName_ReturnsNull()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string ingredientName = "Potatoes";

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByIngredient(null, ingredientName);

            Assert.IsNull(recipes);
        }

        // GetRecipeByDiet()

        [TestMethod]
        public void GetRecipesByDiet_ValidId_ReturnsRecipesWithDietRestrictions()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int dietaryRestrictionId = 1;
            int recipeCount = 3;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByDiet(dietaryRestrictionId, null);

            Assert.AreEqual(recipeCount, recipes?.Count);
        }

        [TestMethod]
        public void GetRecipesByDiet_ValidName_ReturnsRecipesWithDietRestrictions()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string dietaryRestrictionName = "Vegetarian";
            int recipeCount = 3;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByDiet(null, dietaryRestrictionName);

            Assert.AreEqual(recipeCount, recipes?.Count);
        }

        [TestMethod]
        public void GetRecipesByDiet_InvalidId_ReturnsNull()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int dietaryRestrictionId = 999;

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByDiet(dietaryRestrictionId, null);

            Assert.IsNull(recipes);
        }

        [TestMethod]
        public void GetRecipesByDiet_InvalidName_ReturnsNull()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string dietaryRestrictionName = "Pescatarian";

            // act
            HashSet<Recipe>? recipes = bll.GetRecipesByDiet(null, dietaryRestrictionName);

            Assert.IsNull(recipes);
        }

        // GetRecipes()

        [TestMethod]
        public void GetRecipes_ValidId_ReturnsRecipes()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int recipeId = 1;

            // act
            HashSet<Recipe>? recipe = bll.GetRecipes(recipeId, null);

            Assert.IsNotNull(recipe);
        }

        [TestMethod]
        public void GetRecipes_ValidName_ReturnsRecipes()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string recipeName = "Chicken Alfredo";

            // act
            HashSet<Recipe>? recipe = bll.GetRecipes(null, recipeName);

            Assert.IsNotNull(recipe);
        }

        [TestMethod]
        public void GetRecipes_InvalidId_ReturnsNull()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int recipeId = 999;

            // act
            HashSet<Recipe>? recipe = bll.GetRecipes(recipeId, null);

            Assert.IsNull(recipe);
        }

        [TestMethod]
        public void GetRecipes_InvalidName_ReturnsNull()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string recipeName = "Pulled Pork Poutine";

            // act
            HashSet<Recipe>? recipe = bll.GetRecipes(null, recipeName);

            Assert.IsNull(recipe);
        }

        // CreateNewRecipe()

        [TestMethod]
        public void CreateNewRecipe_ValidName_ReturnsNewRecipe()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            RecipeInputModel newRecipe = new RecipeInputModel()
            {
                Recipe = new Recipe
                {
                    Name = "Pulled Pork Poutine",
                    Description = "Fries and cheese curds topped with gravy and pulled pork",
                    Servings = 1
                },
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Potatoes"
                    },
                    new Ingredient
                    {
                        Name = "Cheese Curds"
                    },
                    new Ingredient
                    {
                        Name = "Gravy"
                    },
                    new Ingredient
                    {
                        Name = "Pulled Pork"
                    }
                }
            };

            // act
            Recipe createdRecipe = bll.CreateNewRecipe(newRecipe);

            Assert.IsNotNull(createdRecipe);
            Assert.AreEqual(createdRecipe.Name, newRecipe.Recipe.Name);
        }

        [TestMethod]
        public void CreateNewRecipe_WithExistingRecipeName_ReturnsException()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            RecipeInputModel newRecipe = new RecipeInputModel()
            {
                Recipe = new Recipe
                {
                    Name = "Chicken Alfredo",
                    Description = "Creamy pasta dish with grilled chicken and Parmesan cheese sauce.",
                    Servings = 4
                },
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Chicken"
                    },
                    new Ingredient
                    {
                        Name = "Alfredo Sauce"
                    },
                    new Ingredient
                    { 
                        Name = "Parmesan"
                    },
                }
            };

            // act and assert
            Assert.ThrowsException<InvalidOperationException>(() => bll.CreateNewRecipe(newRecipe));
        }

        // DeleteIngredient()

        [TestMethod]
        public void DeleteIngredient_ValidIngredientId_DeletesCorrectly()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredient = 1;

            // act
            Ingredient deletedIngredient = bll.DeleteIngredient(ingredient, null);

            Assert.IsNotNull(deletedIngredient);
            Assert.AreEqual(ingredient, deletedIngredient.Id);
        }

        [TestMethod]
        public void DeleteIngredient_ValidIngredientName_DeletesCorrectly()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string ingredient = "Spaghetti";

            // act
            Ingredient deletedIngredient = bll.DeleteIngredient(null, ingredient);

            Assert.IsNotNull(deletedIngredient);
            Assert.AreEqual(ingredient, deletedIngredient.Name);
        }

        [TestMethod]
        public void DeleteIngredient_InvalidIngredientId_ThrowsError()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int ingredient = 999;

            // act
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                bll.DeleteIngredient(ingredient, null);
            });
        }

        [TestMethod]
        public void DeleteIngredient_InvalidIngredientName_ThrowsError()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string ingredient = "Cheese Curds";

            // act
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                bll.DeleteIngredient(null, ingredient);
            });
        }

        // DeleteRecipe()

        [TestMethod]
        public void DeleteRecipe_ValidRecipeId_DeletesRecipe()
        {
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int recipeId = 1;

            // Act
            Recipe deletedRecipe = bll.DeleteRecipe(recipeId, null);

            Assert.IsNotNull(deletedRecipe); 
            Assert.AreEqual(recipeId, deletedRecipe.Id);
        }

        [TestMethod]
        public void DeleteRecipe_ValidRecipeName_DeletesRecipe()
        {
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string recipeName = "Chicken Alfredo";

            // Act
            Recipe deletedRecipe = bll.DeleteRecipe(null, recipeName);

            Assert.IsNotNull(deletedRecipe);
            Assert.AreEqual(recipeName, deletedRecipe.Name);
        }

        [TestMethod]
        public void DeleteRecipe_InvalidRecipeId_ThrowsError()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            int recipe = 999;

            // act
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                bll.DeleteRecipe(recipe, null);
            });
        }

        [TestMethod]
        public void DeleteRecipe_InvalidRecipeName_ThrowsError()
        {
            // arrange
            BusinessLogicLayer bll = _initializeBusinessLogic();
            string recipe = "Poutine";

            // act
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                bll.DeleteRecipe(null, recipe);
            });
        }
    }
}