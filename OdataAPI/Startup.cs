using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using OdataAPI.Controllers;
using OdataAPI.Data;

namespace OdataAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TestDbContext>(opt => opt.UseInMemoryDatabase("Test"));

            services.AddControllers(mvcOptions => mvcOptions.EnableEndpointRouting = false);

            services.AddOData();
            services.AddODataQueryFilter();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseODataBatching();

            app.Use((context, next) =>
            {
                context.Response.Headers["OData-Version"] = "4.0";
                return next.Invoke();
            });


            app.UseMvc(routes =>
            {
                routes.Select().Expand().Filter().OrderBy().MaxTop(null).Count().EnableContinueOnErrorHeader(); 
                routes.MapODataServiceRoute("odata", "odata", GetEdmModel(), new DefaultODataBatchHandler());
                routes.EnableDependencyInjection();
            });


            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

                db.Suppliers.AddRange(new List<Supplier> { 
                        new Supplier { Id = 1, Name = "Stonka", Products =  new List<Product> {CreateNewProduct(1,"Cola", 130, "drink",1), CreateNewProduct(2,"Fanta", 140, "drink", 1) } },
                        new Supplier { Id = 2, Name = "Biedronka", Products =  new List<Product> {CreateNewProduct(3,"Pepsi", 130, "drink",2), CreateNewProduct(4,"Sprajt", 40, "drink", 2) } },

                });
                db.SaveChanges();
            }


        }

        IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Product>("Products").EntityType.Filter().Count().Expand().OrderBy().Page().Select(); 
            odataBuilder.EntitySet<Supplier>("Suppliers").EntityType.Filter().Count().Expand().OrderBy().Page().Select(); ;

            odataBuilder.Namespace = "ProductService";
            odataBuilder.EntityType<Product>().Action("Rate").Parameter<int>("Rating");
            odataBuilder.EntityType<Product>().Collection.Function("MostExpensive").Returns<double>();

            return odataBuilder.GetEdmModel();
        }

        static Product CreateNewProduct(int id,string name, decimal price, string categ, int SupplierId)
        {
            return new Product
            {
                Id = id,
                Name = name,
                Price = price,
                Category = categ,
                SupplierId = SupplierId
            };
        }

    }
}
