using BookStoreCMS.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using RedisSystem.Config;
using DAO.Utitlities;
using ShareData.Helper;
using LoggerService;
using BookStoreCMS.Utils;
using BookStoreCMS.Instance;

namespace CoreWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SetupConfig(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureIISIntegration();
            services.ConfigureLoggerService();
            services.ConfigureVersioning();
            services.ConfigureResponseCaching();
            services.ConfigureMailService();
            services.ConfigureTokenService();

            services.AddControllers().AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                }
            );
        }

        private void SetupConfig(IConfiguration Configuration)
        {
            NO_SQL_CONFIG.InitializeCMS(Configuration.GetSection("Mongo")["IpAddress"], Configuration.GetSection("GHN")["CONNECTION"], Configuration.GetSection("GHN")["Token"], Configuration.GetSection("GHN")["ShopId"],
                Configuration.GetSection("Adress")["from_name"], Configuration.GetSection("Adress")["from_phone"], Configuration.GetSection("Adress")["from_address"], Configuration.GetSection("Adress")["from_ward_name"], Configuration.GetSection("Adress")["from_district_name"], Configuration.GetSection("Adress")["from_district_code"], Configuration.GetSection("Adress")["from_province_name"]);
            RedisConfig.Initialize(Configuration.GetSection("RedisConfig")["RedisIp"], Configuration.GetSection("RedisConfig")["RedisPort"], Configuration.GetSection("RedisConfig")["RedisPassword"]);
            ConfigDb.Initialize(Configuration.GetSection("DbConfig")["CONNECTION"], Configuration.GetSection("DbConfig")["SQLPASS"]);
            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            CONFIG.Initialize(Configuration.GetSection("CONFIG")["SecretTokenKey"], Configuration.GetSection("CONFIG")["BaseLink"], emailConfig);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseResponseCaching();
            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
