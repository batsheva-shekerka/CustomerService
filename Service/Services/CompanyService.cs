using AutoMapper;
using Common.Dto;
using DataContext;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class CompanyService:Iservice<CompanyDto>,ICompanyService
    {
        private readonly IRepository<Company> repository;
        private readonly ICompanyRepository companyRepository;
        private readonly IMapper mapper;
        private readonly CustomerServiceContext _context;

        public CompanyService(IRepository<Company> repository,IMapper mapper,CustomerServiceContext context,ICompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository;
            this.repository = repository;
            this.mapper = mapper;
            this._context = context;
        }

        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var rep = await repository.GetAllAsync();
            return  mapper.Map<List<CompanyDto>>(rep);
        }
        public async Task<CompanyDto> GetByIdAsync(int id)
        {
            var rep = await repository.GetByIdAsync(id);
            return mapper.Map<CompanyDto>(rep);

        }

        public async Task<CompanyDto> AddAsync(CompanyDto item)
        {
            var rep = await repository.AddAsync(mapper.Map<Company>(item));
            return mapper.Map<CompanyDto>(rep);


        }
        public async Task<CompanyDto> UpdateAsync(int id, CompanyDto item)
        {
            var rep = await repository.UpdateAsync(id, mapper.Map<Company>(item));

            return mapper.Map<CompanyDto>(rep);
        }
        public async Task DeleteAsync(int id)
        {
            await repository.DeleteAsync(id);
        }

        public async Task<List<ScoreDto>> GetWeeklyImprovementAsync(int id)
        {
            var rep = await companyRepository.GetWeeklyImprovementAsync(id);
            return mapper.Map<List<ScoreDto>>(rep);
        }

    }
}
