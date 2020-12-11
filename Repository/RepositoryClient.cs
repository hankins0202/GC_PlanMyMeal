﻿using GC_PlanMyMeal.Areas.Identity.Data;
using GC_PlanMyMeal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC_PlanMyMeal.Repository
{
    public class RepositoryClient: IRepositoryClient
    {
        private readonly GC_PlanMyMealIdentityDbContext _context;

        public RepositoryClient(GC_PlanMyMealIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveRecipe(int? recipeId, string userId)
        {
            var savedRecipe = new SavedRecipe()
            {
                UserId = userId,
                RecipeId = recipeId
            };
            try
            {
                _context.SavedRecipes.Add(savedRecipe);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }         
            return true;
        }

        public async Task<bool> FindSavedRecipe(int recipeId, string userId)
        {
            var result = await _context.SavedRecipes.FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);
            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<List<SavedRecipe>> RetrieveRecipeList(string userId)
        {
            var result = await _context.SavedRecipes.Where(r => r.UserId == userId).ToListAsync();
            return result;
        }

        public async Task<bool> DeleteRecipe(string userId, int? recipeId, int? customId)
        {
            try
            {
                var recipe = await _context.SavedRecipes.FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);
                _context.SavedRecipes.Remove(recipe);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddCustomRecipe(CustomRecipe customRecipe)
        {
            try
            {
                await _context.CustomRecipes.AddAsync(customRecipe);
                _context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
