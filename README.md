# Send-and-Receive-Data-in-Asp.net-Core-With-RabbitMQ
Console app which sends complex data object as message to a RabbitMQ server and asp .net Core Web API that consume those data object message and send feedback.

There are so many examples about RabbitMQ in c#. But if you are going to implement it is asp.net core and if service needs to run automatically when api starts still can't find simple samples 

This is very simple way to implement RabbitMQ with asp .net core web API. Distributed messages handled by built in [RabbitMQ](https://github.com/rabbitmq/rabbitmq-dotnet-client) implementation.

Take this source code as sample and you can improve with advance functionalities

## Installation ##
You need to install [Erlang OTP](http://www.erlang.org/downloads) first and [RabbitMQ Server](http://www.rabbitmq.com/) later on your computer to run this application.

# Required NuGet Packages
```
PM> Install-Package RabbitMQ.Client
PM> Install-Package Newtonsoft.json
```



## Implementation ##
There is so many ecamples Statuup.cs.
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQReiverCoreAPI.RebbitMQ;

namespace RabbitMQReiverCoreAPI
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddSingleton<RabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }

                return new RabbitMQPersistentConnection(factory);
            });

            services.AddOptions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            //Initilize Rabbit Listener in ApplicationBuilderExtentions
            app.UseRabbitListener();
        }
    }
  
    public static class ApplicationBuilderExtentions
    {
        public static RabbitMQPersistentConnection Listener { get; set; }

        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            Listener = app.ApplicationServices.GetService<RabbitMQPersistentConnection>();
            var life = app.ApplicationServices.GetService<IApplicationLifetime>();
            life.ApplicationStarted.Register(OnStarted);

            //press Ctrl+C to reproduce if your app runs in Kestrel as a console app
            life.ApplicationStopping.Register(OnStopping);
            return app;
        }

        private static void OnStarted()
        {
            Listener.CreateConsumerChannel();
        }

        private static void OnStopping()
        {
            Listener.Disconnect();
        }
    }
}
```
