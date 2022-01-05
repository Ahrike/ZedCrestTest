using System;
using System.Collections.Generic;
using Api.Middleware;
using Application.Interfaces;
using Application.UserHandler;
using Infrastructure.Documents;
using Infrastructure.Email;
using Infrastructure.RabbitMQ;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQConsumers.ConsumerDocumentEmail;
using RabbitMQConsumers.ConsumerDocumentEmail2;
using ZedCrestTest.Persistence.DBContexts;

namespace ZedCrestTest
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

            services.AddControllers();
            
            services.AddDbContext<ZedCrestContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseMySql(Configuration.GetConnectionString("ZedCrestDBConn"),
                new MySqlServerVersion(new Version(8, 0, 19)));
            });
            services.AddMediatR(typeof(Register.Handler).Assembly);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ZedCrestTest", Version = "v1" });
            });
            services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
            services.Configure<SendGridSettings>(Configuration.GetSection("SendGrid"));
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMq"));

            services.AddScoped<IDocumentsAccessor, DocumentsAccessor>();
            services.AddTransient<ISendEmailServiceA, SenderEmailSendGrid>();
            services.AddTransient<ISendEmailServiceB, SendEmail>();
            services.AddTransient<IUserDocumentEmailPulisher, UserDocumentEmailPulisher>();

              /* 
               *consumer background services 
            */
            services.AddHostedService<ConsumerDocumentEmail>();
            services.AddHostedService<ConsumerDocumentEmail2>();
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZedCrestTest v1"));
            }

            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
