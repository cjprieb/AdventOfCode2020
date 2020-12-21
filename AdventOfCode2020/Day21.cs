using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Day21
{
    [TestClass]
    public class Day21
    {
        #region Input

        public IEnumerable<string> GetInput() => Input.GetLines(Properties.Resources.Day21Input);

        public IEnumerable<string> GetTestInput() => new string[]
        {
            "mxmxvkd kfcds sqjhc nhms (contains dairy, fish)",
            "trh fvjkl sbzzf mxmxvkd (contains dairy)",
            "sqjhc fvjkl (contains soy)",
            "sqjhc mxmxvkd sbzzf (contains fish)"
        };

        #endregion Input

        #region Code...

        #region CountNonAllergenIngredients
        private int CountNonAllergenIngredients(IEnumerable<string> input)
        {
            var foods = input.Select(Food.Parse).ToArray();
            var allAllergens = foods.SelectMany(food => food.Allergens).ToHashSet();
            var allIngredients = foods.SelectMany(food => food.Ingredients).ToHashSet();

            var foodsWithAllergens = new Dictionary<string, List<Food>>();
            foods.Aggregate(foodsWithAllergens, (dict, food) =>
            {
                foreach (var allergen in food.Allergens)
                {
                    if (!dict.ContainsKey(allergen)) dict[allergen] = new List<Food>();
                    dict[allergen].Add(food);
                }
                return dict;
            });

            var possibleIngredientsContainingAllergens = new HashSet<string>();
            foreach (var allergen in allAllergens)
            {
                var foodsWithAllergen = foodsWithAllergens[allergen];
                var firstFood = foodsWithAllergen[0];
                var otherFoods = foodsWithAllergen.Skip(1);

                possibleIngredientsContainingAllergens.AddAll(
                    firstFood
                    .Ingredients
                    .Where(ingredient => otherFoods.All(food => food.Ingredients.Contains(ingredient)))
                );
            }

            return foods.Sum(
                food => food.Ingredients.Count(ingredient => !possibleIngredientsContainingAllergens.Contains(ingredient))
            );
        }
        #endregion CountNonAllergenIngredients

        #region GetCanonicalDangerousIngredientList
        private string GetCanonicalDangerousIngredientList(IEnumerable<string> input)
        {
            var foods = input.Select(Food.Parse).ToArray();
            var allAllergens = foods.SelectMany(food => food.Allergens).ToHashSet();
            var allIngredients = foods.SelectMany(food => food.Ingredients).ToHashSet();

            var foodsWithAllergens = new Dictionary<string, List<Food>>();
            foods.Aggregate(foodsWithAllergens, (dict, food) =>
            {
                foreach (var allergen in food.Allergens)
                {
                    if (!dict.ContainsKey(allergen)) dict[allergen] = new List<Food>();
                    dict[allergen].Add(food);
                }
                return dict;
            });

            var possibleIngredientsContainingAllergens = new Dictionary<string, HashSet<string>>();
            foreach (var allergen in allAllergens)
            {
                var foodsWithAllergen = foodsWithAllergens[allergen];
                var firstFood = foodsWithAllergen[0];
                var otherFoods = foodsWithAllergen.Skip(1);

                possibleIngredientsContainingAllergens[allergen] = new HashSet<string>(
                    firstFood
                        .Ingredients
                        .Where(ingredient => otherFoods.All(food => food.Ingredients.Contains(ingredient)))
                );
            }

            bool shouldContinue = possibleIngredientsContainingAllergens.Values.Any(list => list.Count == 1);
            while (shouldContinue)
            {
                shouldContinue = false;
                foreach (var kvp in possibleIngredientsContainingAllergens)
                {
                    if (kvp.Value.Count == 1)
                    {
                        foreach (var otherKvp in possibleIngredientsContainingAllergens)
                        {
                            if (kvp.Key == otherKvp.Key) continue;
                            if (otherKvp.Value.Count == 1) continue;

                            otherKvp.Value.RemoveAll(kvp.Value);

                            if (otherKvp.Value.Count == 1)
                            {
                                shouldContinue = true;
                            }
                        }
                    }
                }
            }

            var badIngredients = possibleIngredientsContainingAllergens
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value.First())
                .ToArray();

            return string.Join(",", badIngredients);
        }
        #endregion GetCanonicalDangerousIngredientList

        #endregion Code...

        #region Tests...
        [TestMethod] public void Test1() => Assert.AreEqual(5, CountNonAllergenIngredients(GetTestInput()));
        [TestMethod] public void Answer1() => Assert.AreEqual(2573, CountNonAllergenIngredients(GetInput()));

        [TestMethod] public void Test2() => Assert.AreEqual("mxmxvkd,sqjhc,fvjkl", GetCanonicalDangerousIngredientList(GetTestInput()));
        [TestMethod] public void Answer2() => Assert.AreEqual("bjpkhx,nsnqf,snhph,zmfqpn,qrbnjtj,dbhfd,thn,sthnsg", GetCanonicalDangerousIngredientList(GetInput()));
        #endregion Tests...
    }

    public class Food
    {
        #region Properties...
        public HashSet<string> Ingredients { get; private set; } = new HashSet<string>();
        public HashSet<string> Allergens { get; private set; } = new HashSet<string>();
        #endregion Properties...

        #region Methods...

        #region Parse
        public static Food Parse(string line)
        {
            Match match = Regex.Match(line, @"(.+) \(contains (.+)\)");
            if (!match.Success) throw new Exception($"'{line}' does not match the pattern");

            var foodItem = new Food();
            var ingredientsString = match.Groups[1].Value;
            foreach (var token in ingredientsString.Split(' '))
            {
                foodItem.Ingredients.Add(token.Trim());
            }

            var allergensString = match.Groups[2].Value;
            foreach (var token in allergensString.Split(','))
            {
                foodItem.Allergens.Add(token.Trim());
            }

            return foodItem;
        }
        #endregion Parse

        #endregion Methods...
    }
}
