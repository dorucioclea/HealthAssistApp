﻿// <copyright file="BodySystemsController.cs" company="HealthAssistApp">
// Copyright (c) HealthAssistApp. All Rights Reserved.
// </copyright>

namespace HealthAssistApp.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using HealthAssistApp.Data;
    using HealthAssistApp.Data.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class BodySystemsController : AdministrationController
    {
        private readonly ApplicationDbContext db;

        public BodySystemsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            return this.View(await this.db.BodySystems.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BodySystem bodySystem)
        {
            await this.db.AddAsync(bodySystem);
            await this.db.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var bodySystem = await this.db.BodySystems.FindAsync(id);
            if (bodySystem == null)
            {
                return this.NotFound();
            }

            return this.View(bodySystem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BodySystem bodySystem)
        {
            if (id != bodySystem.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                this.db.Update(bodySystem);
                await this.db.SaveChangesAsync();
            }

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var bodySystem = await this.db.BodySystems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bodySystem == null)
            {
                return this.NotFound();
            }

            return this.View(bodySystem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var bodySystem = await this.db.BodySystems.FindAsync(id);
            this.db.BodySystems.Remove(bodySystem);
            await this.db.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var bodySystem = await this.db.BodySystems.FindAsync(id);
            if (bodySystem == null)
            {
                return this.NotFound();
            }

            return this.View(bodySystem);
        }
    }
}