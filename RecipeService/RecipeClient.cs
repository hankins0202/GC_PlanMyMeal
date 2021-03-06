﻿using GC_PlanMyMeal.Configuration;
using GC_PlanMyMeal.Models;
using GC_PlanMyMeal.RecipeService.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GC_PlanMyMeal.RecipeService
{
    //Service that handles all API calls
    public class RecipeClient : ISearchRecipe
    {
        private readonly HttpClient _httpClient;
        //Config/Settings file to call apiKey
        private readonly SpoonacularConfiguration _config;

        public RecipeClient(HttpClient httpClient, SpoonacularConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        //used to get recipe info by Id
        public virtual async Task<Recipe> SearchForRecipeById (int? id)
        {            
            var response = await _httpClient.GetAsync($"recipes/{id}/information?apiKey={_config.ApiKey}");
            var recipe = JsonConvert.DeserializeObject<Recipe>(await response.Content.ReadAsStringAsync());
            return recipe;
        }

        //&intolerances=egg&diet=vegetarian&maxCarbs=400&maxProtein=15&minProtein=1
        //Called to list out recipes based on the user search criteria
        public virtual async Task<List<Recipe>> SearchForRecipeByQuery (string diet, Intolerances intolerance, int? maxCalorie, int? maxCarb, int? maxProtein, int? minProtein)
        {
            StringBuilder query = new StringBuilder();
            if(diet != null){ query.Append($"&diet={diet}"); }
            if (intolerance != null) { query.Append($"&intolerances={intolerance.ToString()}"); }
            if (maxCalorie.HasValue) { query.Append($"&maxCalories={maxCalorie}"); }
            if (maxCarb.HasValue) { query.Append($"&maxCarbs={maxCarb}"); }
            if (maxProtein.HasValue) { query.Append($"&maxProtein={maxProtein}"); }
            if (minProtein.HasValue) { query.Append($"&minProtein={minProtein}"); }
            var response = await _httpClient.GetAsync($"/recipes/complexSearch?apiKey={_config.ApiKey}{query}&number=25");
            var recipe = JsonConvert.DeserializeObject<ReciepeApiResults>(await response.Content.ReadAsStringAsync());
            return recipe.Results;
        }     
        
        public async Task<List<Recipe>> SearchForAllRecipes()
        {
            var response = await _httpClient.GetAsync($"/recipes/complexSearch?apiKey={_config.ApiKey}&number=25");
            var recipe = JsonConvert.DeserializeObject<ReciepeApiResults>(await response.Content.ReadAsStringAsync());
            return recipe.Results;
        }
    }
}
