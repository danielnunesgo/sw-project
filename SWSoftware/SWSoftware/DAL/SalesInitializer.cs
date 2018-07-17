using SWSoftware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SWSoftware.DAL
{
    public class SalesInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<SalesContext>
    {
        protected override void Seed(SalesContext context)
        {
            var salesList = new List<Sale>
            {
                new Sale{ID = 1, SaleDescription = "Pague 1 e Leve 2" },
                new Sale{ID = 2, SaleDescription = "3 por 10 reais"}
            };

            salesList.ForEach(w => context.Sales.Add(w));

            context.SaveChanges();
        }
    }
}