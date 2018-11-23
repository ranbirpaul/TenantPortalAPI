using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model;
using Repository.Abstract;
using Repository.Concrete;
using Serilog;
using Service.Abstract;
using Service.Concrete;
using Swashbuckle.AspNetCore.Swagger;
using TenantWebAPI;
using Okta.AspNetCore;

namespace TestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
		
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
        {
			// For example only! Don't store your shared keys as strings in code.
			// Use environment variables or the .NET Secret Manager instead.
			string jwtsecretkey = Configuration.GetSection("JwtSecretKey:key").Value;
			var sharedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsecretkey));

			// Step 1 Add JWT bearer token
		    services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
				options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
				options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
			})
			.AddOktaWebApi(new OktaWebApiOptions()
			{
				OktaDomain = Configuration.GetSection("Okta:TokenUrl").Value,
				ClientId = Configuration.GetSection("Okta:ClientId").Value,
				ClockSkew = TimeSpan.FromMinutes(15)
			});

			services.AddMvc();

			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "Tenant Portal API Service", Version = "v1" });
			});

			// Globally implement exception filter
			// services.AddMvc(options => { options.Filters.Add(new ApiExceptionFilter()); });
			// Injecting Settings in the Options accessor model
			services.Configure<Settings>(Options =>
            {
                Options.Host = Configuration.GetSection("MongoConnection:Host").Value;
                Options.Port = Configuration.GetSection("MongoConnection:Port").Value;
                Options.Database = Configuration.GetSection("MongoConnection:Database").Value;
                Options.UserName = Configuration.GetSection("MongoConnection:UserName").Value;
                Options.Password = Configuration.GetSection("MongoConnection:Password").Value;
				Options.TokenUrl = Configuration.GetSection("Okta:TokenUrl").Value;
				Options.ClientId = Configuration.GetSection("Okta:ClientId").Value;
				Options.ClientSecret = Configuration.GetSection("Okta:ClientSecret").Value;
			});

            // DI in controller constructor
            services.AddTransient<IRackRepository, RackRepository>();
            services.AddTransient<IRackService, RackService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tenant Portal API Service V1");
				c.RoutePrefix = string.Empty;
			});
			Log.Logger = new LoggerConfiguration()
					.WriteTo.RollingFile(@"D:\TenantLog\tenantportallog-{Date}.log")
					.CreateLogger();

			DefaultFilesOptions DefaultFile = new DefaultFilesOptions();
			DefaultFile.DefaultFileNames.Clear();
			DefaultFile.DefaultFileNames.Add("Index.html");
			app.UseDefaultFiles(DefaultFile);
			app.UseStaticFiles();

			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			app.UseMiddleware(typeof(ErrorHandlingMiddleware));
			// Step 2 JWT Authorization
			app.UseAuthentication();
			app.UseMvc();
		}
    }
}
