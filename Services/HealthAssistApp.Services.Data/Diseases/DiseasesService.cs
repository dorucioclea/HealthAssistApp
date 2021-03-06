﻿// <copyright file="DiseasesService.cs" company="HealthAssistApp">
// Copyright (c) HealthAssistApp. All Rights Reserved.
// </copyright>

namespace HealthAssistApp.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using HealthAssistApp.Data.Common.Repositories;
    using HealthAssistApp.Data.Models;
    using HealthAssistApp.Data.Models.DiseaseModels;
    using HealthAssistApp.Data.Models.Enums;
    using HealthAssistApp.Services.Mapping;
    using Microsoft.EntityFrameworkCore;

    public class DiseasesService: IDiseasesService
    {
        private readonly IRepository<Disease> diseaseRepository;
        private readonly IRepository<HealthDosierDisease> healthDosierDiseaseRepository;
        private readonly IRepository<DiseaseSymptom> diseaseSymptomRepository;

        public DiseasesService(
            IRepository<Disease> diseaseRepository,
            IRepository<HealthDosierDisease> healthDosierDiseaseRepository,
            IRepository<DiseaseSymptom> diseaseSymptomRepository)
        {
            this.diseaseRepository = diseaseRepository;
            this.healthDosierDiseaseRepository = healthDosierDiseaseRepository;
            this.diseaseSymptomRepository = diseaseSymptomRepository;
        }

        public async Task<int> CreateAsync(
            string name,
            string description,
            string advice,
            GlycemicIndex? index)
        {
            var newDisease = new Disease
            {
                Name = name,
                Description = description,
                Advice = advice,
                GlycemicIndex = index,
            };
            await this.diseaseRepository.AddAsync(newDisease);
            await this.diseaseRepository.SaveChangesAsync();
            return newDisease.Id;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
        {
            var query = await
                this.diseaseRepository
                .All()
                .To<T>()
                .ToListAsync();

            return query;
        }

        public IEnumerable<T> GetAllPaginatedAsync<T>(int? take, int skip)
        {
            var query = this.diseaseRepository
                .All()
                .OrderByDescending(x => x.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.To<T>().ToList();
        }

        public async Task<T> GetByIdAsync<T>(int id)
        {
            var disease = await this.diseaseRepository
                .All()
                .Where(d => d.Id == id)
                .To<T>()
                .FirstOrDefaultAsync();

            return disease;
        }

        public IEnumerable<T> GetByHealthDosier<T>(string healthDosierId)
        {
            var diseases =
                this.healthDosierDiseaseRepository.All()
                .Where(d => d.HealthDosierId == healthDosierId)
                .Select(d => new
                {
                    d.Disease.Id,
                    d.Disease.Name,
                    d.Disease.Description,
                    d.Disease.Advice,
                    d.Disease.GlycemicIndex,
                    d.Disease.DiseaseSymptoms,
                });

            return diseases.To<T>().ToList();
        }

        public async Task<T> GetByNameAsync<T>(string name)
        {
            var disease = await this.diseaseRepository.All().Where(x => x.Name == name)
                .To<T>().FirstOrDefaultAsync();
            return disease;
        }

        public async Task<int> GetDiseasesCountAsync()
        {
            var diseases = this.diseaseRepository
                .All();

            return diseases.Count();
        }

        public async Task<int> ModifyDiseaseAsync(
            int id,
            string name,
            string description,
            string advice,
            bool isDeleted)
        {
            var disease = await this.diseaseRepository
                .All()
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();

            disease.Name = name;
            disease.Description = description;
            disease.Advice = advice;
            disease.IsDeleted = isDeleted;

            this.diseaseRepository.Update(disease);
            await this.diseaseRepository.SaveChangesAsync();
            return disease.Id;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var disease = await this.diseaseRepository
                .All()
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();

            this.diseaseRepository.Delete(disease);
            await this.diseaseRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> DiseasesDropDownMenuAsync<T>()
        {
            var query = await
                this.diseaseRepository.All()
                .To<T>()
                .ToListAsync();

            return query;
        }

        public async Task<string> CreateHealthDosierDiseaseAsync(int diseaseId, string healthDosierId)
        {
            var healthDosierDisease = new HealthDosierDisease
            {
                HealthDosierId = healthDosierId,
                DiseaseId = diseaseId,
            };

            await this.healthDosierDiseaseRepository.AddAsync(healthDosierDisease);
            await this.healthDosierDiseaseRepository.SaveChangesAsync();
            return healthDosierId;
        }

        public async Task DeleteHealthDosierDiseasesByHealthIdAsync(string id)
        {
            var healthDosierDiseases = await this.healthDosierDiseaseRepository
                .AllAsNoTracking()
                .Where(h => h.HealthDosierId == id)
                .ToListAsync();

            foreach (var item in healthDosierDiseases)
            {
                this.healthDosierDiseaseRepository.Delete(item);
                await this.healthDosierDiseaseRepository.SaveChangesAsync();
            }
        }

        public async Task CreateDiseaseSymptomAsync(int diseaseId, int symptomId)
        {
            var diseaseSymptom = new DiseaseSymptom
            {
                DiseaseId = diseaseId,
                SymptomId = symptomId,
            };

            await this.diseaseSymptomRepository.AddAsync(diseaseSymptom);
            await this.diseaseSymptomRepository.SaveChangesAsync();
        }

        public async Task DeleteDiseaseSymptomAsync(string idS)
        {
            var separatedIdS = idS.Split("X");
            int diseaseId = int.Parse(separatedIdS[0]);
            int symptomsId = int.Parse(separatedIdS[1]);
            var diseaseSymptom = await this.diseaseSymptomRepository
                .AllAsNoTracking()
                .Where(d => d.DiseaseId == diseaseId & d.SymptomId == symptomsId)
                .FirstOrDefaultAsync();
            this.diseaseSymptomRepository.Delete(diseaseSymptom);
            await this.diseaseSymptomRepository.SaveChangesAsync();
        }

        public async Task<T> GetDiseaseSymptomAsync<T>(string idS)
        {
            var separatedIdS = idS.Split("X");
            int diseaseId = int.Parse(separatedIdS[0]);
            int symptomsId = int.Parse(separatedIdS[1]);
            var diseaseSymptom = await this.diseaseSymptomRepository
                .AllAsNoTracking()
                .Where(d => d.DiseaseId == diseaseId & d.SymptomId == symptomsId)
                .To<T>()
                .FirstOrDefaultAsync();

            //var list = new List<string>(

            return diseaseSymptom;
        }

        public async Task<IEnumerable<T>> GetAllDiseaseSymptomsAsync<T>()
        {
            var diseaseSymptom = await this.diseaseSymptomRepository
                .AllAsNoTracking()
                .To<T>()
                .ToListAsync();

            return diseaseSymptom;
        }
    }
}
