﻿using Aplicacao.Domain.Aggregate.Customers.Interfaces.Repositories;
using Aplicacao.Domain.Aggregate.Order.Interfaces.Repositories;
using Aplicacao.Domain.Aggregate.Order.Interfaces.Services;
using Aplicacao.Domain.Aggregate.Order.Validations;
using Aplicacao.Domain.Aggregate.Product.Interfaces.Repositories;
using Aplicacao.Domain.Services;
using Aplicacao.Domain.UoW;
using FluentValidation.Results;
using System.Threading.Tasks;

namespace Aplicacao.Domain.Aggregate.Order.Services
{
    public class OrderService : BaseService<Model.Order, int>, IOrderService
    {
        private readonly IUnitOfWork _uow;

        private readonly IOrderSQLServerRepository _sqlServerRepository;

        private readonly IProductSQLServerRepository _productSQLServerRepository;

        private readonly ICustomerSQLServerRepository _customerSQLServerRepository;

        private readonly IOrderRedisRepository _redisRepository;

        private readonly OrderValidator _validationRules;

        public OrderService(
            IUnitOfWork uow,
            IOrderSQLServerRepository sqlServerRepository,
            IOrderRedisRepository redisRepository,
            ICustomerSQLServerRepository customerSQLServerRepository,
            IProductSQLServerRepository productSQLServerRepository,
            OrderValidator validationRules)
            : base(uow, sqlServerRepository, redisRepository, validationRules)
        {
            _uow = uow;
            _sqlServerRepository = sqlServerRepository;
            _redisRepository = redisRepository;
            _validationRules = validationRules;
            _customerSQLServerRepository = customerSQLServerRepository;
            _productSQLServerRepository = productSQLServerRepository;
        }

        public override async Task<Model.Order> Add(Model.Order entity)
        {
            var idCustomer = entity.Customer.Id;
            entity.Customer = await _customerSQLServerRepository.ReadById(idCustomer);

            if (entity.Customer is null)
            {
                entity.ValidationResult.Errors.Add(new ValidationFailure("Id do cliente", $"Cliente id {idCustomer} inexistente"));
            }

            foreach (var item in entity.OrderItems)
            {
                Product.Model.Product product = await _productSQLServerRepository.ReadById(item.ProductId);

                if (product is null)
                {
                    entity.ValidationResult.Errors.Add(new ValidationFailure("Id do Produto", $"Produto id {item.ProductId} inexistente"));
                }
            }

            if (entity.ValidationResult.Errors.Count > 0)
            {
                return entity;
            }

            return await base.Add(entity);
        }
    }
}