using Entities.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repository.Extensions.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Repository.Extensions

{
    public static class RepositoryCompanyExtensions
    {
        public static IQueryable<Company> Search(this IQueryable<Company> companies, string searchTerm) 
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return companies;
            return companies.Where(c => c.Name.Contains(searchTerm));
        }
        public static IQueryable<Company> Sort(this IQueryable<Company> companies,string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return companies;
            }
            var queryString = OrderQueryBuilder.CreateOrderQuery<Company>(orderByQueryString);
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return companies;
            }
            return companies.OrderBy(queryString);


        }
    }
}
