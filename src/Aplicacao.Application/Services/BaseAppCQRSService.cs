﻿using Aplicacao.Application.Interfaces;
using Aplicacao.Domain.Interfaces.Services;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplicacao.Application.Services
{
    public abstract class BaseAppCQRSService<T, D, Tid> : IAppCQRSService<D, Tid>
        where T : class
        where D : class
    {
        protected readonly IMapper _mapper;
        protected readonly IService<T, Tid> _baseService;

        protected BaseAppCQRSService(IService<T, Tid> baseService, IMapper mapper)
        {
            _mapper = mapper;
            _baseService = baseService;
        }

        public async Task<D> Add(D itemDTO)
        {
            var itemMap = _mapper.Map<D, T>(itemDTO);
            var item = await _baseService.Add(itemMap);
            itemDTO = _mapper.Map<T, D>(item);
            return itemDTO;
        }

        public async Task<bool> Remover(Tid tid)
        {
            return await _baseService.Delete(tid);
        }

        public async Task<D> Update(D itemDTO)
        {
            var itemMap = _mapper.Map<D, T>(itemDTO);
            var item = await _baseService.Update(itemMap);
            itemDTO = _mapper.Map<T, D>(item);

            return itemDTO;
        }
    }
}