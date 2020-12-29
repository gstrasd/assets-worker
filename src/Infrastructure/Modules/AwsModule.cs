using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Library.Amazon;
using Library.Queuing;
using Serilog;
using Library.Configuration;
using Library.Storage;

namespace Infrastructure.Modules
{
    public class AwsModule : Module
    {
        private readonly HostBuilderContext _context;

        public AwsModule(HostBuilderContext context)
        {
            _context = context;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var accessKey = _context.Configuration.GetValue<string>("AWS:AccessKey");
                    var secretKey = _context.Configuration.GetValue<string>("AWS:SecretKey");
                    var options = _context.Configuration.GetAWSOptions("AWS");
                    options.Credentials = new BasicAWSCredentials(accessKey, secretKey);
                    return options;
                })
                .Named<AWSOptions>("Local")
                .SingleInstance();

            builder.Register((context, parameters) =>
                {
                    var environment = _context.HostingEnvironment.EnvironmentName;
                    var awsOptions = context.ResolveNamed<AWSOptions>(environment);
                    var sqsClient = awsOptions.CreateServiceClient<IAmazonSQS>();
                    var configuration = parameters.Named<IConfiguration>("configuration").Bind<SqsQueueClientConfiguration>();
                    var client = new SqsQueueClient(sqsClient, configuration);
                    return client;
                })
                .As<IQueueClient>()
                .InstancePerDependency();

            builder.Register((context, parameters) =>
                {
                    var environment = _context.HostingEnvironment.EnvironmentName;
                    var awsOptions = context.ResolveNamed<AWSOptions>(environment);
                    var s3Client = awsOptions.CreateServiceClient<IAmazonS3>();
                    var configuration = parameters.Named<IConfiguration>("configuration").Bind<S3StorageClientConfiguration>();
                    var client = new S3StorageClient(s3Client, configuration);
                    return client;
                })
                .As<IStorageClient>()
                .InstancePerDependency();
        }
    }
}
